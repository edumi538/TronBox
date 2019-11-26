using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TronBox.Domain.DTO;
using TronBox.Domain.Enums;

namespace TronBox.Application.Services.Interfaces
{
    public interface IHistoricoConsultaAppService : IDisposable
    {
        IEnumerable<HistoricoConsultaDTO> BuscarTodos(string filtro);
        void Inserir(HistoricoConsultaDTO historicoConsulta);
        HistoricoConsultaDTO ObterUltimaConsulta();
        string ObterUltimoNSU(ETipoDocumentoConsulta tipoDocumento);
        void BuscarManualmente();
    }
}
