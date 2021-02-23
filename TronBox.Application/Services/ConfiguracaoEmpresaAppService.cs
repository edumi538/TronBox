using AutoMapper;
using Comum.Aggregates.PessoaAgg;
using Comum.Domain.Aggregates.EmpresaAgg;
using Comum.Domain.Aggregates.EmpresaAgg.Repository;
using Comum.Domain.Aggregates.PessoaAgg;
using Comum.Domain.Aggregates.PessoaAgg.Repository;
using Comum.Domain.Enums;
using Comum.Domain.Interfaces;
using Comum.Domain.ViewModels;
using Comum.DTO;
using Comum.DTO.Usuario;
using FluentValidation.Results;
using Newtonsoft.Json;
using Sentinela.Domain.DTO;
using Sentinela.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sentinela.Domain.Aggregates.TenantAgg;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.Aggregates.ConfiguracaoEmpresaAgg;
using TronBox.Domain.Aggregates.ConfiguracaoEmpresaAgg.Repository;
using TronBox.Domain.Aggregates.ConfiguracaoUsuarioAgg;
using TronBox.Domain.Aggregates.ConfiguracaoUsuarioAgg.Repository;
using TronBox.Domain.DTO;
using TronBox.Domain.Enums;
using TronCore.DefinicoesConfiguracoes;
using TronCore.Dominio.Base;
using TronCore.Dominio.Bus;
using TronCore.Dominio.Notifications;
using TronCore.Enumeradores;
using TronCore.InjecaoDependencia;
using TronCore.Persistencia.Interfaces;
using TronCore.Utilitarios;
using TronCore.Utilitarios.Email;

namespace TronBox.Application.Services
{
    public class ConfiguracaoEmpresaAppService : IConfiguracaoEmpresaAppService
    {
#if DEBUG
        public static string URL_FILA_EMPRESA = "http://10.20.30.33:5000";
#else
        public static string URL_FILA_EMPRESA = "http://10.20.30.33:3000";
#endif

        #region Membros
        private readonly IBus _bus;
        private readonly IMapper _mapper;
        private readonly IPessoaUsuarioLogado _usuarioLogado;
        private readonly IRepositoryFactory _repositoryFactory;
        #endregion

        #region Construtor
        public ConfiguracaoEmpresaAppService(IBus bus, IMapper mapper, IRepositoryFactory repositoryFactory, IPessoaUsuarioLogado usuarioLogado)
        {
            _bus = bus;
            _mapper = mapper;
            _usuarioLogado = usuarioLogado;
            _repositoryFactory = repositoryFactory;
        }
        #endregion

        public void Dispose()
        {
        }

        public EmpresaDTO BuscarEmpresa()
        {
            var empresa = _mapper.Map<EmpresaDTO>(_repositoryFactory.Instancie<IEmpresaRepository>().BuscarTodos().FirstOrDefault());

            empresa.ConfiguracaoEmpresa = _mapper.Map<ConfiguracaoEmpresaDTO>(BuscarConfiguracaoEmpresa());

            var certificado = UtilitarioHttpClient.GetRequest(_usuarioLogado.GetToken(), Constantes.URI_BASE_CT,
                $"api/v1/certificados/{empresa.Inscricao}").GetAwaiter().GetResult();

            if (certificado != null)
                empresa.Certificado = JsonConvert.DeserializeObject<CertificadoSimplificadoDTO>(certificado);

            return empresa;
        }

