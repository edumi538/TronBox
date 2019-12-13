using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TronBox.Domain.DTO;
using TronBox.Domain.DTO.InnerClassDTO;

namespace TronBox.Application.Services.Interfaces
{
    public interface IDocumentoFiscalAppService : IDisposable
    {
        Task<IEnumerable<RetornoDocumentoFiscalDTO>> Inserir(EnviarArquivosDTO arquivos);
        Task<byte[]> DownloadPDF(Guid id);
        IEnumerable<DocumentoFiscalDTO> BuscarTodos(string filtro);
        IEnumerable<DocumentoFiscalDTO> BuscarPendentes(string filtro);
        Task<DetalhesDocumentoFiscalDTO> BuscarPorId(Guid id);
        void Deletar(Guid id);
        bool ExisteNaoProcessado();
        void ConfirmarImportacao(Guid id, DadosImportacaoDTO dadosImportacao);
    }
}
