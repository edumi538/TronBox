import React, { Component } from 'react'
import { connect } from 'react-redux'
import { bindActionCreators } from 'redux'

import { getPessoaLogada, setEstadoInicialUsuarioLogado } from '../../actions/usuarioActions'
import { getRotasPermitidas, getRotaInicial } from '../../actions/rotaActions'
import Navigation from './navigation'

class Index extends Component {

    async componentWillMount() {
        await this.props.getRotaInicial()
        // this.props.getEscritorioAtivo('')
        if (sessionStorage.getItem('token') && sessionStorage.getItem('tenant')) {
            await this.props.getRotasPermitidas()
            if (!this.props.usuario.pessoaJaLogada) {
                await this.props.getPessoaLogada()
            }
        }
    }

    render() {
        return (
           <Navigation />
        )
    }
}

const mapStateToProps = state => ({ usuario: state.usuario, default: state.default, rota: state.rota })
const mapDispatchToProps = dispatch => bindActionCreators({
    getPessoaLogada,
    setEstadoInicialUsuarioLogado,
    getRotasPermitidas,
    getRotaInicial
}, dispatch)

export default connect(mapStateToProps, mapDispatchToProps)(Index)
