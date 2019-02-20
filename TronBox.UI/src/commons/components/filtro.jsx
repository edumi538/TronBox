import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Field, reduxForm } from 'redux-form'

import { required } from '../../helpers/validations'

import { getContribuintes } from '../../actions/contribuinteIndividualActions'
import { getEmpregados } from '../../actions/empregadoActions'
import { setEmpregado } from '../../actions/filtroActions'

import { AppSelect, AppSelectWithCode } from '../../theme/inputs'
import { FormDateRangePicker, FormDateMonthPicker } from '../../theme/formInputs'

import HeaderGrid from '../../theme/headerGrid'

export class Filtro extends Component {

    constructor(props) {
        super(props);
    }

    componentDidMount() {
        if (this.props.renderEmpregado) {
            this.props.getEmpregados()

            if (this.props.inscricao)
                this.props.setEmpregado(this.props.inscricao)
        }

        if (this.props.renderContribuinte) {
            this.props.getContribuintes()
        }
    }

    renderEmpregado() {
        if (this.props.renderEmpregado) {
            return (
                <div className="row">
                    <Field
                        name="inscricao"
                        label="Empregado:"
                        component={AppSelectWithCode}
                        options={this.props.empregado.empregados}
                        col={12}
                        valueKey="inscricao"
                        labelKey="nome"
                        labelKey1="matricula"
                        disabled={!!this.props.inscricao}
                        validate={required}
                        required={required} />
                </div>
            )
        }
    }

    renderContribuinte() {
        if (this.props.renderContribuinte) {
            return (
                <div className="row">
                    <Field
                        name="inscricao"
                        label="Contribuinte Individual:"
                        component={AppSelect}
                        options={this.props.contribuinteIndividual.contribuintes}
                        col={12}
                        valueKey="inscricao"
                        labelKey="nome"
                        validate={required}
                        required={required} />
                </div>
            )
        }
    }

    renderDatas() {
        if (this.props.renderDatas) {
            if (this.props.monthPicker) {
                return (
                    <div className="row">
                        <Field
                            name="mesInicial"
                            label="Mês Inicial:"
                            component={FormDateMonthPicker}
                            col={6}
                            validate={required}
                            required={required} />

                        <Field
                            name="mesFinal"
                            label="Mês Final:"
                            component={FormDateMonthPicker}
                            col={6}
                            validate={required}
                            required={required} />
                    </div>
                )
            } else {
                return (
                    <div className="row">
                        <Field
                            name="rangeDatas"
                            label="Período:"
                            component={FormDateRangePicker}
                            col={12}
                            validate={required}
                            required={required} />
                    </div>
                )
            }
        }
    }

    render() {
        const { handleSubmit } = this.props

        return (
            <form onSubmit={handleSubmit}>
                <div className="ibox float-e-margins">
                    <HeaderGrid title="Filtro de Pesquisa" />

                    <div className="ibox-content">
                        {this.renderEmpregado()}

                        {this.renderContribuinte()}

                        {this.renderDatas()}

                        <div className="row">
                            <div className="col-md-12">
                                <button type="submit" className="btn btn-primary pull-right">
                                    <i className="fa fa-search"></i> Buscar Dados
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </form>
        )
    }
}

Filtro = reduxForm({ form: 'filtroForm', destroyOnUnmount: true })(Filtro);

const mapStateToProps = (state) => ({
    empregado: state.empregado,
    contribuinteIndividual: state.contribuinteIndividual
});

const mapDispatchToProps = {
    getContribuintes,
    getEmpregados,
    setEmpregado
};

export default connect(mapStateToProps, mapDispatchToProps)(Filtro);