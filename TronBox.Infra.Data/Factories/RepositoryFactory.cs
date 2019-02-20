using TronCore.Dominio.Commands;
using TGCW.Domain.Interfaces;
using TGCW.Infra.Data.Context;
using System.Collections.Generic;
using TronCore.Seguranca.AOP.Auditoria;
using System.Diagnostics;
using TronCore.InjecaoDependencia;
using Microsoft.EntityFrameworkCore;
using System;
using TGCW.Infra.Data.UoW;
using TronCore.Persistencia.Interface;

namespace TronBox.Infra.Data.Factories
{
    public class RepositoryFactory : IRepositoryFactory
    {
        #region Membros

        private GCDbContext _context;
        private IGCUnitOfWork _unitOfWork;
        private readonly IUsuarioLogado _usuarioLogado;
        private static IDictionary<Guid, IRepositoryFactory> _instancias = new Dictionary<Guid, IRepositoryFactory>();
        public DbContext Context { get => _context; set => _context = (GCDbContext)value; }
        #endregion

        public IRepositoryFactory ObtenhaInstancia()
        {
            if (_instancias != null && _context != null && _context.CurrentTenant != null)
            {
                if (_instancias.ContainsKey(_context.CurrentTenant.Id))
                {
                    return _instancias[_context.CurrentTenant.Id];
                }
                _instancias.Add(_context.CurrentTenant.Id, this);
            }
            return this;
        }
        public IRepositoryFactory ObtenhaInstancia(DbContext context)
        {
            if (context != null)
            {
                _context = (GCDbContext)context;
                if (_instancias.ContainsKey(((GCDbContext)context).CurrentTenant.Id))
                    return _instancias[((GCDbContext)context).CurrentTenant.Id];
            }
            return ObtenhaInstancia();
        }

        #region Construtor
        
        public RepositoryFactory(GCDbContext context, IUsuarioLogado usuarioLogado)
        {
            _unitOfWork = new GCUnitOfWork(context);
            _context = context;
            _usuarioLogado = usuarioLogado;
        }

        #endregion

        #region IRepositoryFactory

        public T Instancie<T>() 
        {
            //Crio um proxy da instância do repositorio via ProxyAuditoria
            var instancia = FabricaGeral.Instancie<T>();
                return ProxyAuditoria<T>.Criar(instancia,
                s => Debug.WriteLine("Info:" + s),
                s => Debug.WriteLine("Error:" + s),
                null, _usuarioLogado.GetToken());
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public CommandResponse Commit()
        {
            return _unitOfWork.Commit();
        }

        public void Dispose()
        {
            if (_context != null && _context.CurrentTenant != null)
            {
                _instancias.Remove(_context.CurrentTenant.Id);
            }
        }
        #endregion
    }
}
