import axios from 'axios'
import Util from '../helpers/util'

import { change } from 'redux-form'
import { toastr } from 'react-redux-toastr'

import * as a from '../actionTypes/actionTypes'

export const carregaAutenticacao = async (token, tenant) => {
    sessionStorage.setItem('token', token)
    sessionStorage.setItem('tenant', tenant)
}

export const logOutUser = () =>{
    sessionStorage.removeItem('token')
    sessionStorage.removeItem('tenant')
}

export const getTiposEmpregado = () => {
    const request = axios.get(`${Util.getBaseUrl()}/enumeradores/tipos-empregado`, Util.getHeaders());

    return {
        type: a.DEFAULT_GET_TIPOS_EMPREGADO,
        payload: request
    }
}

export const getGeneros = () => {
    const request = axios.get(`${Util.getBaseUrl()}/enumeradores/generos`, Util.getHeaders());

    return {
        type: a.DEFAULT_GET_GENEROS,
        payload: request
    }
}

export const getEstadosCivis = () => {
    const request = axios.get(`${Util.getBaseUrl()}/enumeradores/estados-civis`, Util.getHeaders());

    return {
        type: a.DEFAULT_GET_ESTADOS_CIVIS,
        payload: request
    }
}

export const getRacas = () => {
    const request = axios.get(`${Util.getBaseUrl()}/enumeradores/racas`, Util.getHeaders());

    return {
        type: a.DEFAULT_GET_RACAS,
        payload: request
    }
}

export const getGrausInstrucao = () => {
    const request = axios.get(`${Util.getBaseUrl()}/enumeradores/graus-instrucao`, Util.getHeaders());

    return {
        type: a.DEFAULT_GET_GRAUS_INSTRUCAO,
        payload: request
    }
}

export const getTiposLogradouro = () => {
    const request = axios.get(`${Util.getBaseUrl()}/enumeradores/tipos-logradouro`, Util.getHeaders());

    return {
        type: a.DEFAULT_GET_TIPOS_LOGRADOURO,
        payload: request
    }
}

export const getCidades = () => {
    const request = axios.get('https://servicodados.ibge.gov.br/api/v1/localidades/municipios');

    return {
        type: a.DEFAULT_GET_CIDADES,
        payload: request
    }
}

export const getFormasPagamento = () => {
    const request = axios.get(`${Util.getBaseUrl()}/enumeradores/formas-pagamento`, Util.getHeaders());

    return {
        type: a.DEFAULT_GET_FORMAS_PAGAMENTO,
        payload: request
    }
}

export const getBancos = () => {
    const request = axios.get(`${Util.getBaseUrl()}/enumeradores/bancos`, Util.getHeaders());

    return {
        type: a.DEFAULT_GET_BANCOS,
        payload: request
    }
}

export const getTiposConta = () => {
    const request = axios.get(`${Util.getBaseUrl()}/enumeradores/tipos-conta`, Util.getHeaders());

    return {
        type: a.DEFAULT_GET_TIPOS_CONTA,
        payload: request
    }
}

export const getUnidadesFederativas = () => {
    const request = axios.get(`${Util.getBaseUrl()}/cidades/estados`, Util.getHeaders());

    return {
        type: a.DEFAULT_GET_UNIDADES_FEDERATIVAS,
        payload: request
    }
}

export const getTiposCertidao = () => {
    const request = axios.get(`${Util.getBaseUrl()}/enumeradores/tipos-certidao`, Util.getHeaders());

    return {
        type: a.DEFAULT_GET_TIPOS_CERTIDAO,
        payload: request
    }
}

export const getTiposAdmissao = () => {
    const request = axios.get(`${Util.getBaseUrl()}/enumeradores/tipos-admissao`, Util.getHeaders());

    return {
        type: a.DEFAULT_GET_TIPOS_ADMISSAO,
        payload: request
    }
}

export const getVinculos = () => {
    const request = axios.get(`${Util.getBaseUrl()}/enumeradores/vinculos`, Util.getHeaders());

    return {
        type: a.DEFAULT_GET_VINCULOS,
        payload: request
    }
}

export const getCategorias = () => {
    const request = axios.get(`${Util.getBaseUrl()}/enumeradores/categorias`, Util.getHeaders());

    return {
        type: a.DEFAULT_GET_CATEGORIAS,
        payload: request
    }
}

export const getParametrosSalarial = () => {
    const request = axios.get(`${Util.getBaseUrl()}/enumeradores/parametros-salarial`, Util.getHeaders());

    return {
        type: a.DEFAULT_GET_PARAMETROS_SALARIAL,
        payload: request
    }
}

export const getTiposPagamento = () => {
    const request = axios.get(`${Util.getBaseUrl()}/enumeradores/tipos-pagamento`, Util.getHeaders());

    return {
        type: a.DEFAULT_GET_TIPOS_PAGAMENTO,
        payload: request
    }
}

export const getTiposSalario = () => {
    const request = axios.get(`${Util.getBaseUrl()}/enumeradores/tipos-salario`, Util.getHeaders());

    return {
        type: a.DEFAULT_GET_TIPOS_SALARIO,
        payload: request
    }
}

export const getDadosCep = () => {
    return (dispatch, getState) => {

        const { form } = getState();

        const cep = form.empregadoForm.values.dadosPessoais.cep;

        if (cep && cep.length > 7) {
            const cepSemMascara = cep.replace(/[^0-98]/g, '');

            axios.get(`https://viacep.com.br/ws/${cepSemMascara}/json/`)
                .then(response => {
                    if (response.data.erro) {
                        toastr.error('Parâmetro', 'Não foi possível encontrar o endereço pelo CEP informado.');
                    } else {
                        dispatch([
                            change('empregadoForm', 'dadosPessoais.logradouro', response.data.logradouro),
                            change('empregadoForm', 'dadosPessoais.bairro', response.data.bairro),
                            change('empregadoForm', 'dadosPessoais.cidade', response.data.ibge)
                        ]);
                    }
                })
                .catch(error => toastr.error('Parâmetro', 'Não foi possível encontrar o endereço pelo CEP informado.'));
        }
    }
}

export const onChangeCheckBox = (form, field, value) => {
    return dispatch => {
        dispatch(change(form, field, value))
    }
}
