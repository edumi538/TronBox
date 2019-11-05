using Comum.DTO;
using Comum.Enums;
using Comum.UI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.DTO;
using TronBox.Domain.Enums;
using TronCore.Domain.Factories;
using TronCore.Dominio.Notifications;
using TronCore.Seguranca.AOP.Anotacao;

namespace TronBox.UI.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/v1/empresas")]
    [IdentificadorFuncao(typeof(eFuncaoTronBox), eFuncaoTronBox.ID_EMPRESA)]
    public class EmpresaController : BaseController
    {
        readonly IDomainNotificationHandler<DomainNotification> _notifications;

        public EmpresaController(IDomainNotificationHandler<DomainNotification> notifications, IAppServiceFactory appServiceFactory) : base(notifications, appServiceFactory)
        {
            _notifications = notifications;
        }

        [HttpGet]
        [IdentificadorOperacao(eFuncaoTronBox.ID_EMPRESA, "Carregar Empresa", eOperacaoSuite.ID_OP_ACESSO, typeof(eOperacaoSuite), typeof(eFuncaoTronBox), "/empresas")]
        public IActionResult Get() => Ok(AppServiceFactory.Instancie<IConfiguracaoEmpresaAppService>().BuscarEmpresa());

        [HttpPut]
        [IdentificadorOperacao(eFuncaoTronBox.ID_EMPRESA, "Atualizar Empresa", eOperacaoSuite.ID_OP_EDITAR, typeof(eOperacaoSuite), typeof(eFuncaoTronBox), "/empresas/editar/:id")]
        public IActionResult Put([FromBody]EmpresaDTO empresa)
        {
            AppServiceFactory.Instancie<IConfiguracaoEmpresaAppService>().AtualizarEmpresa(empresa);

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

            return Ok(new
            {
                sucesso = true,
                mensagem = "Operação realizada com sucesso."
            });
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] CertificadoCreateDTO certificadoCreateDTO)
        {
            var resposta = await AppServiceFactory.Instancie<IConfiguracaoEmpresaAppService>().Upload(certificadoCreateDTO);

            if (!resposta.Sucesso)
                return BadRequest(resposta);

            return Ok(resposta);
        }
    }
}