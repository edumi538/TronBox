import React, { Component } from 'react'
import { smoothlyMenu } from './helpers/helpers'
import { connect } from 'react-redux'
import { bindActionCreators } from 'redux'
import { Tooltip } from 'antd'

import { logOutUser } from '../actions/defaultActions'

import Util from '../helpers/util'

class TopHeader extends Component {

    toggleNavigation(e) {
        e.preventDefault()
        $("body").toggleClass("mini-navbar")
        smoothlyMenu()
    }

    refreshPage() {
        window.location.reload();
    }

    render() {
        return (
            <div className="row border-bottom">
                <nav className="navbar navbar-static-top" role="navigation" style={{ marginBottom: 0 }}>
                    <div className="navbar-header">
                        <a className="minimalize-styl-2 btn btn-primary" onClick={(e) => this.toggleNavigation(e)} href="#"><i className="fa fa-bars"></i> </a>
                    </div>
                    <ul className="nav navbar-top-links navbar-right">
                        <li>
                            <Tooltip title="Recarregar página!">
                                <a onClick={() => this.refreshPage()}>
                                    <i className="fa fa-refresh" />
                                </a>
                            </Tooltip>
                        </li>
                        <li>
                            <a href={Util.getUrlSentinela()} onClick={this.props.logOutUser}>
                                <i className="fa fa-sign-out"></i> Sair
                            </a>
                        </li>
                    </ul>
                </nav>
            </div>
        )
    }
}

const mapStateToProps = state => ({ usuario: state.usuario })
const mapDispatchToProps = dispatch => bindActionCreators({ logOutUser }, dispatch)

export default connect(mapStateToProps, mapDispatchToProps)(TopHeader)
