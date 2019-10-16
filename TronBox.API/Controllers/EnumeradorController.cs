using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using TronBox.Domain.Enums;
using TronCore.Enumeradores.Helpers;

namespace TronBox.API.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/v1/enumeradores")]
    public class EnumeradorController : Controller
    {
        [HttpGet("arquitetura-downloads")]
        public IActionResult GetArquiteturaDownload()
        {
            var arquiteturas = from ArquiteturaDownload d in Enum.GetValues(typeof(ArquiteturaDownload))
                               select new { value = (int)d, label = EnumHelper<ArquiteturaDownload>.GetDisplayValue(d) };

            return Ok(arquiteturas);
        }

        [HttpGet("tipos-acesso")]
        public IActionResult GetAcessoMatoGrosso()
        {
            var tiposAcesso = from TipoAcessoMatoGrosso d in Enum.GetValues(typeof(TipoAcessoMatoGrosso))
                              select new { value = (int)d, label = EnumHelper<TipoAcessoMatoGrosso>.GetDisplayValue(d) };

            return Ok(tiposAcesso);
        }

        [HttpGet("tipos-documento-fiscal")]
        public IActionResult GetTipoDocumentoFiscal()
        {
            var tiposDocumentoFiscal = from TipoDocumentoFiscal d in Enum.GetValues(typeof(TipoDocumentoFiscal))
                              select new { value = (int)d, label = EnumHelper<TipoDocumentoFiscal>.GetDisplayValue(d) };

            return Ok(tiposDocumentoFiscal);
        }

        [HttpGet("origens-manifesto")]
        public IActionResult GetOrigemManifesto()
        {
            var origensManifesto = from OrigemManifesto d in Enum.GetValues(typeof(OrigemManifesto))
                              select new { value = (int)d, label = EnumHelper<OrigemManifesto>.GetDisplayValue(d) };

            return Ok(origensManifesto);
        }

        [HttpGet("origens-documento-fiscal")]
        public IActionResult GetOrigemDocumentoFiscal()
        {
            var origensDocumentoFiscal = from OrigemDocumentoFiscal d in Enum.GetValues(typeof(OrigemDocumentoFiscal))
                                         select new { value = (int)d, label = EnumHelper<OrigemDocumentoFiscal>.GetDisplayValue(d) };

            return Ok(origensDocumentoFiscal);
        }

        [HttpGet("situacoes-documento")]
        public IActionResult GetSituacaoDocumento()
        {
            var situacoesDocumento = from SituacaoDocumentoFiscal d in Enum.GetValues(typeof(SituacaoDocumentoFiscal))
                                   select new { value = (int)d, label = EnumHelper<SituacaoDocumentoFiscal>.GetDisplayValue(d) };

            return Ok(situacoesDocumento);
        }

        [HttpGet("situacoes-manifesto")]
        public IActionResult GetSituacaoManifesto()
        {
            var situacoesManifesto = from SituacaoManifesto d in Enum.GetValues(typeof(SituacaoManifesto))
                                     select new { value = (int)d, label = EnumHelper<SituacaoManifesto>.GetDisplayValue(d) };

            return Ok(situacoesManifesto);
        }
    }
}