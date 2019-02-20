using TronCore.Dominio.Commands;
using System.Collections.Generic;
using TronCore.Seguranca.AOP.Auditoria;
using System.Diagnostics;
using TronCore.InjecaoDependencia;
using Microsoft.EntityFrameworkCore;
using System;
using TronCore.Persistencia.Interfaces;
using Comum.Infra.Data.Context;
using Comum.Infra.Data.UoW;
using TronCore.Domain.Interfaces;
using Sentinela.Domain.Interfaces;
using TronCore.Persistencia.Context;
using TronBox.Domain.Interfaces;

namespace TronBox.Infra.Data.Factories
{
    public class RepositoryFactoryNoSql : IRepositoryFactory
    {
        #region Membros

        private SuiteMongoDbContext _context;
        private readonly IUsuarioLogado _usuarioLogado;
        private static IDictionary<Guid, IRepositoryFactoryNoSql> _instancias = new Dictionary<Guid, IRepositoryFactoryNoSql>();
        public MongoDbContext Context { get => _context; set => _context = (SuiteMongoDbContext)value; }
        MongoDbContext IRepositoryFactoryNoSql.Context { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        #endregion

        public IRepositoryFactoryNoSql ObtenhaInstancia()
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
        public IRepositoryFactoryNoSql ObtenhaInstancia(MongoDbContext context)
        {
            if (context != null)
            {
                _context = (SuiteMongoDbContext)context;
                if (_instancias.ContainsKey(((SuiteMongoDbContext)context).CurrentTenant.Id))
                    return _instancias[((SuiteMongoDbContext)context).CurrentTenant.Id];
            }
            return ObtenhaInstancia();
        }

        #region Construtor

        public RepositoryFactoryNoSql(SuiteMongoDbContext context, IUsuarioLogado usuarioLogado)
        {
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
            //_context.SaveChanges();
        }

        public CommandResponse Commit()
        {
            return default(CommandResponse);
        }

        public void Dispose()
        {
            if (_context != null && _context.CurrentTenant != null)
            {
                _instancias.Remove(_context.CurrentTenant.Id);
            }
        }

        public IRepositoryFactoryNoSql ObtenhaInstancia(DbContext context)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
