import React, { Component } from 'react'

export default class HeaderGrid extends Component {
    render() {
        return (
            <div className="ibox-title">
                <h5>{this.props.title}</h5>
                <div className="ibox-tools">
                    <a className="collapse-link">
                        <i className="fa fa-chevron-up"></i>
                    </a>                                     
                </div>
            </div>
        )
    }
}
