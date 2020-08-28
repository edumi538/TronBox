using System;
using System.Collections.Generic;
using TronBox.Domain.DTO;

namespace TronBox.Application.Services.Interfaces
{
    public interface IDashboardAppService : IDisposable
    {
        long ContarSemManifesto();
        List<DashboardDocumentosDTO> ObterDadosDocumentosArmazenados(int dataInicial, int dataFinal);
        List<DashboardOrigemDocumentoDTO> ObterDadosOrigemDocumentos(int dataInicial, int dataFinal);
        List<DashboardUltimaSemanaDTO> ObterDadosUltimaSemana(int dataInicial, int dataFinal);
    }
}
