using System.ComponentModel.DataAnnotations;

namespace TronBox.Domain.Enums
{
    public enum OrigemManifesto
    {
        [Display(Name = "Agente")]
        Agente = 1,

        [Display(Name = "Monitor")]
        Monitor = 2,
    }
}
