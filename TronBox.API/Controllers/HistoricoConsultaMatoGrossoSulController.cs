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
    [Route("api/v1/historicos-consulta-mato-grosso-sul")]
    public class HistoricoConsultaMatoGrossoSulController : BaseController
    {
        public HistoricoConsultaMatoGrossoSulController(IDomainNotificationHandler<DomainNotification> notifications, IAppServiceFactory appServiceFactory) : base(notifications, appServiceFactory)
        {
        }

        [HttpGet]
        public IActionResult Get(string filtro) => Ok(AppServiceFactory.Instancie<IHistoricoConsultaMatoGrossoSulAppService>().BuscarTodos(filtro));

        [HttpGet("ultima")]
        public IActionResult UltimaConsulta()
        {
            var ultimaConsulta = AppServiceFactory.Instancie<IHistoricoConsultaMatoGrossoSulAppService>().ObterUltimaConsulta();

            if (ultimaConsulta != null)
                return Ok(ultimaConsulta.DataHoraConsultaFormatada);

            return NotFound();
        }

        [HttpPost]
        public IActionResult Post([FromBody]HistoricoConsultaMatoGrossoSulDTO historicoConsulta)
        {
            AppServiceFactory.Instancie<IHistoricoConsultaMatoGrossoSulAppService>().Inserir(historicoConsulta);

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

            return Created(historicoConsulta.Id,
                new
                {
                    sucesso = true,
                    mensagem = "Operação realizada com sucesso."
                }
            );
        }
    }
}