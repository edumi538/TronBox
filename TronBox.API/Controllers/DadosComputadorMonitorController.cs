using Comum.UI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.DTO;
using TronCore.Domain.Factories;
using TronCore.Dominio.Notifications;

namespace TronBox.API.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/v1/dados-computador-monitor")]
    public class DadosComputadorMonitorController : BaseController
    {
        public DadosComputadorMonitorController(IDomainNotificationHandler<DomainNotification> notifications, IAppServiceFactory appServiceFactory) : base(notifications, appServiceFactory)
        {
        }

        [HttpGet]
        public IActionResult Get(string filtro) => Ok(AppServiceFactory.Instancie<IDadosComputadorMonitorAppService>().BuscarTodos(filtro));

        [HttpPost]
        public IActionResult Post([FromBody]DadosComputadorMonitorDTO dadosComputadorMonitorDTO)
        {
            AppServiceFactory.Instancie<IDadosComputadorMonitorAppService>().Inserir(dadosComputadorMonitorDTO);

            if (Notifications.HasNotifications())
            {
                return BadRequest(new
                {
                    sucesso = false,
                    erros = Notifications.GetNotifications()
                        .Select(c => new
                        {
                            Chave = c.Key,
                            Mensagem = c.Value
                        })
                });
            }

            return Created(dadosComputadorMonitorDTO.Id,
                new
                {
                    sucesso = true,
                    mensagem = "Operação realizada com sucesso."
                }
            );
        }
    }
}