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
    [Route("api/v1/documentos-fiscais")]
    [IdentificadorFuncao(typeof(eFuncaoTronBox), eFuncaoTronBox.ID_DOCUMENTO_FISCAL)]
    public class DocumentoFiscalController : BaseController
    {
        readonly IDomainNotificationHandler<DomainNotification> _notifications;

        public DocumentoFiscalController(IDomainNotificationHandler<DomainNotification> notifications, IAppServiceFactory appServiceFactory) : base(notifications, appServiceFactory)
        {
            _notifications = notifications;
        }

        [HttpGet]
        [IdentificadorOperacao(eFuncaoTronBox.ID_DOCUMENTO_FISCAL, "Carregar Documentos Fiscais", eOperacaoSuite.ID_OP_ACESSO, typeof(eOperacaoSuite), typeof(eFuncaoTronBox), "/documentos-fiscais")]
        public IActionResult Get(string filtro) => Ok(AppServiceFactory.Instancie<IDocumentoFiscalAppService>().BuscarTodos(filtro));

        [HttpGet("{id:GUID}")]
        public IActionResult Get(Guid id) => Ok(AppServiceFactory.Instancie<IDocumentoFiscalAppService>().BuscarPorId(id));

        [HttpPut]
        [IdentificadorOperacao(eFuncaoTronBox.ID_DOCUMENTO_FISCAL, "Atualizar Documento Fiscal", eOperacaoSuite.ID_OP_EDITAR, typeof(eOperacaoSuite), typeof(eFuncaoTronBox), "/documentos-fiscais/editar/:id")]
        public IActionResult Put([FromBody]DocumentoFiscalDTO documentoFiscalDTO)
        {
            AppServiceFactory.Instancie<IDocumentoFiscalAppService>().Atualizar(documentoFiscalDTO);

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
        [IdentificadorOperacao(eFuncaoTronBox.ID_DOCUMENTO_FISCAL, "Inserir Documento Fiscal", eOperacaoSuite.ID_OP_INSERIR, typeof(eOperacaoSuite), typeof(eFuncaoTronBox), "/documentos-fiscais/adicionar")]
        public IActionResult Post([FromBody]DocumentoFiscalDTO documentoFiscalDTO)
        {
            if (documentoFiscalDTO.Id == null)
                documentoFiscalDTO.Id = Guid.NewGuid().ToString();

            AppServiceFactory.Instancie<IDocumentoFiscalAppService>().Inserir(documentoFiscalDTO);

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

            return Created(documentoFiscalDTO.Id,
                new
                {
                    sucesso = true,
                    mensagem = "Operação realizada com sucesso."
                }
            );
        }

        [HttpDelete("{id:GUID}")]
        [IdentificadorOperacao(eFuncaoTronBox.ID_DOCUMENTO_FISCAL, "Excluir Documento Fiscal", eOperacaoSuite.ID_OP_EXCLUIR, typeof(eOperacaoSuite), typeof(eFuncaoTronBox), "/documentos-fiscais/excluir")]
        public IActionResult Delete(Guid id)
        {
            AppServiceFactory.Instancie<IDocumentoFiscalAppService>().Deletar(id);

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