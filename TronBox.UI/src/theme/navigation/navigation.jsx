import React, { Component } from 'react'
import { connect } from 'react-redux'
import { bindActionCreators } from 'redux'

import MenuItem from '../menuItem'
import MenuTree from '../menuTree'
import { logOutUser } from '../../actions/defaultActions'
import { getPessoaLogada, setEstadoInicialUsuarioLogado } from '../../actions/usuarioActions'
import { getRotasPermitidas, getRotaInicial } from '../../actions/rotaActions'
import miniLogo from '../assets/images/navigation/nav-header-mini.png'

class Navigation extends Component {

    componentDidUpdate() {
        const { menu } = this.refs
        if (this.props.rota.menus.length > 0) {
            $(function () {
                $(menu).metisMenu({
                    toggle: true
                })
            })
        }
    }

    componentWillUnmount() {
        this.props.setEstadoInicialUsuarioLogado()
    }

    renderMenuTree(menu) {
        return (
            <MenuTree key={menu.funcaoId} icon={menu.icone} label={menu.funcaoDescricao}>
                {
                    menu.menus.map(item => {
                        if (item.menus && item.menus.length > 0) {
                            this.renderMenuTree(item)
                        }
                        else {
                            return (
                                this.renderMenuItemTree(item)
                            )
                        }
                    })
                }
            </MenuTree>
        )
    }

    renderMenuItemTree(menu) {
        return (
            <MenuItem key={menu.funcaoId} path={`#${menu.rota}`} label={menu.funcaoDescricao} tree={true} />
        )
    }

    renderMenuItem(menu) {
        return (
            <MenuItem key={menu.funcaoId} path={`#${menu.rota}`} icon={menu.icone} label={menu.funcaoDescricao} />
        )
    }

    renderNavigation() {
        return (
            <nav className="navbar-default navbar-static-side" role="navigation">
                <div className="sidebar-collapse">
                    <nav className="navbar-default navbar-static-side" role="navigation">
                        <div className="sidebar-collapse">

                            <ul className="nav metismenu" id="side-menu" ref="menu" style={{ zIndex: 2000 }}>
                                <li className="nav-header">
                                    <div className="dropdown profile-element">
                                        <div className="row">
                                            <div className="col-md-3"></div>
                                            <div className="col-md-3">
                                                <span>
                                                    <img alt="image" className="img-circle img-md img-center" src="http://app.tronconnect.com.br/img/tron/new/nav-header-mini.png" />
                                                    {/* <img alt="image" className="img-circle" style={{ width: 48, height: 48 }} src={this.props.usuario.pessoaLogada.foto} /> */}
                                                </span>
                                            </div>
                                        </div>
                                        <a data-toggle="dropdown" className="dropdown-toggle" href="#">
                                            <span className="clear">
                                                <span className="block m-t-xs">
                                                    <strong className="font-bold">{this.props.usuario.pessoaLogada.nome}</strong>
                                                </span>
                                                <span className="block m-t-xs">
                                                    <small>{this.props.usuario.pessoaLogada.empresa}</small>
                                                </span>
                                                <span className="text-muted text-xs block">
                                                    {this.props.usuario.cargo || "Configurações"} <b className="caret"></b>
                                                </span>
                                            </span>
                                        </a>
                                        <ul className="dropdown-menu animated fadeIn m-t-xs">
                                            <li>
                                                {this.props.usuario.pessoaLogada.isEscritorio ?
                                                    <a href={`#/edit-info-pessoa-escritorio/${this.props.usuario.pessoaLogada.id}/${this.props.usuario.pessoaLogada.empresaId}`}>Informações Pessoais</a>
                                                    :
                                                    <a href={`#/edit-info-pessoa-empresa/${this.props.usuario.pessoaLogada.id}/${this.props.usuario.pessoaLogada.empresaId}`}>Informações Pessoais</a>
                                                }
                                                <a href="#" onClick={this.props.logOutUser}>Sair</a>
                                            </li>
                                        </ul>
                                    </div>
                                    <div className="logo-element">
                                    <img alt="image" className="img-sm" src={miniLogo} />
                                    </div>
                                </li>
                                {/* menu */}
                                {this.props.rota.menus.map(menu => {
                                    if (menu.permitido) {
                                        if (menu.menus && menu.menus.length > 0) {
                                            return (
                                                this.renderMenuTree(menu)
                                            )
                                        } else {
                                            return (
                                                this.renderMenuItem(menu)
                                            )
                                        }
                                    }
                                })}
                            </ul >
                        </div>
                    </nav>
                </div>
            </nav>
        )
    }

    render() {
        return (
            <div>
                {this.renderNavigation()}
            </div>
        )
    }
}

const mapStateToProps = state => ({ usuario: state.usuario, default: state.default, rota: state.rota })
const mapDispatchToProps = dispatch => bindActionCreators({
    logOutUser,
    getPessoaLogada,
    setEstadoInicialUsuarioLogado,
    getRotasPermitidas,
    getRotaInicial
}, dispatch)

export default connect(mapStateToProps, mapDispatchToProps)(Navigation)
