using Comum.Domain.Enums;
using Comum.Domain.Services.Interfaces;
using Comum.Domain.ViewModels;
using Comum.DTO;
using Comum.UI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using TronCore.Domain.Factories;
using TronCore.Dominio.Notifications;

namespace TronBox.UI.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/inquilinos")]
    public class InquilinoController : BaseController
    {
        readonly IDomainNotificationHandler<DomainNotification> _notifications;

        public InquilinoController(IDomainNotificationHandler<DomainNotification> notifications, IAppServiceFactory appServiceFactory) : base(notifications, appServiceFactory)
        {
            _notifications = notifications;
        }

        [HttpPost]
        public IActionResult Post([FromBody]InquilinoMongoDTO inquilinoConnect)
        {
            var empresaExistente = AppServiceFactory.Instancie<IEmpresaAppService>().BuscarPorInscricao(inquilinoConnect.Inscricao);

            if (empresaExistente == null)
            {
                var empresa = new EmpresaViewModel
                {
                    RazaoSocial = inquilinoConnect.Nome,
                    Inscricao = inquilinoConnect.Inscricao,
                    TipoInscricao = eTipoInscricaoEmpresa.CNPJ
                };

                AppServiceFactory.Instancie<IEmpresaAppService>().Inserir(empresa);

                if (_notifications.HasNotifications())
                {
                    return BadRequest(new
                    {
                        sucesso = false,
                        erro = _notifications.GetNotifications()
                            .Select(c => new
                            {
                                Chave = c.Key,
                                Mensagem = c.Value
                            })
                    });
                }

                if (inquilinoConnect.PessoaAdministrador != null)
                    CriarPessoa(inquilinoConnect.PessoaAdministrador);

                if (inquilinoConnect.PessoaImplantacao != null)
                    CriarPessoa(inquilinoConnect.PessoaImplantacao);

                if (_notifications.HasNotifications())
                {
                    return BadRequest(new
                    {
                        sucesso = false,
                        erro = _notifications.GetNotifications()
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