using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TronBox.Domain.DTO;

namespace TronBox.Application.Services.Interfaces
{
    public interface IDocumentoFiscalAppService : IDisposable
    {
        Task<IEnumerable<string>> Inserir(EnviarArquivosDTO arquivos);
        void Atualizar(DocumentoFiscalDTO documentoFiscalDTO);
        Task<byte[]> DownloadPDF(Guid id);
        IEnumerable<DocumentoFiscalDTO> BuscarTodos(string filtro);
        Task<DetalhesDocumentoFiscalDTO> BuscarPorId(Guid id);
        void Deletar(Guid id);
    }
}
