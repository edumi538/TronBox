using System.ComponentModel.DataAnnotations;

namespace TronBox.Domain.Enums
{
    public enum EEstruturaDownload
    {
        [Display(Name = "Ano / Mês")]
        AnoMes = 1,

        [Display(Name = "Ano / Mês / Inscrição da Empresa")]
        InscricaoEmpresa = 2,

        [Display(Name = "Ano / Mês / Razão Social da Empresa")]
        RazaoSocial = 3,
    }
}
