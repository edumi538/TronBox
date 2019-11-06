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
        [IdentificadorOperacao(eFuncaoTronBox.ID_DOCUMENTO_FISCAL, "Ver Detalhes Documento Fiscal", eOperacaoSuite.ID_OP_EDITAR, typeof(eOperacaoSuite), typeof(eFuncaoTronBox), "/documentos-fiscais/:id")]
        public async Task<IActionResult> Get(Guid id) => Ok(await AppServiceFactory.Instancie<IDocumentoFiscalAppService>().BuscarPorId(id));

        [HttpPut]
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
        public IActionResult Post([FromBody]DocumentoFiscalDTO documentoFiscalDTO)
        {
            if (documentoFiscalDTO.Id == null)
                documentoFiscalDTO.Id = Guid.NewGuid().ToString();

            //AppServiceFactory.Instancie<IDocumentoFiscalAppService>().Inserir(documentoFiscalDTO);

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

        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download(Guid id)
        {
            try
            {
                var fileBytes = await AppServiceFactory.Instancie<IDocumentoFiscalAppService>().DownloadPDF(id);

                return File(fileBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, erro = ex.Message });
            }
        }
    }
}