        public void AtualizarEmpresa(EmpresaDTO empresaDto)
        {
            var empresaRepository = _repositoryFactory.Instancie<IEmpresaRepository>();
            
            var empresaExistente = empresaRepository.BuscarTodos().FirstOrDefault();
            var empresa = _mapper.Map<Empresa>(empresaDto);

            empresa.Id = empresaExistente.Id;
            empresa.Inscricao = empresa.Inscricao.RemoveMascaras();

            var configuracaoEmpresa = _mapper.Map<ConfiguracaoEmpresa>(empresaDto.ConfiguracaoEmpresa);

            if (configuracaoEmpresa != null)
            {
                configuracaoEmpresa.InscricoesComplementares = configuracaoEmpresa.InscricoesComplementares
                    .Select(c =>
                    {
                        if (c.Id == Guid.Empty)
                            c.Id = Guid.NewGuid();

                        return c;
                    });
            }

            if (!EhValido(configuracaoEmpresa)) return;
            var configuracaoExistente = BuscarConfiguracaoEmpresa();

            if (configuracaoExistente != null)
            {
                configuracaoEmpresa.Id = configuracaoExistente.Id;
                _repositoryFactory.Instancie<IConfiguracaoEmpresaRepository>().Atualizar(configuracaoEmpresa);
            }
            else _repositoryFactory.Instancie<IConfiguracaoEmpresaRepository>().Inserir(configuracaoEmpresa);

            if ((int)empresa.TipoInscricao > 0)
                empresaRepository.Atualizar(empresa);

            ExcluirUsuarioCriarNovo(empresaExistente, empresa.EmailPrincipal);

            var consultarCte = configuracaoEmpresa.SalvarCteEntrada || configuracaoEmpresa.SalvarCteSaida;

            if (!consultarCte) RemoverEmpresaFilaCte(empresa.Inscricao);
        }

        public async Task AtualizarTodasCredenciaisPortalEstadual(IEnumerable<Guid> tenantIds)
        {
            var tenantsList = tenantIds?.ToList();
            
            if (tenantsList == null || !tenantsList.Any())
            {
                _bus.RaiseEvent(new DomainNotification("SemBases", "Nenhuma base foi selecionada!"));
                return;
            }

            var configReferencia = BuscarConfiguracaoEmpresa();

            if (configReferencia.DadosMatoGrosso == null && configReferencia.DadosMatoGrossoSul == null)
            {
                _bus.RaiseEvent(new DomainNotification("NaoConfigurado", "A empresa de referência não está configurada"));
                return;
            }

            var mobileToken = await UtilitarioHttpClient.ObtenhaCredencialRootAsync();

            var requestPath = $"/api/usuarios/{_usuarioLogado.GetUsuarioId()}/tenants-modulo/{(int)eModuloTron.BX}";
            var result = await UtilitarioHttpClient.GetRequest(mobileToken,
                Constantes.URI_BASE_ST_API,
                requestPath);
            
            var tenants = JsonConvert.DeserializeObject<List<Tenant>>(result);
            
            var tenantsSelecionados = tenants.Where(t => tenantsList.Contains(t.Id));
          
            var tp = FabricaGeral.Instancie<ITenantProvider>();
            foreach (var tnt in tenantsSelecionados)
            {
                tp.SetTenantTmp(tnt);
                var configuracaoEmpresa = BuscarConfiguracaoEmpresa();

                if (configuracaoEmpresa.DadosMatoGrosso == null && configuracaoEmpresa.DadosMatoGrossoSul == null)
                    continue;
                
                if (configuracaoEmpresa.DadosMatoGrosso != null && configReferencia.DadosMatoGrosso != null)
                    configuracaoEmpresa.DadosMatoGrosso = configReferencia.DadosMatoGrosso;

                if (configuracaoEmpresa.DadosMatoGrossoSul != null && configReferencia.DadosMatoGrossoSul != null)
                    configuracaoEmpresa.DadosMatoGrossoSul = configReferencia.DadosMatoGrossoSul;
                
                _repositoryFactory.Instancie<IConfiguracaoEmpresaRepository>()
                    .Atualizar(configuracaoEmpresa);
            }
            
            tp.SetTenantTmp(null);
        }


        public async Task<Resposta> Upload(CertificadoCreateDTO certificadoCreateDTO)
        {
            var dictionary = new Dictionary<string, dynamic>
            {
                { "inscricaoEmpresa", certificadoCreateDTO.InscricaoEmpresa.RemoveMascaras() },
                { "senha", certificadoCreateDTO.Senha },
                { "arquivo", certificadoCreateDTO.Arquivo }
            };

            var result = await UtilitarioHttpClient.PostRequest(_usuarioLogado.GetToken(), Constantes.URI_BASE_CT, "api/v1/certificados", dictionary, "certificado.pfx");

            var resposta = JsonConvert.DeserializeObject<Resposta>(result);

            if (resposta.Sucesso)
            {
                var configuracaoEmpresa = BuscarConfiguracaoEmpresa();

                if (certificadoCreateDTO.ManifestarAutomaticamente)
                {
                    configuracaoEmpresa.ManifestarAutomaticamente = true;
                    _repositoryFactory.Instancie<IConfiguracaoEmpresaRepository>().Atualizar(configuracaoEmpresa);
                }

                InserirEmpresaFila(configuracaoEmpresa);
            }

            return resposta;
        }

