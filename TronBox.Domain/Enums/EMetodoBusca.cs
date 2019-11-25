using System.ComponentModel.DataAnnotations;

namespace TronBox.Domain.Enums
{
    public enum EMetodoBusca
    {
        [Display(Name = "Mês Atual")]
        MesAtual = 1,

        [Display(Name = "Últimos 3 Meses")]
        UltimosMeses = 2,
    }
}
