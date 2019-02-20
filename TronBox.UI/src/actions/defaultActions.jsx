import axios from 'axios'

import { change } from 'redux-form'
import { toastr } from 'react-redux-toastr'

export const carregaAutenticacao = async (token, tenant) => {
    sessionStorage.setItem('token', token)
    sessionStorage.setItem('tenant', tenant)
}

export const logOutUser = () => {
    sessionStorage.removeItem('token')
    sessionStorage.removeItem('tenant')
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
