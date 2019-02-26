using System;
using System.Collections.Generic;
using TronBox.Domain.DTO;

namespace TronBox.Application.Services.Interfaces
{
    public interface ICustomerAppService: IDisposable
    {
        void Inserir(CustomerDTO customerDTO);
        void Atualizar(CustomerDTO customerDTO);

        void Deletar(Guid id);

        IEnumerable<CustomerDTO> BuscarTodos(string filtro);
    }
}
