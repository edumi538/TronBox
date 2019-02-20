using Comum.Domain.Aggregates.PessoaAgg;
using Comum.Domain.Aggregates.PessoaAgg.Repository;
using Microsoft.AspNetCore.Http;
using Sentinela.Domain.Interfaces;
using System;
using System.Linq;
using TGCW.Domain.Aggregates.PessoaAgg;
using TGCW.Domain.Aggregates.PessoaAgg.Repository;
using TGCW.Domain.Interfaces;
using TronCore.DefinicoesConfiguracoes;
using TronCore.InjecaoDependencia;
using TronCore.Utilitarios;

namespace TGCW.Infra.IoC.Services
{
    public class UsuarioLogado : IUsuarioLogado
    {   
        private readonly Guid _usuarioId;
        private readonly Guid _tenantId;
        private readonly string _token;
        private Pessoa _pessoa;

        public UsuarioLogado(
            IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext != null && httpContextAccessor.HttpContext.Request.Headers["Authorization"].Count > 0)
            {
                var token = httpContextAccessor.HttpContext.Request.Headers["Authorization"].First().Replace("Bearer ", string.Empty);
                if (!token.Equals("undefined"))
                {
                    dynamic jsonDeserialize = UtilitarioToken.Deserializar(token, Constantes.CHAVE_TOKEN);

                    _tenantId   = Guid.Parse(Convert.ToString(jsonDeserialize.tid));
                    _usuarioId  = Guid.Parse(Convert.ToString(jsonDeserialize.userId));

                    var pessoaRepository = FabricaGeral.Instancie<IPessoaRepository>();

                    _pessoa     = pessoaRepository.BuscarPessoaDoUsuario(_usuarioId);
                    _token = token;
                }
            }
            else
            {
                _tenantId    = Guid.Empty;
                _usuarioId   = Guid.Empty;
            }
        }
        
        public string GetEmpresaNome()
        {
            return string.Empty;
        }

        public Guid GetUsuarioId()
        {
            return _usuarioId;
        }

        public Pessoa GetPessoa()
        {
            if (_pessoa != null)
            {
                return _pessoa;
            }

            return null;
        }

        public bool EhUsuarioDoEscritorio()
        {
            if (_pessoa != null)
            {
                return _pessoa.IsEscritorio;
            }

            return true;
        }

        public Guid GetTenantId()
        {
            if(_pessoa != null)
            {
                return _tenantId;
            }

            return Guid.Empty;
        }

        public string GetToken()
        {
            return _token;
        }
    }
}
