using Comum.Enums;
using Comum.UI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TronBox.Domain.Enums;
using TronCore.Domain.Factories;
using TronCore.Dominio.Notifications;
using TronCore.Seguranca.AOP.Anotacao;

namespace TronBox.UI.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/dashboards")]
    [IdentificadorFuncao(typeof(eFuncaoTronBox), eFuncaoTronBox.ID_DASHBOARD)]
    public class DashboardController : BaseController
    {
        public DashboardController(IDomainNotificationHandler<DomainNotification> notifications, IAppServiceFactory appServiceFactory) : base(notifications, appServiceFactory)
        {
        }

        [HttpGet]
        [IdentificadorOperacao(eFuncaoTronBox.ID_DASHBOARD, "Dashboard", eOperacaoSuite.ID_OP_ACESSO, typeof(eOperacaoSuite), typeof(eFuncaoTronBox), "/")]
        public IActionResult Get() => Ok();
    }
}