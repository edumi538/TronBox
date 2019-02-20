import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';

import PageHeader from '../../theme/header';
import PageContent from '../../theme/content';

import Form from './genericForm';

import { postGeneric} from './genericActions';

import { hashHistory } from 'react-router';
import { toastr } from 'react-redux-toastr';

class Editar extends Component {

    componentWillMount() {
        //--Metodo para listar os dados
    }

    render() {
        return (
            <div>
                <PageHeader title="Generic">
                    <a href="#/escritorio" className="btn btn-primary btn-sm">Voltar</a>
                </PageHeader>
                <PageContent title="Adicionar Generic">
                    <Form onSubmit={this.props.postGeneric} />
                </PageContent>
            </div>
        )
    }
}

const mapStateToProps = state => ({ genericReducer: state.generic });
const mapDispatchToProps = dispatch => bindActionCreators({ postGeneric }, dispatch);
export default connect(mapStateToProps, mapDispatchToProps)(Editar);
