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
    }
}