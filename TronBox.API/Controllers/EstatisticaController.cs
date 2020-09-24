using Comum.UI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.DTO;
using TronCore.Domain.Factories;
using TronCore.Dominio.Notifications;

namespace TronBox.API.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/estatisticas")]
    public class EstatisticaController : BaseController
    {
        public EstatisticaController(IDomainNotificationHandler<DomainNotification> notifications, IAppServiceFactory appServiceFactory) : base(notifications, appServiceFactory)
        {
        }

        [HttpGet]
        public IActionResult Get(string filtro)
        {
            var estatisticas = AppServiceFactory.Instancie<IEstatisticaAppService>().BuscarTodos(filtro);

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            return Json(estatisticas, settings);
        }

        [HttpPost]
        public async Task<IActionResult> Calcular([FromBody] EstatisticaForCreateDTO estatisticaForCreate)
        {
            var estatistica = await AppServiceFactory.Instancie<IEstatisticaAppService>().Calcular(estatisticaForCreate.DataHora);

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            return Json(estatistica, settings);
        }
    }
}