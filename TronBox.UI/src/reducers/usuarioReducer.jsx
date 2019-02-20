import * as a from './../actionTypes/actionTypes'

const INITIAL_STATE = {
    pessoaLogada: {
        nome: null,
        foto: null,
        tipo: null,
        empresa: null,
        isEscritorio: null,
        empresaId: null
    },
    pessoaJaLogada: false,
    cargo: null
}

export default ( state = INITIAL_STATE, action ) =>{

    const { type, payload } = action

    switch (type) {
        case a.USR_GET_PESSOA_LOGADA:
            return {
                ...state, pessoaLogada: {
                    id: payload.data.id,
                    nome: payload.data.nome,
                    foto: payload.data.foto,
                    tipo: payload.data.tipo,
                    empresa: payload.data.empresa,
                    empresaId: payload.data.empresaId,
                    isEscritorio: payload.data.isEscritorio,
                }
            }
        case a.USR_GET_CARGO_EMPREGADO:
            return { ...state, cargo : payload.data.descricao }
        case a.USR_SET_ESTADO_INICIAL_USUARIO_LOGADO:
            return {
                pessoaLogada: {}
            }
        default:
            return { ...state };
    }
}
