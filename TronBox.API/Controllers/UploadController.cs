using Comum.Domain.Interfaces;
using Comum.Enums;
using Comum.UI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
        readonly IPessoaUsuarioLogado _usuarioLogado;

        public UploadController(IDomainNotificationHandler<DomainNotification> notifications, IAppServiceFactory appServiceFactory, IPessoaUsuarioLogado usuarioLogado) : base(notifications, appServiceFactory)
        {
            _notifications = notifications;
            _usuarioLogado = usuarioLogado;
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
                        ChaveDocumentoFiscal = c.Key,
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
        [IdentificadorOperacao(eFuncaoTronBox.ID_UPLOAD, "Upload Documentos Fiscais", eOperacaoSuite.ID_OP_UPLOAD, typeof(eOperacaoSuite), typeof(eFuncaoTronBox), "/enviar-documentos")]
        public async Task<IActionResult> Upload([FromForm]IFormFile arquivo)
        {
            var arquivoEnviado = new EnviarArquivosDTO()
            {
                Origem = OrigemDocumentoFiscal.UploadManual,
                Originador = _usuarioLogado.ObtenhaPessoa().Pessoa.Nome,
                Arquivos = new List<IFormFile>() { arquivo }
            };

            await AppServiceFactory.Instancie<IDocumentoFiscalAppService>().Inserir(arquivoEnviado);

            if (_notifications.HasNotifications())
            {
                return BadRequest(new
                {
                    sucesso = false,
                    erros = _notifications.GetNotifications()
                        .Select(c => new
                        {
                            Chave = c.Key,
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