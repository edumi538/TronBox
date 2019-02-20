import _ from 'lodash'

class Util {
    static getBaseUrl() {
        return location.href.indexOf('localhost') > 0 ? 'http://localhost:6004/api' : '/api';
    }
    static getBaseUrlSentinela() {
        return location.href.indexOf('localhost') > 0 ? 'http://localhost:6005/api' : 'http://login.tron.com.br/api';
    }

    static getUrlSentinela() {
        return location.href.indexOf('localhost') > 0 ? 'http://localhost:6005/' : 'http://login.tron.com.br';
    }

    static getHeaders() {
        return { headers: { 'Authorization': `Bearer ${sessionStorage.token}`, 'Accept': 'application/json', 'ServiceIdentify': `${sessionStorage.tenant}` } }
    }

    static getHeadersFunctionName(functionName) {
        return { headers: { 'Authorization': `Bearer ${sessionStorage.token}`, 'Accept': 'application/json', 'NomeFuncao': functionName, 'ServiceIdentify': `${sessionStorage.tenant}` } }
    }

    static getHeaders2(token) {
        return { headers: { 'Authorization': `Bearer ${token}`, 'Accept': 'application/json', 'ServiceIdentify': `${sessionStorage.tenant}` } }
    }

    static getHeaders3() {
        return { headers: { 'Authorization': `Bearer ${sessionStorage.token}`, 'Accept': 'application/json' } }
    }

    static possuiPermissao(controller, action) {
        let temPermissao = true
        const permissoes = JSON.parse(sessionStorage.getItem('permissoes'))
        _.forEach(permissoes, function (permissao) {
            if (permissao.rota === controller) {
                _.forEach(permissao.operacoes, function (operacao) {
                    if (operacao.rota === action && !operacao.permitido) {
                        temPermissao = false
                    }
                })
            }
        })

        return temPermissao
    }
}

export default Util
