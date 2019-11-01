using System.ComponentModel.DataAnnotations;

namespace TronBox.Domain.Enums
{
    public enum EOrigemManifesto
    {
        [Display(Name = "Agente")]
        Agente = 1,

        [Display(Name = "Monitor")]
        Monitor = 2,
    }
}
