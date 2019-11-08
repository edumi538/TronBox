using Comum.Enums;
using Comum.UI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [Route("api/v1/enviar-documentos")]
    [IdentificadorFuncao(typeof(eFuncaoTronBox), eFuncaoTronBox.ID_UPLOAD)]
    public class UploadController : BaseController
    {
        readonly IDomainNotificationHandler<DomainNotification> _notifications;

        public UploadController(IDomainNotificationHandler<DomainNotification> notifications, IAppServiceFactory appServiceFactory) : base(notifications, appServiceFactory)
        {
            _notifications = notifications;
        }

        [HttpPost("multiple")]
        [Consumes("application/json", "application/json-patch+json", "multipart/form-data")]
        [IdentificadorOperacao(eFuncaoTronBox.ID_UPLOAD, "Upload Documentos Fiscais", eOperacaoSuite.ID_OP_UPLOAD, typeof(eOperacaoSuite), typeof(eFuncaoTronBox), "/enviar-documentos")]
        public async Task<IActionResult> Upload([FromForm]EnviarArquivosDTO arquivos)
        {
            var documentosInseridos = await AppServiceFactory.Instancie<IDocumentoFiscalAppService>().Inserir(arquivos);

            var documentosNaoInseridos = _notifications.GetNotifications()
                .Select(c => new
                    {
                        NomeArquivo = c.Key,
                        Mensagem = c.Value,
                        Erros = c.Object
                    });

            return Ok(new
            {
                documentosInseridos,
                documentosNaoInseridos
            });
        }

        [HttpPost("single")]
        [Consumes("application/json", "application/json-patch+json", "multipart/form-data")]
        public async Task<IActionResult> UploadSingle([FromForm]EnviarArquivosDTO arquivos)
        {
            await AppServiceFactory.Instancie<IDocumentoFiscalAppService>().Inserir(arquivos);

            if (_notifications.HasNotifications())
            {
                return BadRequest(new
                {
                    sucesso = false,
                    erros = _notifications.GetNotifications()
                        .Select(c => new
                        {
                            NomeArquivo = c.Key,
                            Mensagem = c.Value,
                            Erros = c.Object
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