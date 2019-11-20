using Comum.Enums;
using Comum.UI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
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

        [HttpGet("{id:GUID}")]
        public IActionResult Get(Guid id) => Ok(AppServiceFactory.Instancie<IManifestoAppService>().BuscarPorId(id));

        [HttpPatch("{id}")]
        [IdentificadorOperacao(eFuncaoTronBox.ID_MANIFESTO, "Atualizar Manifesto", eOperacaoSuite.ID_OP_EDITAR, typeof(eOperacaoSuite), typeof(eFuncaoTronBox), "/manifestos/editar/:id")]
        public IActionResult Patch(Guid id, [FromBody]dynamic manifestoDTO)
        {
            AppServiceFactory.Instancie<IManifestoAppService>().Atualizar(id, manifestoDTO);

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

        [HttpPost]
        [IdentificadorOperacao(eFuncaoTronBox.ID_MANIFESTO, "Inserir Manifesto", eOperacaoSuite.ID_OP_INSERIR, typeof(eOperacaoSuite), typeof(eFuncaoTronBox), "/manifestos/adicionar")]
        public IActionResult Post([FromBody]ManifestoDTO manifestoDTO)
        {
            if (manifestoDTO.Id == null)
                manifestoDTO.Id = Guid.NewGuid().ToString();

            AppServiceFactory.Instancie<IManifestoAppService>().Inserir(manifestoDTO);

            if (_notifications.HasNotifications())
            {
                var notification = _notifications.GetNotifications().FirstOrDefault();

                if (notification.Key == "ManifestoExistente")
                {
                    return Conflict(new
                    {
                        sucesso = false,
                        mensagem = notification.Value
                    });
                }

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

            return Created(manifestoDTO.Id,
                new
                {
                    sucesso = true,
                    mensagem = "Operação realizada com sucesso."
                }
            );
        }

        [HttpDelete("{id:GUID}")]
        [IdentificadorOperacao(eFuncaoTronBox.ID_MANIFESTO, "Excluir Manifesto", eOperacaoSuite.ID_OP_EXCLUIR, typeof(eOperacaoSuite), typeof(eFuncaoTronBox), "/manifestoss/excluir")]
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
    }
}