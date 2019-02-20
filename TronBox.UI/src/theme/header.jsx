import React, { Component } from 'react'

export default class Header extends Component {
    render(){
        return (
            <div className="row wrapper border-bottom white-bg page-heading">
                <div className="col-sm-8">
                    <h2>{this.props.title}</h2>
                    <ol className="breadcrumb">
                        <li>
                            <a href="#/home">Home</a>
                        </li>

                        {(() => {
                            if (this.props.father) {
                                return (
                                    <li>
                                        <a href={`#${this.props.fatherLink}`}>{this.props.father}</a>
                                    </li>
                                )
                            }
                        })()}

                        <li className="active">
                            <strong>{this.props.title}</strong>
                        </li>
                    </ol>
                </div>
                <div className="col-sm-4">
                    <div className="title-action">
                        {this.props.children}
                    </div>
                </div>
            </div>
        )
    }
}
