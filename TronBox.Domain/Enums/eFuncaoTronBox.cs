﻿using System;
using System.Collections.Generic;
using System.Linq;
using TGCW.Domain.Enums;
using TronCore.Enumeradores;

namespace TronBox.Domain.Enums
{
    public class eFuncaoTronBox : eFuncao
    {
        public const string ID_EMPRESA = "11E7F776-3BCF-462E-8388-1807472C2B0B";
        public const string ID_MANIFESTO = "FA95626C-566D-452E-8917-9713495D023F";

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

        public static eFuncao Empresa { get { return new eFuncao(Guid.Parse(ID_EMPRESA), "Empresa", Modulo.Box, "/empresas", "building", eTipoFuncao.ROTA_MENU, 1); } }
        public static eFuncao Manifesto { get { return new eFuncao(Guid.Parse(ID_MANIFESTO), "Manifesto", Modulo.Box, "/manifestos", "file-text", eTipoFuncao.ROTA_MENU, 2); } }

        public new static IList<eFuncao> ObtenhaTodos() => ObtenhaTodos(typeof(eFuncaoTronBox)).ToList();
    }
}
