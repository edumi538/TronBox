using AutoMapper;
using Comum.Domain.Aggregates.EmpresaAgg;
using Comum.Domain.Aggregates.EmpresaAgg.Repository;
using Comum.Domain.Interfaces;
using Comum.DTO;
using FluentValidation.Results;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.Aggregates.ConfiguracaoEmpresaAgg;
using TronBox.Domain.Aggregates.ConfiguracaoEmpresaAgg.Repository;
using TronBox.Domain.DTO;
using TronCore.DefinicoesConfiguracoes;
using TronCore.Dominio.Base;
using TronCore.Dominio.Bus;
using TronCore.Dominio.Notifications;
using TronCore.Persistencia.Interfaces;
using TronCore.Utilitarios;

namespace TronBox.Application.Services
{
    public class ConfiguracaoEmpresaAppService : IConfiguracaoEmpresaAppService
    {
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
            var empresa = _mapper.Map<Empresa>(empresaDto);
            var configuracaoEmpresa = _mapper.Map<ConfiguracaoEmpresa>(empresaDto.ConfiguracaoEmpresa);
            configuracaoEmpresa.Inscricao = empresa.Inscricao;

            if (EhValido(configuracaoEmpresa))
            {
                var configuracaoExistente = BuscarConfiguracaoEmpresa();

                if (configuracaoExistente != null)
                {
                    configuracaoEmpresa.Id = configuracaoExistente.Id;
                    _repositoryFactory.Instancie<IConfiguracaoEmpresaRepository>().Atualizar(configuracaoEmpresa);
                }
                else _repositoryFactory.Instancie<IConfiguracaoEmpresaRepository>().Inserir(configuracaoEmpresa);

                _repositoryFactory.Instancie<IEmpresaRepository>().Atualizar(empresa);
            }
        }

        public async Task<Resposta> Upload(CertificadoCreateDTO certificadoCreateDTO)
        {
            var dictionary = new Dictionary<string, dynamic>
            {
                { "inscricaoEmpresa", certificadoCreateDTO.InscricaoEmpresa },
                { "senha", certificadoCreateDTO.Senha },
                { "arquivo", certificadoCreateDTO.Arquivo }
            };

            var result = await UtilitarioHttpClient.PostRequest(_usuarioLogado.GetToken(), Constantes.URI_BASE_CT, "api/v1/certificados", dictionary, "certificado.pfx");

            return JsonConvert.DeserializeObject<Resposta>(result);
        }

        public async Task<Resposta> DeletarCertificado(Guid id)
        {
            var result = await UtilitarioHttpClient.DeleteRequest(_usuarioLogado.GetToken(), Constantes.URI_BASE_CT, $"api/v1/certificados/{id}");

            return JsonConvert.DeserializeObject<Resposta>(result);
        }

        #region Private Methods
        private ConfiguracaoEmpresa BuscarConfiguracaoEmpresa() => _repositoryFactory.Instancie<IConfiguracaoEmpresaRepository>().BuscarTodos().FirstOrDefault();

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
        #endregion
    }
}
