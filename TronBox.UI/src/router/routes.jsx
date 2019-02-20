import React from 'react'
import { Router, Route, IndexRoute, Redirect } from 'react-router'
import { toastr } from 'react-redux-toastr'

import _ from 'lodash'

import Main from '../theme/main'


export default (
    <Router path="/" component={Main}>
        

        <Redirect from='*' to='/' />
    </Router>
)
