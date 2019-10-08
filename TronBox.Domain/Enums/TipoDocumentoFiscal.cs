using System.ComponentModel.DataAnnotations;

namespace TronBox.Domain.Enums
{
    public enum TipoDocumentoFiscal
    {
        [Display(Name = "NFe Entrada")]
        NfeEntrada = 1,

        [Display(Name = "NFe Saída")]
        NfeSaida = 2,

        [Display(Name = "CTe Entrada")]
        CteEntrada = 3,

        [Display(Name = "CTe Saída")]
        CteSaida = 4,

        [Display(Name = "NFse Entrada")]
        NfseEntrada = 5,

        [Display(Name = "NFse Saída")]
        NfseSaida = 6,
    }
}
