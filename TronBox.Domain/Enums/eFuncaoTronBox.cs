using System;
using System.Collections.Generic;
using System.Linq;
using TronCore.Enumeradores;

namespace TronBox.Domain.Enums
{
    public class eFuncaoTronBox : eFuncao
    {
        #region Funcoes
        
        #endregion

        #region Agrupadores
        
        #endregion

        /// <summary>
        /// Construtor padrão para a chamada do construtor da classe base.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="descricao"></param>
        /// <param name="produto"></param>
        /// <param name="rota"></param>
        /// <param name="icone"></param>
        public eFuncaoTronBox(object id,  string descricao, Modulo produto, string rota, string icone, int ordem) : base(id, descricao, produto, rota, icone, ordem)
        {
        }

        #region Propriedades - Funções

        #region Apenas API
        #endregion

        #endregion

        #region Propriedades - Agrupadores

        #endregion

        public new static IList<eFuncao> ObtenhaTodos()
        {
            return ObtenhaTodos(typeof(eFuncaoTronBox)).ToList();
        }
    }
}