        public async Task<Resposta> DeletarCertificado(Guid id)
        {
            var result = await UtilitarioHttpClient.DeleteRequest(_usuarioLogado.GetToken(), Constantes.URI_BASE_CT, $"api/v1/certificados/{id}");

            return JsonConvert.DeserializeObject<Resposta>(result);
        }

        public void AtualizarEmail(AtualizacaoEmailDTO atualizacaoEmail)
        {
            var empresa = _repositoryFactory.Instancie<IEmpresaRepository>().BuscarTodos().FirstOrDefault();

            ExcluirUsuarioCriarNovo(empresa, atualizacaoEmail.Email);

            if (empresa != null && (int)empresa.TipoInscricao > 0)
                _repositoryFactory.Instancie<IEmpresaRepository>().Atualizar(empresa);
        }

        public CertificadoSituacaoDTO SituacaoCertificado()
        {
            var empresa = _mapper.Map<EmpresaDTO>(_repositoryFactory.Instancie<IEmpresaRepository>().BuscarTodos().FirstOrDefault());

            return ObterSituacaoCertificadoEmpresa(empresa);
        }

        public async Task NotificarContadorAcessoInvalido(ESefazEstado estado)
        {
            var regex = new Regex(@"\bDB-(GC|CN|LR|BX)-[0-9]{4,6}@tron.com.br\b", RegexOptions.None);

            var empresa = _repositoryFactory.Instancie<IEmpresaRepository>().BuscarTodos().FirstOrDefault();
            var configuracoes = _repositoryFactory.Instancie<IConfiguracaoUsuarioRepository>().BuscarTodos(c => c.NotificarPortalEstadual);

            foreach (var configuracao in configuracoes)
            {
                var pessoa = _repositoryFactory.Instancie<IPessoaRepository>().BuscarPorExpressao(c => c.Cpf == configuracao.Inscricao);

                if (pessoa != null && !string.IsNullOrEmpty(pessoa.Email) && !regex.IsMatch(pessoa.Email))
                    await TemplateEmail.EnviarEmailSefazSenhaInvalida(pessoa.Email, empresa.RazaoSocial, estado);
            }
        }

        public void AlterarPessoa(PessoaViewModel pessoa)
        {
            var pessoaExistente = _repositoryFactory.Instancie<IPessoaRepository>().BuscarPorId(pessoa.Id);

            if (pessoaExistente != null)
            {
                if (pessoa.Cpf != pessoaExistente.Cpf)
                {
                    var configuracao = _repositoryFactory.Instancie<IConfiguracaoUsuarioRepository>().BuscarPorExpressao(c => c.Inscricao == pessoaExistente.Cpf);

                    if (configuracao != null)
                    {
                        configuracao.Inscricao = pessoa.Cpf;

                        _repositoryFactory.Instancie<IConfiguracaoUsuarioRepository>().Atualizar(configuracao);
                    }
                }

                _repositoryFactory.Instancie<IPessoaRepository>().Atualizar(_mapper.Map<Pessoa>(pessoa));
            }
        }

        public void CriarPessoa(PessoaUsuarioDTO pessoaUsuarioDto)
        {
            var pessoaId = InserirPessoa(pessoaUsuarioDto);

            InserirPessoaUsuario(pessoaUsuarioDto, pessoaId);

            InserirPessoaEmpresa(pessoaUsuarioDto, pessoaId);

            InserirConfiguracaoUsuario(pessoaUsuarioDto.Cpf);
        }

        #region Private Methods
        private ConfiguracaoEmpresa BuscarConfiguracaoEmpresa() => _repositoryFactory.Instancie<IConfiguracaoEmpresaRepository>().BuscarTodos().FirstOrDefault();

