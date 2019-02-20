import axios from 'axios'
import Util from '../helpers/util'
import { toastr } from 'react-redux-toastr'

import * as a from './../actionTypes/actionTypes'

export const getPessoaLogada = async () => {
    return dispatch => {
        axios.get(`${Util.getBaseUrl()}/pessoas-usuarios/pessoa-logada`, Util.getHeaders())
            .then(responsePessoa => {
                if (responsePessoa.status === 200) {
                    if (responsePessoa.data.nome === "Super Usuário") {
                        dispatch(setPessoaLogada(responsePessoa))
                    } else {
                        dispatch([
                            setPessoaLogada(responsePessoa),
                            getEmpregadoPessoa(responsePessoa.data.id)
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

export function setEstadoInicialUsuarioLogado() {
    return {
        type: a.USR_SET_ESTADO_INICIAL_USUARIO_LOGADO
    }
}

export const setDadosEmpregado = (data) => {
    return {
        type: a.USR_GET_DADOS_EMPREGADO,
        payload: data
    }
}

export const verificaPessoaLogada = async () => {
    return (dispatch, getState) => {
        const { usuario } = getState()

        dispatch({
            type: a.USR_SET_PESSOA_JA_LOGADA,
            payload: usuario.pessoaLogada.id ? true : false
        })
    }
}

export const getEmpregadoPessoa = async (idPessoa) => {
    return dispatch => {
        axios.get(`${Util.getBaseUrl()}/empregados?filtro=[{"campo": "pessoaId", "valor": "${idPessoa}"}]`, Util.getHeaders())
            .then(response => {
                if (response.data.length > 0) {
                    dispatch(setDadosEmpregado(response.data[0]))
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
