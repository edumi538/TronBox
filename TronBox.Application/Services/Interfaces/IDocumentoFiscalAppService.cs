﻿using System;
using System.Collections.Generic;
using TronBox.Domain.DTO;

namespace TronBox.Application.Services.Interfaces
{
    public interface IDocumentoFiscalAppService : IDisposable
    {
        void Inserir(DocumentoFiscalDTO documentoFiscalDTO);
        void Atualizar(DocumentoFiscalDTO documentoFiscalDTO);
        void Deletar(Guid id);
        IEnumerable<DocumentoFiscalDTO> BuscarTodos(string filtro);
        DocumentoFiscalDTO BuscarPorId(Guid id);
    }
}