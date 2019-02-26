using System;
using System.Threading.Tasks;

namespace TronBox.Domain.Interfaces
{
    public interface ITenantUnitOfWork : IDisposable
    {
        void Save();
        Task SaveAsync();
    }
}
