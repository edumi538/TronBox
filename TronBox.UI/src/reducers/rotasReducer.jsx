import * as a from '../actionTypes/actionTypes'

const INITIAL_STATE = {
    rotas: [],
    menus: []
}

export default ( state = INITIAL_STATE, action ) =>{

    const { type, payload } = action

    switch (type) {
        case a.ROT_GET_ROTAS_PERMITIDAS:
            return { ...state, rotas: payload.data.rotasPermitidas }
        case a.ROT_SET_ROTAS_PERMITIDAS:
            return { ...state, rotas: payload }
        case a.ROT_SET_MENUS:
            return { ...state, menus: payload }
        default:
            return state;
    }
}
