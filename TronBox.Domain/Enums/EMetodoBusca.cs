using System.ComponentModel.DataAnnotations;

namespace TronBox.Domain.Enums
{
    public enum EMetodoBusca
    {
        [Display(Name = "Últimos 30 dias")]
        UltimosTrintaDias = 1,

        [Display(Name = "Últimos 3 Meses")]
        UltimosMeses = 2,
    }
}
