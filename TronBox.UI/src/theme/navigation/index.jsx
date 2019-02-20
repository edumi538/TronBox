import React, { Component } from 'react'
import { connect } from 'react-redux'

import Navigation from './navigation'

class Index extends Component {
    render() {
        return (
            <Navigation />
        )
    }
}

const mapStateToProps = state => ({ rota: state.rota })

export default connect(mapStateToProps, null)(Index)
