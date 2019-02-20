import * as a from './../actionTypes/actionTypes'

const INITIAL_STATE = {
    tiposEmpregado: [],
    generos: [],
    estadosCivis: [],
    racas: [],
    grausInstrucao: [],
    tiposLogradouro: [],
    cidades: [],
    formasPagamento: [],
    bancos: [],
    tiposConta: [],
    unidadesFederativas: [],
    tiposCertidao: [],
    tiposAdmissao: [],
    vinculos: [],
    categorias: [],
    parametrosSalarial: [],
    tiposPagamento: [],
    tiposSalario: []
}

export default ( state = INITIAL_STATE, action ) =>{

    const { type, payload } = action

    switch (type) {
        case a.DEFAULT_GET_TIPOS_EMPREGADO:
            return { ...state, tiposEmpregado: payload.data }
        case a.DEFAULT_GET_GENEROS:
            return { ...state, generos: payload.data }
        case a.DEFAULT_GET_ESTADOS_CIVIS:
            return { ...state, estadosCivis: payload.data }
        case a.DEFAULT_GET_RACAS:
            return { ...state, racas: payload.data }
        case a.DEFAULT_GET_GRAUS_INSTRUCAO:
            return { ...state, grausInstrucao: payload.data }
        case a.DEFAULT_GET_TIPOS_LOGRADOURO:
            return { ...state, tiposLogradouro: payload.data }
        case a.DEFAULT_GET_CIDADES:
            return { ...state, cidades: payload.data }
        case a.DEFAULT_GET_FORMAS_PAGAMENTO:
            return { ...state, formasPagamento: payload.data }
        case a.DEFAULT_GET_BANCOS:
            return { ...state, bancos: payload.data }
        case a.DEFAULT_GET_TIPOS_CONTA:
            return { ...state, tiposConta: payload.data }
        case a.DEFAULT_GET_UNIDADES_FEDERATIVAS:
            return { ...state, unidadesFederativas: payload.data.estados }
        case a.DEFAULT_GET_TIPOS_CERTIDAO:
            return { ...state, tiposCertidao: payload.data }
        case a.DEFAULT_GET_TIPOS_ADMISSAO:
            return { ...state, tiposAdmissao: payload.data }
        case a.DEFAULT_GET_VINCULOS:
            return { ...state, vinculos: payload.data }
        case a.DEFAULT_GET_CATEGORIAS:
            return { ...state, categorias: payload.data }
        case a.DEFAULT_GET_PARAMETROS_SALARIAL:
            return { ...state, parametrosSalarial: payload.data }
        case a.DEFAULT_GET_TIPOS_PAGAMENTO:
            return { ...state, tiposPagamento: payload.data }
        case a.DEFAULT_GET_TIPOS_SALARIO:
            return { ...state, tiposSalario: payload.data }
        default:
            return { ...state };
    }
}
