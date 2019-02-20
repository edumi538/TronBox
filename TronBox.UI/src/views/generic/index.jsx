import React, { Component } from 'react';

import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';

import PageHeader from '../../theme/header';
import PageContent from '../../theme/content';

import { getGeneric } from './genericActions';


class Index extends Component {

    render() {
        return (
            <div>
                <PageHeader title="Generic">
                    <a href="#/home" className="btn btn-primary btn-sm">Voltar</a>
                </PageHeader>
                <PageContent title="Dados do Generic">
                    <div>
                        <table className="table">
                            <thead>
                                <tr>
                                    <th>Razão Social/Nome</th>
                                    <th>Inscrição</th>
                                    <th>Ações</th>
                                </tr>
                            </thead>
                            <tbody>
                               <h3>table generic</h3>
                            </tbody>
                        </table>
                    </div>
                </PageContent>
            </div>
        )
    }
}

const mapStateToProps = state => ({ genericReducer: state.generic });
const mapDispatchToProps = dispatch => bindActionCreators({ getGeneric }, dispatch);

export default connect(mapStateToProps, mapDispatchToProps)(Index);
