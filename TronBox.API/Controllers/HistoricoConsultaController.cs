using Comum.Enums;
using Comum.UI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
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
    [Route("api/v1/historicos-consulta")]
    [IdentificadorFuncao(typeof(eFuncaoTronBox), eFuncaoTronBox.ID_HISTORICO_CONSULTA)]
    public class HistoricoConsultaController : BaseController
    {
        readonly IDomainNotificationHandler<DomainNotification> _notifications;

        public HistoricoConsultaController(IDomainNotificationHandler<DomainNotification> notifications, IAppServiceFactory appServiceFactory) : base(notifications, appServiceFactory)
        {
            _notifications = notifications;
        }

        [HttpGet]
        [IdentificadorOperacao(eFuncaoTronBox.ID_HISTORICO_CONSULTA, "Carregar Histórico de Consultas", eOperacaoSuite.ID_OP_ACESSO, typeof(eOperacaoSuite), typeof(eFuncaoTronBox), "/historicos-consulta")]
        public IActionResult Get(string filtro) => Ok(AppServiceFactory.Instancie<IHistoricoConsultaAppService>().BuscarTodos(filtro));

        [HttpGet("ultima")]
        public IActionResult UltimaConsulta()
        {
            var ultimaConsulta = AppServiceFactory.Instancie<IHistoricoConsultaAppService>().ObterUltimaConsulta();

            if (ultimaConsulta != null)
                return Ok(ultimaConsulta.DataHoraConsultaFormatada);

            return NotFound();
        }

        [HttpGet("ultimo-nsu/{tipo}")]
        public IActionResult UltimoNSU(ETipoDocumentoConsulta tipo) => Ok(AppServiceFactory.Instancie<IHistoricoConsultaAppService>().ObterUltimoNSU(tipo));

        [HttpPost]
        public IActionResult Post([FromBody]HistoricoConsultaDTO historicoConsulta)
        {
            if (historicoConsulta.Id == null)
                historicoConsulta.Id = Guid.NewGuid().ToString();

            AppServiceFactory.Instancie<IHistoricoConsultaAppService>().Inserir(historicoConsulta);

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

        [HttpPost("realizar-busca")]
        public IActionResult BuscarManualmente()
        {
            AppServiceFactory.Instancie<IHistoricoConsultaAppService>().BuscarManualmente();

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

            return Ok(new
            {
                sucesso = true,
                mensagem = "Operação realizada com sucesso."
            });
        }
    }
}