import axios from 'axios'
import Util from '../helpers/util'
import { toastr } from 'react-redux-toastr'
import * as a from './../actionTypes/actionTypes'

export const getPessoaLogada = () => {
    return dispatch => {
        axios.get(`${Util.getBaseUrl()}/pessoas-usuarios/pessoa-logada`, Util.getHeaders())
            .then(responsePessoa => {
                if (responsePessoa.status === 200) {
                    if (responsePessoa.data.nome === "Super Usuário") {
                        dispatch(setPessoaLogada(responsePessoa))
                    } else {
                        dispatch([
                            setPessoaLogada(responsePessoa),
                            getCargoEmpregado(responsePessoa.data.id)
                        ])
                    }
                }
            }).catch(error => {
                if (error.response.data) {
                    toastr.warning('Dados do Usuário', `${error.response.data.erro}`, { timeOut: 8000 });
                } else {
                    toastr.error('Dados do Usuário', `${error.message}`, { timeOut: 8000 });
                }
            })
    }
}

export const setPessoaLogada = values => {
    return {
        type: a.USR_GET_PESSOA_LOGADA,
        payload: values
    }
}

export const verificaPessoaLogada = () => {
    return (dispatch, getState) => {
        const { usuario } = getState()
        dispatch({ type: a.USR_SET_PESSOA_JA_LOGADA }, usuario.id ? true : false)
    }
}

export function setEstadoInicialUsuarioLogado() {
    return {
        type: a.USR_SET_ESTADO_INICIAL_USUARIO_LOGADO
    }
}

export const getCargoEmpregado = (idPessoa) => {
    return dispatch => {
        axios.get(`${Util.getBaseUrl()}/cargos/cargo-pessoa/${idPessoa}`, Util.getHeaders())
        .then(response => {
            dispatch({
                type: a.USR_GET_CARGO_EMPREGADO,
                payload: response
            })
        }).catch(error => {
            toastr.error('Dados do usuário', 'Opps... Não foi possível carregar o cargo do usuário!', {timeOut: 6000})
        })
    }
}
