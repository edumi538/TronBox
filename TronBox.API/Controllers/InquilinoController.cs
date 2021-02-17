using Comum.Domain.Enums;
using Comum.Domain.Services.Interfaces;
using Comum.Domain.ViewModels;
using Comum.DTO;
using Comum.UI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.DTO;
using TronCore.Domain.Factories;
using TronCore.Dominio.Notifications;

namespace TronBox.UI.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/inquilinos")]
    public class InquilinoController : BaseController
    {
        public InquilinoController(IDomainNotificationHandler<DomainNotification> notifications, IAppServiceFactory appServiceFactory) : base(notifications, appServiceFactory)
        {
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]InquilinoMongoDTO inquilino)
        {
            EmpresaViewModel empresaExistente = null;
            try
            {
                empresaExistente = AppServiceFactory.Instancie<IEmpresaAppService>()
                    .BuscarPorInscricao(inquilino.Inscricao);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            

            if (empresaExistente == null)
            {
                var empresa = new EmpresaViewModel
                {
                    RazaoSocial = inquilino.Nome,
                    Inscricao = inquilino.Inscricao,
                    TipoInscricao = inquilino.Inscricao.Length == 11 ? eTipoInscricaoEmpresa.CPF : eTipoInscricaoEmpresa.CNPJ
                };

                AppServiceFactory.Instancie<IEmpresaAppService>().Inserir(empresa);
                await AppServiceFactory.Instancie<IDocumentoFiscalAppService>().CriarIndexChaveDocumentoFiscalAsync();

                if (Notifications.HasNotifications())
                {
                    return BadRequest(new
                    {
                        sucesso = false,
                        erro = Notifications.GetNotifications()
                            .Select(c => new
                            {
                                Chave = c.Key,
                                Mensagem = c.Value
                            })
                    });
                }

                if (inquilino.PessoaAdministrador != null)
                    CriarPessoa(inquilino.PessoaAdministrador);

                if (inquilino.PessoaImplantacao != null)
                    CriarPessoa(inquilino.PessoaImplantacao);

                if (Notifications.HasNotifications())
                {
                    return BadRequest(new
                    {
                        sucesso = false,
                        erro = Notifications.GetNotifications()
                            .Select(c => new
                            {
                                Chave = c.Key,
                                Mensagem = c.Value
                            })
                    });
                }
            }

            return Ok(new
            {
                sucesso = true,
                mensagem = "Operação realizada com sucesso."
            });
        }

        #region Private Methods
        private void CriarPessoa(PessoaViewModel pessoa)
        {
            var pessoaId = CriarRelacionamentoPessoaUsuario(pessoa);

            CriarRelacionamentoPessoaEmpresa(pessoaId);
            CriarConfiguracaoUsuario(pessoa.Cpf);
        }

        private void CriarConfiguracaoUsuario(string cpf)
        {
            var configuracao = new ConfiguracaoUsuarioDTO
            {
                Inscricao = cpf,
                NotificarPortalEstadual = true
            };

            AppServiceFactory.Instancie<IConfiguracaoUsuarioAppService>().Inserir(configuracao);
        }

        private Guid CriarRelacionamentoPessoaUsuario(PessoaViewModel pessoa)
        {
            Guid.TryParse(pessoa.UsuarioId.ToString(), out Guid usuarioId);

            PessoaUsuarioViewModel pessoaUsuario = AppServiceFactory.Instancie<IPessoaUsuarioAppService>().BuscarPorUsuarioId(usuarioId);

            if (pessoaUsuario == null)
            {
                var pessoaId = BuscarIdPessoa(pessoa);

                pessoaUsuario = new PessoaUsuarioViewModel
                {
                    UsuarioId = usuarioId,
                    PessoaId = pessoaId
                };

                AppServiceFactory.Instancie<IPessoaUsuarioAppService>().Inserir(pessoaUsuario);

                return pessoaId;
            }

            return pessoaUsuario.PessoaId;
        }

        private Guid BuscarIdPessoa(PessoaViewModel pessoaViewModel)
        {
            var pessoa = new PessoaViewModel
            {
                Id = pessoaViewModel.Id,
                Nome = pessoaViewModel.Nome,
                Cpf = pessoaViewModel.Cpf,
                Email = pessoaViewModel.Email,
                Status = true
            };

            if (pessoa.Id == default(Guid))
                pessoa.Id = Guid.NewGuid();

            AppServiceFactory.Instancie<IPessoaAppService>().Inserir(pessoa);

            return pessoa.Id;
        }

        private void CriarRelacionamentoPessoaEmpresa(Guid pessoaId)
        {
            EmpresaViewModel empresa = AppServiceFactory.Instancie<IEmpresaAppService>().BuscarTodos().FirstOrDefault();

            if (empresa != null)
            {
                PessoaEmpresaViewModel pessoaEmpresa = AppServiceFactory.Instancie<IPessoaEmpresaAppService>()
                    .BuscarPorId(pessoaId, empresa.Id);

                if (pessoaEmpresa == null)
                {
                    pessoaEmpresa = new PessoaEmpresaViewModel
                    {
                        PessoaId = pessoaId,
                        EmpresaId = empresa.Id,
                        ClassificacaoFuncionario = eClassificacaoPessoa.Administrador
                    };

                    AppServiceFactory.Instancie<IPessoaEmpresaAppService>().Inserir(pessoaEmpresa);
                }
            }
        }
        #endregion
    }
}