        private CertificadoSituacaoDTO ObterSituacaoCertificadoEmpresa(EmpresaDTO empresa)
        {
            var certificadoResposta = UtilitarioHttpClient.GetRequest(_usuarioLogado.GetToken(), Constantes.URI_BASE_CT,
                $"api/v1/certificados/{empresa.Inscricao}").GetAwaiter().GetResult();

            if (certificadoResposta == null) return new CertificadoSituacaoDTO(empresa.Inscricao, empresa.RazaoSocial);

            var certificado = JsonConvert.DeserializeObject<CertificadoSimplificadoDTO>(certificadoResposta);

            if (certificado == null) return new CertificadoSituacaoDTO(empresa.Inscricao, empresa.RazaoSocial);

            return new CertificadoSituacaoDTO()
            {
                InscricaoEmpresa = empresa.Inscricao,
                NomeEmpresa = empresa.RazaoSocial,
                DataVencimento = certificado.DataVencimento,
                Vencido = certificado.DataVencimento <= UtilitarioDatas.ConvertToIntDateTime(DateTime.Now)
            };
        }

        private void InserirEmpresaFila(Empresa empresa, ConfiguracaoEmpresa configuracaoEmpresa, Guid tenantId)
        {
            if (configuracaoEmpresa.InscricoesComplementares.Any())
            {
                foreach (var inscricaoComplementar in configuracaoEmpresa.InscricoesComplementares)
                {
                    if (inscricaoComplementar.ConsultaPortalEstadual && inscricaoComplementar.Situacao == eSituacao.Ativo && !string.IsNullOrEmpty(inscricaoComplementar.InscricaoEstadual))
                    {
                        if (configuracaoEmpresa.DadosMatoGrosso != null)
                            AdicionarFilaMatoGrosso(empresa, configuracaoEmpresa, inscricaoComplementar, tenantId);

                        if (configuracaoEmpresa.DadosMatoGrossoSul != null)
                            AdicionarFilaMatoGrossoSul(empresa, configuracaoEmpresa, inscricaoComplementar, tenantId);
                    }
                }
            }
        }

        private void InserirEmpresaFila(ConfiguracaoEmpresa configuracaoEmpresa)
        {
            if (configuracaoEmpresa.InscricoesComplementares.Any())
            {
                var empresa = _repositoryFactory.Instancie<IEmpresaRepository>().BuscarTodos().FirstOrDefault();

                var certificado = ObterSituacaoCertificadoEmpresa(_mapper.Map<EmpresaDTO>(empresa));

                if (certificado != null && !certificado.Vencido)
                {
                    var tenantId = FabricaGeral.Instancie<ITenantProvider>().GetTenant().Id;

                    InserirEmpresaFila(empresa, configuracaoEmpresa, tenantId);

                    foreach (var inscricaoComplementar in configuracaoEmpresa.InscricoesComplementares)
                    {
                        AdicionarFilaNfe(empresa, configuracaoEmpresa, inscricaoComplementar, tenantId);

                        if (configuracaoEmpresa.SalvarCteEntrada || configuracaoEmpresa.SalvarCteSaida)
                            AdicionarFilaCte(empresa, configuracaoEmpresa, inscricaoComplementar, tenantId);
                    }
                }
            }
        }

        private static void AdicionarFilaNfe(Empresa empresa, ConfiguracaoEmpresa configuracaoEmpresa, InscricaoComplementar inscricaoComplementar, Guid tenantId)
        {
            var ultimoNsu = FabricaGeral.Instancie<IHistoricoConsultaAppService>().ObterUltimoNSU(ETipoDocumentoConsulta.NFe);

            var empresaFila = new
            {
                id = tenantId,
                dataCriacao = DateTime.Now,
                nome = inscricaoComplementar.NomeFantasia ?? empresa.RazaoSocial,
                inscricaoEmpresa = empresa.Inscricao,
                uf = inscricaoComplementar.UF,
                ultimoNsu,
                configuracaoEmpresa = new
                {
                    manifestarAutomaticamente = configuracaoEmpresa.ManifestarAutomaticamente,
                    salvarSomenteManifestadas = configuracaoEmpresa.SalvarSomenteManifestadas,
                    metodoBusca = configuracaoEmpresa.MetodoBusca,
                }
            };

            UtilitarioHttpClient.PostRequest(string.Empty, URL_FILA_EMPRESA, "api/nfes", empresaFila);
        }

