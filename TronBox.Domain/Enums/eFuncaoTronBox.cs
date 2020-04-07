using System;
using System.Collections.Generic;
using System.Linq;
using TGCW.Domain.Enums;
using TronCore.Enumeradores;

namespace TronBox.Domain.Enums
{
    public class eFuncaoTronBox : eFuncao
    {
        public const string ID_DASHBOARD = "215834BB-C102-41D1-8AAF-B61B69CA8D5B";
        public const string ID_EMPRESA = "11E7F776-3BCF-462E-8388-1807472C2B0B";
        public const string ID_MANIFESTO = "FA95626C-566D-452E-8917-9713495D023F";
        public const string ID_DOCUMENTO_FISCAL = "F24BF7BE-05A1-49C5-8496-B6E867B91605";
        public const string ID_UPLOAD = "4607A76A-87EC-4C87-B067-C5B05B2D5281";
        public const string ID_AGR_HISTORICO_CONSULTA = "60F78EDA-DA6F-4C6E-96C9-C7084E826285";
        public const string ID_HISTORICO_CONSULTA = "3811D14A-11C3-48D8-908E-9827864CC581";
        public const string ID_HISTORICO_CONSULTA_SEFAZ_MT = "E0C1AC1E-2A7C-401F-8D50-C804DD384691";

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
        public static eFuncao Dashboard { get { return new eFuncao(Guid.Parse(ID_DASHBOARD), "Dashboard", Modulo.Box, "/dashboards", "tachometer", eTipoFuncao.ROTA_MENU, 1); } }
        public static eFuncao Empresa { get { return new eFuncao(Guid.Parse(ID_EMPRESA), "Empresa", Modulo.Box, "/empresas", "building", eTipoFuncao.ROTA_MENU, 2); } }
        public static eFuncao DocumentoFiscal { get { return new eFuncao(Guid.Parse(ID_DOCUMENTO_FISCAL), "Consulta Documentos", Modulo.Box, "/documentos-fiscais", "file-code-o", eTipoFuncao.ROTA_MENU, 3); } }
        public static eFuncao Upload { get { return new eFuncao(Guid.Parse(ID_UPLOAD), "Enviar Documentos", Modulo.Box, "/enviar-documentos", "cloud-upload", eTipoFuncao.ROTA_MENU, 4); } }
        public static eFuncao Manifesto { get { return new eFuncao(Guid.Parse(ID_MANIFESTO), "Manifesto", Modulo.Box, "/manifestos", "file-text", eTipoFuncao.ROTA_MENU, 5); } }
        public static eFuncao AgrupadorHistoricoConsulta { get { return new eFuncao(Guid.Parse(ID_AGR_HISTORICO_CONSULTA), "Histórico de Consultas", Modulo.Box, "", "history", eTipoFuncao.AGRUPADOR, 6); } }
        public static eFuncao HistoricoConsultaPortalNacional { get { return new eFuncao(Guid.Parse(ID_HISTORICO_CONSULTA), "Portal Nacional", Modulo.Box, "/historicos-consulta", "", eTipoFuncao.ROTA_MENU, AgrupadorHistoricoConsulta, 1); } }
        public static eFuncao HistoricoConsultaSefazMt { get { return new eFuncao(Guid.Parse(ID_HISTORICO_CONSULTA_SEFAZ_MT), "SEFAZ MT", Modulo.Box, "/historicos-consulta-sefaz-mt", "", eTipoFuncao.ROTA_MENU, AgrupadorHistoricoConsulta, 2); } }

        public new static IList<eFuncao> ObtenhaTodos() => ObtenhaTodos(typeof(eFuncaoTronBox)).ToList();
    }
}
