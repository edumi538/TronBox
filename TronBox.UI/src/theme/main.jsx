import React, { Component } from 'react'
import { connect } from 'react-redux'
import { bindActionCreators } from 'redux';

import Messages from './messages'
import Progress from './progress'
import Navigation from './navigation/index'
import TopHeader from './topHeader'
import Footer from './footer'
import Loading from '../commons/components/loading'

import Hash from 'react-router-history'

import { correctHeight, detectBody } from './helpers/helpers'
import { getPessoaLogada, verificaPessoaLogada } from '../actions/usuarioActions'
import { getRotasPermitidas } from '../actions/rotaActions'
import { carregaAutenticacao } from '../actions/defaultActions'

import './assets/dependencies'

const hashHistory = Hash.hashHistory

class Main extends Component {
    async setup() {
        await this.props.getRotasPermitidas()
        await this.props.verificaPessoaLogada()

        if (!this.props.usuario.pessoaJaLogada)
            await this.props.getPessoaLogada()
    }

    async componentWillMount() {
        if (!sessionStorage.getItem('token')) {
            if (this.props.params.token) {
                this.props.carregaAutenticacao(this.props.params.token, this.props.params.tenant)
            }
        }

        await this.setup()

        if (this.props.params.token) {
            hashHistory.push('/dashboards/pontualidade')
        }
    }

    componentDidMount() {
        $(window).bind("load resize", function () {
            correctHeight()
            detectBody()
        });

        $('.metismenu a').click(() => {
            setTimeout(() => {
                correctHeight()
            }, 300)
        })
    }

    render() {
        if ((this.props.usuario.empregado != null) || (this.props.usuario.pessoaLogada.tipo != null && this.props.usuario.pessoaLogada.tipo != 'Colaborador')) {
            return (
                <div id="wrapper">
                    <Progress />
                    <Navigation pathName={this.props.location.pathname} />
                    <div id="page-wrapper" className="gray-bg">
                        <TopHeader />
                        {this.props.children}
                        <Footer />
                        <Messages />
                    </div>
                </div>
            )
        } else {
            return (
                <div>
                    <div className="bg-login">
                        <Loading
                            title="Estamos preparando tudo para você! Aguarde..."
                            type="bars" />
                    </div>
                </div>
            )
        }
    }
}

const mapStateToProps = state => ({ usuario: state.usuario });

const mapDispatchToProps = dispatch => bindActionCreators({
    getPessoaLogada,
    getRotasPermitidas,
    carregaAutenticacao,
    verificaPessoaLogada
}, dispatch)

export default connect(mapStateToProps, mapDispatchToProps)(Main)