        private static void AdicionarFilaCte(Empresa empresa, ConfiguracaoEmpresa configuracaoEmpresa, InscricaoComplementar inscricaoComplementar, Guid tenantId)
        {
            var ultimoNsu = FabricaGeral.Instancie<IHistoricoConsultaAppService>().ObterUltimoNSU(ETipoDocumentoConsulta.CTe);

            var empresaFila = new
            {
                id = tenantId,
                dataCriacao = DateTime.Now,
                nome = inscricaoComplementar.NomeFantasia ?? empresa.RazaoSocial,
                inscricaoEmpresa = empresa.Inscricao,
                uf = inscricaoComplementar.UF,
                ultimoNsu,
                configuracaoEmpresa = new
                {
                    salvarCteEntrada = configuracaoEmpresa.SalvarCteEntrada,
                    salvarCteSaida = configuracaoEmpresa.SalvarCteSaida,
                    metodoBusca = configuracaoEmpresa.MetodoBusca,
                }
            };

            UtilitarioHttpClient.PostRequest(string.Empty, URL_FILA_EMPRESA, "api/ctes", empresaFila);
        }

        private static void AdicionarFilaMatoGrosso(Empresa empresa, ConfiguracaoEmpresa configuracaoEmpresa, InscricaoComplementar inscricaoComplementar, Guid tenantId)
        {
            var inscricaoEstadual = inscricaoComplementar.InscricaoEstadual.PadLeft(11, '0');

            var dataFinalConsultada = FabricaGeral.Instancie<IHistoricoConsultaMatoGrossoAppService>().ObterUltimoPeriodo(inscricaoEstadual);

            var dataInicial = dataFinalConsultada != null && dataFinalConsultada.HasValue
                ? UtilitarioDatas.ConvertToIntDate(dataFinalConsultada.Value.AddDays(-7))
                : configuracaoEmpresa.MetodoBusca == EMetodoBusca.UltimosTrintaDias
                    ? UtilitarioDatas.ConvertToIntDate(DateTime.Now.AddMonths(-1))
                    : UtilitarioDatas.ConvertToIntDate(DateTime.Now.AddMonths(-3));

            var dataFinal = UtilitarioDatas.ConvertToIntDate(DateTime.Now.AddDays(-1));

            var empresaFila = new
            {
                id = tenantId,
                dataCriacao = DateTime.Now,
                nome = inscricaoComplementar.NomeFantasia ?? empresa.RazaoSocial,
                inscricaoEmpresa = empresa.Inscricao,
                inscricaoEstadual,
                dadosMatoGrosso = new
                {
                    tipo = configuracaoEmpresa.DadosMatoGrosso.Tipo,
                    usuario = configuracaoEmpresa.DadosMatoGrosso.Usuario,
                    senha = configuracaoEmpresa.DadosMatoGrosso.Senha
                },
                dataInicial,
                dataFinal,
            };

            UtilitarioHttpClient.PostRequest(string.Empty, URL_FILA_EMPRESA, "api/mato-grosso", empresaFila);
        }

        private static void AdicionarFilaMatoGrossoSul(Empresa empresa, ConfiguracaoEmpresa configuracaoEmpresa, InscricaoComplementar inscricaoComplementar, Guid tenantId)
        {
            var dataFinalConsultada = FabricaGeral.Instancie<IHistoricoConsultaMatoGrossoSulAppService>().ObterUltimoPeriodo();

            var dataInicial = dataFinalConsultada != null && dataFinalConsultada.HasValue
                ? UtilitarioDatas.ConvertToIntDate(dataFinalConsultada.Value.AddDays(-7))
                : configuracaoEmpresa.MetodoBusca == EMetodoBusca.UltimosTrintaDias
                    ? UtilitarioDatas.ConvertToIntDate(DateTime.Now.AddMonths(-1))
                    : UtilitarioDatas.ConvertToIntDate(DateTime.Now.AddMonths(-3));

            var dataFinal = UtilitarioDatas.ConvertToIntDate(DateTime.Now.AddDays(-1));

            var empresaFila = new
            {
                id = tenantId,
                dataCriacao = DateTime.Now,
                nome = inscricaoComplementar.NomeFantasia ?? empresa.RazaoSocial,
                inscricaoEmpresa = empresa.Inscricao,
                dadosMatoGrossoSul = new
                {
                    usuario = configuracaoEmpresa.DadosMatoGrossoSul.Usuario,
                    codigoAcesso = configuracaoEmpresa.DadosMatoGrossoSul.CodigoAcesso,
                    senha = configuracaoEmpresa.DadosMatoGrossoSul.Senha
                },
                dataInicial,
                dataFinal,
            };

            UtilitarioHttpClient.PostRequest(string.Empty, URL_FILA_EMPRESA, "api/mato-grosso-sul", empresaFila);
        }

