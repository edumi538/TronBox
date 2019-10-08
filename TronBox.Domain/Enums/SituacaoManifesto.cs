using System.ComponentModel.DataAnnotations;

namespace TronBox.Domain.Enums
{
    public enum SituacaoManifesto
    {
        [Display(Name = "Sem Manifesto")]
        SemManifesto = 1,

        [Display(Name = "Ciência")]
        Ciencia = 2,

        [Display(Name = "Confirmado")]
        Confirmado = 3,

        [Display(Name = "Desconhecido")]
        Desconhecido = 4,

        [Display(Name = "Não Realizado")]
        NaoRealizado = 5,

        [Display(Name = "Ciência/Autom")]
        CienciaAutomatica = 6,
    }
}
