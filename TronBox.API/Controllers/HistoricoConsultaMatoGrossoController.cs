using Comum.UI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.DTO;
using TronCore.Domain.Factories;
using TronCore.Dominio.Notifications;

namespace TronBox.API.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/v1/historicos-consulta-mato-grosso")]
    public class HistoricoConsultaMatoGrossoController : BaseController
    {
        readonly IDomainNotificationHandler<DomainNotification> _notifications;

        public HistoricoConsultaMatoGrossoController(IDomainNotificationHandler<DomainNotification> notifications, IAppServiceFactory appServiceFactory) : base(notifications, appServiceFactory)
        {
            _notifications = notifications;
        }

        [HttpGet]
        public IActionResult Get(string filtro) => Ok(AppServiceFactory.Instancie<IHistoricoConsultaMatoGrossoAppService>().BuscarTodos(filtro));

        [HttpGet("ultima")]
        public IActionResult UltimaConsulta()
        {
            var ultimaConsulta = AppServiceFactory.Instancie<IHistoricoConsultaMatoGrossoAppService>().ObterUltimaConsulta();

            if (ultimaConsulta != null)
                return Ok(ultimaConsulta.DataHoraConsultaFormatada);

            return NotFound();
        }

        [HttpGet("ultimo-periodo")]
        public IActionResult UltimoNSU() => Ok(AppServiceFactory.Instancie<IHistoricoConsultaMatoGrossoAppService>().ObterUltimoPeriodo());

        [HttpPost]
        public IActionResult Post([FromBody]HistoricoConsultaMatoGrossoDTO historicoConsulta)
        {
            if (historicoConsulta.Id == null)
                historicoConsulta.Id = Guid.NewGuid().ToString();

            AppServiceFactory.Instancie<IHistoricoConsultaMatoGrossoAppService>().Inserir(historicoConsulta);

            if (_notifications.HasNotifications())
            {
                return BadRequest(new
                {
                    sucesso = false,
                    erros = _notifications.GetNotifications()
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