        private static void RemoverEmpresaFilaCte(string inscricaoEmpresa) => UtilitarioHttpClient.DeleteRequest(string.Empty, URL_FILA_EMPRESA, $"api/ctes/{inscricaoEmpresa}");

        private void ExcluirUsuarioCriarNovo(Empresa empresa, string novoEmail)
        {
            string emailRemocao = null;

            if ((empresa.EmailPrincipal != null) && (empresa.EmailPrincipal.Length > 0) && (novoEmail != empresa.EmailPrincipal))
                emailRemocao = empresa.EmailPrincipal;

            empresa.EmailPrincipal = novoEmail;

            CriarUsuarioEmpresa(empresa, emailRemocao);
        }

        private void CriarUsuarioEmpresa(Empresa empresa, string emailRemocao)
        {
            var pessoaId = AtualizarDadosPessoa(empresa.Inscricao, empresa.RazaoSocial, empresa.EmailPrincipal);

            if ((emailRemocao != null) && (emailRemocao.Length > 0))
                RemoverUsuarioBaseDados(emailRemocao, pessoaId);

            CriarRelacionamentoPessoaEmpresa(empresa.Id, pessoaId);

            if ((empresa.EmailPrincipal != null) && (empresa.EmailPrincipal.Length > 0))
            {
                var usuarioTenant = new UsuarioTenantDTO
                {
                    UsuarioEmail = empresa.EmailPrincipal,
                    TenantId = _usuarioLogado.GetTenantId(),
                    ClassificacaoFuncionario = eClassificacaoPessoa.Cliente.ToString()
                };

                PersistirUsuario(usuarioTenant, pessoaId);
            }
        }

        private Guid AtualizarDadosPessoa(string inscricao, string nome, string email)
        {
            var pessoa = _repositoryFactory.Instancie<IPessoaRepository>().BuscarPorExpressao(p => p.Cpf == inscricao);

            if (pessoa == null)
                pessoa = new Pessoa();

            pessoa.Nome = nome;
            pessoa.Cpf = inscricao;
            pessoa.Email = email;
            pessoa.Status = true;

            if (pessoa.Id == Guid.Empty)
            {
                pessoa.Id = Guid.NewGuid();
                _repositoryFactory.Instancie<IPessoaRepository>().Inserir(pessoa);
            }
            else
                _repositoryFactory.Instancie<IPessoaRepository>().Atualizar(pessoa);

            return pessoa.Id;
        }

        private void CriarRelacionamentoPessoaEmpresa(Guid empresaId, Guid pessoaId)
        {
            var pessoaEmpresa = _repositoryFactory.Instancie<IPessoaEmpresaRepository>()
                .BuscarPorExpressao(c => c.PessoaId == pessoaId && c.EmpresaId == empresaId);

            if (pessoaEmpresa == null)
            {
                pessoaEmpresa = new PessoaEmpresa()
                {
                    PessoaId = pessoaId,
                    EmpresaId = empresaId,
                    ClassificacaoFuncionario = eClassificacaoPessoa.Cliente
                };

                _repositoryFactory.Instancie<IPessoaEmpresaRepository>().Inserir(pessoaEmpresa);
            }
        }

        private void PersistirUsuario(UsuarioTenantDTO usuarioTenant, Guid pessoaId)
        {
            Guid usuarioId = CriarUsuarioSentinela(usuarioTenant);

            if (usuarioId.ToString() != Guid.Empty.ToString())
            {
                var pessoaUsuario = _repositoryFactory.Instancie<IPessoaUsuarioRepository>().BuscarPorUsuario(usuarioId);

                if (pessoaUsuario == null)
                {
                    pessoaUsuario = new PessoaUsuario
                    {
                        UsuarioId = usuarioId,
                        PessoaId = pessoaId
                    };

                    _repositoryFactory.Instancie<IPessoaUsuarioRepository>().Inserir(pessoaUsuario);
                }
            }
        }

