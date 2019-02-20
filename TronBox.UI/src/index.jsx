import React from 'react'
import ReactDOM from 'react-dom'
import { Router, hashHistory } from 'react-router'
import HttpsRedirect from 'react-https-redirect'

import { applyMiddleware, createStore } from 'redux'
import { Provider } from 'react-redux'

import { LocaleProvider } from 'antd'
import ptBR from 'antd/lib/locale-provider/pt_BR'

import promise from 'redux-promise'
import multi from 'redux-multi'
import thunk from 'redux-thunk'

import reducers from './reducers/reducers'
import routes from './router/routes'

const middlewares = [
    multi,
    promise,
    thunk
]

const devTools = window.__REDUX_DEVTOOLS_EXTENSION__ && window.__REDUX_DEVTOOLS_EXTENSION__()
const store = applyMiddleware(...middlewares)(createStore)(reducers, devTools)

ReactDOM.render(
    <LocaleProvider locale={ptBR}>
        <Provider store={store}>
            {/* <HttpsRedirect> */}
                <Router history={hashHistory} routes={routes} />
            {/* </HttpsRedirect> */}
        </Provider>
    </LocaleProvider>,
    document.getElementById('app'))
