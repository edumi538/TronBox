using System.ComponentModel.DataAnnotations;

namespace TronBox.Domain.Enums
{
    public enum ETipoAcessoMatoGrosso
    {
        [Display(Name = "Contabilista")]
        Contabilista = 1,

        [Display(Name = "Escritorio")]
        Escritorio = 2,

        [Display(Name = "Individual")]
        Individual = 3,
    }
}
