import React, { Component } from 'react'

export default class Content extends Component {
    render(){
        return (
            <div className="wrapper wrapper-content animated fadeIn">
            { this.props.title && 
                <div className="ibox-title">
                    <h5>{this.props.title}</h5>
                </div>
            }
                <div className="row">
                    <div className="col-md-4">
                        <div className="ibox">
                            <div className="ibox-content">
                                {this.props.children}
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        )
    }
}