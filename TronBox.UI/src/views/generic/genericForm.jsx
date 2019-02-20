import React, { Component } from 'react'
import { Field, reduxForm, change, formValueSelector } from 'redux-form'
import axios from 'axios'
import fetchJsonp from 'fetch-jsonp'
import { connect } from 'react-redux'
import { bindActionCreators } from 'redux'

import { getGeneric } from './genericActions';


class GenericForm extends Component {
    
    render() {
        const { handleSubmit, submitting } = this.props;

        return (
            <form onSubmit={handleSubmit}>
                <div className="panel panel-primary">
                    <div className="panel-heading">
                        <strong>Dados Generic</strong>
                    </div>
                    <div className="panel-body">
                        <h3>Panel Generic</h3>                            
                    </div>
                </div>
                <div className="row">
                    <div className="col-md-12">
                        <button type="submit" disabled={submitting} className="btn btn-primary pull-right">Salvar</button>
                    </div>
                </div>
            </form>
        )
    }
}

GenericForm = reduxForm({ form: 'genericForm', destroyOnUnmount: true })(GenericForm);

const mapStateToProps = state => ({
    genericReducer: state.generic
});


const mapDispatchToProps = dispatch => bindActionCreators({ getGeneric }, dispatch);

export default connect(mapStateToProps, mapDispatchToProps)(GenericForm);
