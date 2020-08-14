using System;
using System.Collections.Generic;
using TronBox.Domain.DTO;

namespace TronBox.Application.Services.Interfaces
{
    public interface IHistoricoConsultaMatoGrossoAppService : IDisposable
    {
        IEnumerable<HistoricoConsultaMatoGrossoDTO> BuscarTodos(string filtro);
        void Inserir(HistoricoConsultaMatoGrossoDTO historicoConsulta);
        DateTime? ObterUltimoPeriodo(string inscricaoEstadual);
    }
}
