using System.ComponentModel.DataAnnotations;

namespace TronBox.Domain.Enums
{
    public enum ArquiteturaDownload
    {
        [Display(Name = "Ano / Mês")]
        AnoMes = 1,

        [Display(Name = "Ano / Mês / CNPJ Emissor")]
        AnoMesCnpjEmissor = 2,

        [Display(Name = "Ano / Mês / Razão Social do Emissor")]
        AnoMesRazaoEmissor = 3,

        [Display(Name = "Ano / Mês / CNPJ Destinatário")]
        AnoMesCnpjDestinatario = 4,

        [Display(Name = "Ano / Mês / Razão Social do Destinatário")]
        AnoMesRazaoDestinatario = 5,
    }
}
