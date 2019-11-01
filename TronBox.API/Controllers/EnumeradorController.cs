using Comum.UI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NFe.Classes.Informacoes.Detalhe.Tributacao.Estadual.Tipos;
using NFe.Utils;
using System;
using System.Linq;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.Enums;
using TronCore.Domain.Factories;
using TronCore.Dominio.Notifications;
using TronCore.Enumeradores.Helpers;

namespace TronBox.API.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/v1/enumeradores")]
    public class EnumeradorController : BaseController
    {
        public EnumeradorController(IDomainNotificationHandler<DomainNotification> notifications, IAppServiceFactory appServiceFactory) : base(notifications, appServiceFactory)
        {
        }

        [HttpGet("tipos-acesso")]
        public IActionResult GetAcessoMatoGrosso()
        {
            var tiposAcesso = from ETipoAcessoMatoGrosso d in Enum.GetValues(typeof(ETipoAcessoMatoGrosso))
                              select new { value = (int)d, label = EnumHelper<ETipoAcessoMatoGrosso>.GetDisplayValue(d) };

            return Ok(tiposAcesso);
        }

        [HttpGet("tipos-documento-fiscal")]
        public IActionResult GetTipoDocumentoFiscal()
        {
            var tiposDocumentoFiscal = from ETipoDocumentoFiscal d in Enum.GetValues(typeof(ETipoDocumentoFiscal))
                              select new { value = (int)d, label = EnumHelper<ETipoDocumentoFiscal>.GetDisplayValue(d) };

            return Ok(tiposDocumentoFiscal);
        }

        [HttpGet("origens-manifesto")]
        public IActionResult GetOrigemManifesto()
        {
            var origensManifesto = from EOrigemManifesto d in Enum.GetValues(typeof(EOrigemManifesto))
                              select new { value = (int)d, label = EnumHelper<EOrigemManifesto>.GetDisplayValue(d) };

            return Ok(origensManifesto);
        }

        [HttpGet("origens-documento-fiscal")]
        public IActionResult GetOrigemDocumentoFiscal()
        {
            var origensDocumentoFiscal = from EOrigemDocumentoFiscal d in Enum.GetValues(typeof(EOrigemDocumentoFiscal))
                                         select new { value = (int)d, label = EnumHelper<EOrigemDocumentoFiscal>.GetDisplayValue(d) };

            return Ok(origensDocumentoFiscal);
        }

        [HttpGet("situacoes-documento")]
        public IActionResult GetSituacaoDocumento()
        {
            var situacoesDocumento = from ESituacaoDocumentoFiscal d in Enum.GetValues(typeof(ESituacaoDocumentoFiscal))
                                   select new { value = (int)d, label = EnumHelper<ESituacaoDocumentoFiscal>.GetDisplayValue(d) };

            return Ok(situacoesDocumento);
        }

        [HttpGet("situacoes-manifesto")]
        public IActionResult GetSituacaoManifesto()
        {
            var situacoesManifesto = from ESituacaoManifesto d in Enum.GetValues(typeof(ESituacaoManifesto))
                                     select new { value = (int)d, label = EnumHelper<ESituacaoManifesto>.GetDisplayValue(d) };

            return Ok(situacoesManifesto);
        }

        [HttpGet("cst-icms/{csticms}")]
        public IActionResult GetCSTICMS(Csticms csticms) => Ok(AppServiceFactory.Instancie<IEnumeradorAppService>().ObterCSTICMS(csticms));
    }
}