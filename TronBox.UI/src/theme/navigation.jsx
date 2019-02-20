import React, { Component } from 'react'
import { connect } from 'react-redux'
import { bindActionCreators } from 'redux'

import MenuItem from './menuItem'

import miniLogo from './assets/images/navigation/nav-header-mini.png'
import avatar from './assets/images/navigation/nav-header.png'

class Navigation extends Component {

    componentDidMount() {
        const { menu } = this.refs

        $(function () {
            $(menu).metisMenu({
                toggle: true
            })
        })
    }

    renderNavigation() {
        return (
            <ul className="nav metismenu" id="side-menu" ref="menu">
                <li className="nav-header">
                    <div className="dropdown profile-element">
                        <img alt="image" src={avatar}></img>
                        <a data-toggle="dropdown" className="dropdown-toggle" href="#">
                            <span className="clear m-t m-b-n">
                                <span className="block m-t-xs">
                                    <strong className="text-muted text-sm">Configurações</strong>
                                </span>
                                <span className="text-muted text-sm">email <b className="caret"></b></span>
                            </span>
                        </a>
                        <ul className="dropdown-menu m-t-xs">
                            <li><a href="">Informações Pessoais</a></li>
                            <li className="divider"></li>
                            <li><a href="#/alterar-senha">Aterar Senha</a></li>
                        </ul>
                    </div>
                    <div className="logo-element">
                        <img alt="image" className="img-sm" src={miniLogo} />
                    </div>
                </li>
                {/* menu */}
                <MenuItem path="#/home" icon="home" label="Home" />
                <MenuItem path="#/empregados" icon="archive" label="Empregados" />
                <MenuItem path="#/programacoes-ferias" icon="archive" label="Programação de Férias" />
            </ul>
        )
    }

    render() {
        return (
            <nav className="navbar-default navbar-static-side" role="navigation">
                <div className="sidebar-collapse">
                    {this.renderNavigation()}
                </div>
            </nav>
        )
    }
}

const mapStateToProps = state => ({ default: state.default })
const mapDispatchToProps = dispatch => bindActionCreators({ }, dispatch)

export default connect(mapStateToProps, mapDispatchToProps)(Navigation)
