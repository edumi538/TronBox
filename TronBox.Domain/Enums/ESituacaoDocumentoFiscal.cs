﻿using System.ComponentModel.DataAnnotations;

namespace TronBox.Domain.Enums
{
    public enum ESituacaoDocumentoFiscal
    {
        [Display(Name = "Armazenado")]
        Armazenado = 1,

        [Display(Name = "Pendente")]
        Pendente = 2,

        [Display(Name = "Não Armazenado")]
        NaoArmazenado = 3,
    }
}