        private Guid CriarUsuarioSentinela(UsuarioTenantDTO usuarioTenant)
        {
            var result = UtilitarioHttpClient.PostRequest(_usuarioLogado.GetToken(), UtilitarioHttpClient.APIBaseUriSentinela,
                "api/usuarios", usuarioTenant, _usuarioLogado.GetTenantId().ToString()).GetAwaiter().GetResult();

            var usuarioResult = JsonConvert.DeserializeObject<UsuarioDTOPost>(result);

            if (usuarioResult.Sucesso)
                return Guid.Parse(usuarioResult.UsuarioId);

            return default;
        }

        private void RemoverUsuarioBaseDados(string email, Guid pessoaId)
        {
            FabricaGeral.Instancie<IPessoaUsuarioRepository>().Excluir(pessoaId);

            var usuarioTenant = new UsuarioTenantDTO
            {
                UsuarioEmail = email,
                TenantId = _usuarioLogado.GetTenantId()
            };

            UtilitarioHttpClient.PutRequest(_usuarioLogado.GetToken(), UtilitarioHttpClient.APIBaseUriSentinela,
                $"api/usuarios/remover-tenant", usuarioTenant, _usuarioLogado.GetTenantId().ToString()).GetAwaiter().GetResult();
        }

        private bool EhValido(ConfiguracaoEmpresa configuracaoEmpresa)
        {
            var validator = (new ConfiguracaoEmpresaValidator()).Validate(configuracaoEmpresa);

            CriarMensagensErro(validator);

            return validator.IsValid;
        }

        private void CriarMensagensErro(ValidationResult validator)
        {
            foreach (var error in validator.Errors)
                _bus.RaiseEvent(new DomainNotification(error.PropertyName, error.ErrorMessage));
        }

        private Guid InserirPessoa(PessoaUsuarioDTO pessoaUsuarioDto)
        {
            var pessoa = new Pessoa()
            {
                Id = Guid.NewGuid(),
                Email = pessoaUsuarioDto.Email,
                Cpf = pessoaUsuarioDto.Cpf,
                Nome = pessoaUsuarioDto.Nome,
                Cep = pessoaUsuarioDto.Cep,
                Logradouro = pessoaUsuarioDto.Logradouro,
                Numero = pessoaUsuarioDto.Numero,
                Complemento = pessoaUsuarioDto.Complemento,
                Bairro = pessoaUsuarioDto.Bairro,
                TelefoneFixo = pessoaUsuarioDto.TelefoneFixo,
                TelefoneCelular = pessoaUsuarioDto.TelefoneCelular,
                Status = true,
            };

            _repositoryFactory.Instancie<IPessoaRepository>().Inserir(pessoa);

            return pessoa.Id;
        }

        private void InserirPessoaUsuario(PessoaUsuarioDTO pessoaUsuarioDto, Guid pessoaId)
        {
            var usuarioTenant = new UsuarioTenantDTO
            {
                UsuarioEmail = pessoaUsuarioDto.Email,
                TenantId = _usuarioLogado.GetTenantId(),
                ClassificacaoFuncionario = ObterClassificacao(pessoaUsuarioDto.Tipo).ToString()
            };

            PersistirUsuario(usuarioTenant, pessoaId);
        }

        private void InserirPessoaEmpresa(PessoaUsuarioDTO pessoaUsuarioDto, Guid pessoaId)
        {
            var pessoaEmpresa = new PessoaEmpresa
            {
                PessoaId = pessoaId,
                EmpresaId = pessoaUsuarioDto.EmpresaId,
                ClassificacaoFuncionario = ObterClassificacao(pessoaUsuarioDto.Tipo)
            };

            _repositoryFactory.Instancie<IPessoaEmpresaRepository>().Inserir(pessoaEmpresa);
        }

        private void InserirConfiguracaoUsuario(string cpf)
        {
            var configuracao = new ConfiguracaoUsuario
            {
                Inscricao = cpf,
                NotificarPortalEstadual = true
            };

            _repositoryFactory.Instancie<IConfiguracaoUsuarioRepository>().Inserir(configuracao);
        }

        private static eClassificacaoPessoa ObterClassificacao(eClassificacaoPessoa tipo)
        {
            if (tipo == eClassificacaoPessoa.Visitante || tipo == eClassificacaoPessoa.Padrao)
                return eClassificacaoPessoa.Colaborador;

            return tipo;
        }
        #endregion
    }
}
