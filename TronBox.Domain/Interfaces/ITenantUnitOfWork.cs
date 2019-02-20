using System;
using System.Threading.Tasks;

namespace TronConnect.Domain.Interfaces
{
    public interface ITenantUnitOfWork : IDisposable
    {
        void Save();
        Task SaveAsync();
    }
}
