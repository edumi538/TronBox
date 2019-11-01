using System.ComponentModel.DataAnnotations;

namespace TronBox.Domain.Enums
{
    public enum EOrigemDocumentoFiscal
    {
        [Display(Name = "E-mail")]
        Email = 1,

        [Display(Name = "Upload Manual")]
        UploadManual = 2,

        [Display(Name = "Download Agente")]
        DownloadAgente = 3,

        [Display(Name = "Agente Manifestação")]
        AgenteManifestacao = 4,

        [Display(Name = "Monitor")]
        Monitor = 5,

        [Display(Name = "Portal Estadual")]
        PortalEstadual = 6,
    }
}
