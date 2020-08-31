using Comum.UI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Linq;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.DTO;
using TronCore.Domain.Factories;
using TronCore.Dominio.Notifications;

namespace TronBox.API.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/configuracoes-usuario")]
    public class ConfiguracaoUsuarioController : BaseController
    {
        public ConfiguracaoUsuarioController(IDomainNotificationHandler<DomainNotification> notifications, IAppServiceFactory appServiceFactory) : base(notifications, appServiceFactory)
        {
        }

        [HttpGet]
        public IActionResult Get()
        {
            var configuracaoUsuario = AppServiceFactory.Instancie<IConfiguracaoUsuarioAppService>().BuscarConfiguracaoUsuario();

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            return Json(configuracaoUsuario, settings);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ConfiguracaoUsuarioDTO configuracaoUsuario)
        {
            AppServiceFactory.Instancie<IConfiguracaoUsuarioAppService>().InserirOuAtualizar(configuracaoUsuario);

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

            return NoContent();
        }
    }
}