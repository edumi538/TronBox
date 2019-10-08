using System.ComponentModel.DataAnnotations;

namespace TronBox.Domain.Enums
{
    public enum OrigemDocumentoFiscal
    {
        [Display(Name = "Agente")]
        Agente = 1,

        [Display(Name = "Monitor")]
        Monitor = 2,

        [Display(Name = "Upload Manual")]
        UploadManual = 3,

        [Display(Name = "E-mail")]
        Email = 4,
    }
}
