import * as a from './../actionTypes/actionTypes'

const INITIAL_STATE = {
    pessoaLogada: {
        id: null,
        nome: null,
        tipo: null,
        empresaId: null,
        empresa: null,
        email: null
    },
    empregado: null,
    pessoaJaLogada: false
}

export default (state = INITIAL_STATE, action) => {

    const { type, payload } = action

    switch (type) {
        case a.USR_GET_PESSOA_LOGADA:
            return {
                ...state, pessoaLogada: {
                    id: payload.data.id,
                    nome: payload.data.nome,
                    tipo: payload.data.tipo,
                    empresaId: payload.data.empresaId,
                    empresa: payload.data.empresa,
                    email: payload.data.email
                }
            }
        case a.USR_GET_DADOS_EMPREGADO:
            return { ...state, empregado: payload }
        case a.USR_SET_ESTADO_INICIAL_USUARIO_LOGADO:
            return { ...state, pessoaLogada: {}, empregado: false, empregado: null }
        case a.USR_SET_PESSOA_JA_LOGADA:
            return { ...state, pessoaJaLogada: payload }
        default:
            return { ...state };
    }
}
