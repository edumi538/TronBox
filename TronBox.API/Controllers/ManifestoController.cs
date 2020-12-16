using Comum.Enums;
using Comum.UI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
    [Route("api/v1/manifestos")]
    [IdentificadorFuncao(typeof(eFuncaoTronBox), eFuncaoTronBox.ID_MANIFESTO)]
    public class ManifestoController : BaseController
    {
        readonly IDomainNotificationHandler<DomainNotification> _notifications;

        public ManifestoController(IDomainNotificationHandler<DomainNotification> notifications, IAppServiceFactory appServiceFactory) : base(notifications, appServiceFactory)
        {
            _notifications = notifications;
        }

        [HttpGet]
        [IdentificadorOperacao(eFuncaoTronBox.ID_MANIFESTO, "Carregar Manifestos", eOperacaoSuite.ID_OP_ACESSO, typeof(eOperacaoSuite), typeof(eFuncaoTronBox), "/manifestos")]
        public IActionResult Get(string filtro) => Ok(AppServiceFactory.Instancie<IManifestoAppService>().BuscarTodos(filtro));

        [HttpPost("obter")]
        public IActionResult ObterVarios([FromBody] IEnumerable<string> chaves) => Ok(AppServiceFactory.Instancie<IManifestoAppService>().BuscarPorChaves(chaves));

        [HttpGet("{id:GUID}")]
        public IActionResult Get(Guid id) => Ok(AppServiceFactory.Instancie<IManifestoAppService>().BuscarPorId(id));

        [HttpPost]
        public IActionResult Post([FromBody] IEnumerable<dynamic> manifestos)
        {
            var inseridos = AppServiceFactory.Instancie<IManifestoAppService>().InserirOuAtualizar(manifestos);

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

            return Ok(new { inseridos });
        }

        [HttpDelete("{id:GUID}")]
        public IActionResult Delete(Guid id)
        {
            AppServiceFactory.Instancie<IManifestoAppService>().Deletar(id);

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

        [HttpDelete("duplicados")]
        public IActionResult DeleteDuplicados()
        {
            AppServiceFactory.Instancie<IManifestoAppService>().DeletarDuplicados();

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

        [HttpPost("manifestar")]
        public async Task<IActionResult> Manifestar([FromBody] ManifestarDTO manifestarDTO) 
            => Ok(await AppServiceFactory.Instancie<IManifestoAppService>().Manifestar(manifestarDTO));
    }
}