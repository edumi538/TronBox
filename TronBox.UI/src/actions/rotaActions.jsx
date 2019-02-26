import axios from 'axios'
import { toastr } from 'react-redux-toastr'
import Util from '../helpers/util'
import PagesConstants from '../helpers/consts/pageConsts'
import * as a from '../actionTypes/actionTypes'
/**
 * Imports the pages.
 */
import DashboardPainel from '../pages/dashboard'

export const getRotaInicial = async () => {
    return dispatch => {
        let rotas = [
            {
                path: '/home',
                exact: true,
                component: renderPage('/home')
            },
            {
                path: '/home/:token/:tenant',
                exact: true,
                component: renderPage('/home/:token/:tenant')
            }
        ]
        dispatch(setRotasPermitidas(rotas))
    }
}

/**
 * Function to set routes.
 * @param {Array} rotas
 */
export const setRotasPermitidas = rotas => {
    return {
        type: a.ROT_SET_ROTAS_PERMITIDAS,
        payload: rotas
    }
}

export const setMenus = menus => {
    return {
        type: a.ROT_SET_MENUS,
        payload: menus
    }
}

/**
 * Function to search for allowed routes.
 */
export const getRotasPermitidas = async () => {
    return dispatch => {
        getRotasPromise().then(response => {
            var listaRotas = []
            response.rotas.map(rota => {
                rota.component = renderPage(rota.path)
                rota.exact = true
                if (rota.isAuthenticated) {
                    rota.onEnter = requireAuth
                }

                listaRotas.push(rota)
            })
            dispatch([
                setRotasPermitidas(listaRotas),
                setMenus(response.menus)
            ])
        }).catch(error => {
            toastr.error('Busca Rotas', `Opps... Erro ao tentar buscar as rotas \n Erro: ${error.message}`)
        })
    }
}

/**
 * Function to get promise routes. Buscando rotas do Módulo BOX - 32.
 */
const getRotasPromise = async () => {
    const modulo = 32;
    return new Promise((resolve, reject) => {
        axios.get(`${Util.getBaseUrlSentinela()}/rotas/rotas-permitidas/${modulo}`, Util.getHeaders())
            .then((response => {
                resolve(response.data)
            })).catch(error => {
                reject(error)
            })
    })
}

function requireAuth(nextState, replace) {
    if (!sessionStorage.token) {
        replace({
            pathname: `${Util.getUrlSentinela()}`,
            state: { nextPathname: nextState.location.pathname }
        })
    }

    const permissoes = JSON.parse(sessionStorage.getItem('permissoes'))
    const isEscritorio = JSON.parse(sessionStorage.getItem('isEscritorio'))
    const rota = nextState.location.pathname

    let controller = _.split(rota, '/', 2)[1]
    let action = _.split(rota, '/', 3).length > 2 ? _.split(rota, '/', 3)[2] : 'index'

    if (rota === '/empresa/obrigacao') {
        controller = 'empresa/obrigacao'
        action = 'index'
    }

    _.forEach(permissoes, function (permissao) {
        if (isEscritorio == 1) {
            if (permissao.rota === controller) {
                _.forEach(permissao.operacoes, function (operacao) {
                    if (operacao.rota === action && !operacao.permitido) {
                        toastr.error('Permissões', 'Ops... Você não tem permissão para executar essa operação. Caso seja necessário fale com o Administrador do sistema!')
                        replace({
                            pathname: '/home',
                            state: { nextPathname: nextState.location.pathname }
                        })
                    }
                })
            }
        }
    })
}

/**
* Function render pages.
* @param {class} page
*/
const renderPage = (page) => {
    switch (page) {
        case PagesConstants.DASHBOARD_PAINEL:
            return DashboardPainel;
        default:
            return DashboardPainel;
    }
}
