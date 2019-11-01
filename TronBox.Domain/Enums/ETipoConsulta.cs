using System.ComponentModel.DataAnnotations;

namespace TronBox.Domain.Enums
{
    public enum ETipoConsulta
    {
        [Display(Name = "Automática")]
        Automatica = 1,

        [Display(Name = "Manual")]
        Manual = 2,
    }
}
