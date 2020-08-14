using Comum.Enums;
using Comum.UI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.DTO;
using TronBox.Domain.Enums;
using TronCore.Domain.Factories;
using TronCore.Dominio.Notifications;
using TronCore.Seguranca.AOP.Anotacao;

namespace TronBox.API.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/v1/historicos-consulta-mato-grosso")]
    [IdentificadorFuncao(typeof(eFuncaoTronBox), eFuncaoTronBox.ID_HISTORICO_CONSULTA_PORTAL_ESTADUAL)]
    public class HistoricoConsultaMatoGrossoController : BaseController
    {
        readonly IDomainNotificationHandler<DomainNotification> _notifications;

        public HistoricoConsultaMatoGrossoController(IDomainNotificationHandler<DomainNotification> notifications, IAppServiceFactory appServiceFactory) : base(notifications, appServiceFactory)
        {
            _notifications = notifications;
        }

        [HttpGet]
        [IdentificadorOperacao(eFuncaoTronBox.ID_HISTORICO_CONSULTA_PORTAL_ESTADUAL, "Carregar Histórico de Consultas", eOperacaoSuite.ID_OP_ACESSO, typeof(eOperacaoSuite), typeof(eFuncaoTronBox), "/historicos-consulta-portal-estadual")]
        public IActionResult Get(string filtro) => Ok(AppServiceFactory.Instancie<IHistoricoConsultaMatoGrossoAppService>().BuscarTodos(filtro));

        [HttpGet("ultima")]
        public IActionResult UltimaConsulta()
        {
            var ultimaConsulta = AppServiceFactory.Instancie<IHistoricoConsultaMatoGrossoAppService>().ObterUltimaConsulta();

            if (ultimaConsulta != null)
                return Ok(ultimaConsulta.DataHoraConsultaFormatada);

            return NotFound();
        }

        [HttpGet("ultimo-periodo/{inscricaoEstadual}")]
        public IActionResult UltimoNSU(string inscricaoEstadual) => Ok(AppServiceFactory.Instancie<IHistoricoConsultaMatoGrossoAppService>().ObterUltimoPeriodo(inscricaoEstadual));

        [HttpPost]
        public IActionResult Post([FromBody]HistoricoConsultaMatoGrossoDTO historicoConsulta)
        {
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