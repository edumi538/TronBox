import React, { Component } from 'react'

import logo from './assets/images/footer/logo-tron.png';

export default class Footer extends Component {
    render() {
        return (
            <div className="footer">
                <div className="pull-right">
                    <a target="_blank" href="http://www.tron.com.br">
                        <img alt="image" src={logo} />
                    </a>
                </div>
                <div>
                    <label>Copyright</label> Tron Informática &copy; {new Date().getFullYear()}
                </div>
            </div>
        )
    }
}
