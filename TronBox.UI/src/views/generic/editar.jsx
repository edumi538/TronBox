import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';

import PageHeader from '../../theme/header';
import PageContent from '../../theme/content';

import Form from './genericForm';
import { putGeneric } from './genericActions';

class Editar extends Component {

    componentWillMount() {
        
    }

    render() {
        return (
            <div>
                <PageHeader title="Generic">
                    <a href="#/generic" className="btn btn-primary btn-sm">Voltar</a>
                </PageHeader>
                <PageContent title="Generic">
                    <Form onSubmit={this.props.putGeneric} />
                </PageContent>
            </div>
        )
    }
}

const mapDispatchToProps = dispatch => bindActionCreators({ putGeneric }, dispatch);
export default connect(null, mapDispatchToProps)(Editar);
