import React, { Component } from 'react'
import Messages from './messages'
import Progress from './progress'
import Navigation from './navigation/index'
import TopHeader from './topHeader'
import Footer from './footer'
import { correctHeight, detectBody } from './helpers/helpers'
import { connect } from 'react-redux'

import './assets/dependencies'

class Main extends Component {

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
    }
}

const mapStateToProps = state => ({ });

export default connect(mapStateToProps)(Main)
