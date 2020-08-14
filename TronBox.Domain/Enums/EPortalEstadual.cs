using System.ComponentModel.DataAnnotations;

namespace TronBox.Domain.Enums
{
    public enum EPortalEstadual
    {
        [Display(Name = "SEFAZ MT")]
        SefazMt = 1,

        [Display(Name = "ICMS Transparente - MS")]
        ImcsTransparenteMs = 2,
    }
}
