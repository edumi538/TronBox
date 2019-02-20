import React, { Component } from 'react'
import { Tooltip, Icon } from 'antd'
import Moment from 'moment'

import simpleNumberLocalizer from 'react-widgets-simple-number'
import momentLocalizer from 'react-widgets-moment'
import DropdownList from 'react-widgets/lib/DropdownList'
import DateTimePicker from 'react-widgets/lib/DateTimePicker'
import NumberPicker from 'react-widgets/lib/NumberPicker'

import InputMask from 'react-input-mask'
import Dropzone from 'react-dropzone'

import Select from 'react-select'
import 'react-select/dist/react-select.css'

//css react-widgets
import 'react-widgets/dist/css/react-widgets.css'

Moment.locale('pt-BR')

momentLocalizer();
simpleNumberLocalizer()

const tooltipColor = 'rgb(201, 195, 195)'

const errorMessage = (visited, value, touched, warning, error) => {
    if (visited) {
        return (
            <small className="help-block" style={{ display: (visited && (value.indexOf('_') === -1)) ? 'initial' : 'none' }} >
                <span className="text-danger">{error}</span>
            </small>
        );
    }
    return (
        <small className="help-block">
            {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))}
        </small>
    );

};

const AppDropDownList = ({ input, data, valueField, textField, meta: { touched, error } }) => (
    <div>
        <DropdownList filter {...input} data={data} valueField={valueField} textField={textField} onChange={input.onChange} />
        <small className="help-block">
            {touched && (error && <span className="text-danger">{error}</span>)}
        </small>
    </div>
);

const AppSelect = ({ input: { onChange, value }, tooltip, label, col, required, clear, valueKey, labelKey, options, disabled, valueRenderer, valueComponent, meta: { touched, error, visited, warning } }) => (
    <div className={`col-md-${col}`}>
        <div className="form-group">
            <label className="control-label">
                {label}
                <span style={{ color: 'transparent' }}>.</span>
                {required &&
                    <span className="text-danger"> * </span>
                }
                {
                    tooltip &&
                    <Tooltip title={tooltip}>
                        <Icon type="question-circle-o" style={{ color: tooltipColor }} />
                    </Tooltip>
                }
            </label>
            <Select
                value={!value ? null : value} menuContainerStyle={{ zIndex: 1000 }} openOnFocus
                valueRenderer={valueRenderer} valueComponent={valueComponent} searchable simpleValue
                onChange={onChange} options={options} disabled={disabled} valueKey={valueKey} labelKey={labelKey}
                loadingPlaceholder="Carregando..." placeholder="" noResultsText="Nenhum registro" searchPromptText="Digite para pesquisar"
            />
            {errorMessage(visited, value, touched, warning, error)}
        </div>
    </div>
);

const AppSelectWithCode = ({ input: { onChange, value }, tooltip, label, col, required, clear, valueKey, labelKey, labelKey1, options, disabled, valueRenderer, valueComponent, meta: { touched, error, visited, warning } }) => (
    <div className={`col-md-${col}`}>
        <div className="form-group">
            <label className="control-label">
                {label}
                {required &&
                    <span className="text-danger"> * </span>
                }
                {
                    tooltip &&
                    <Tooltip title={tooltip}>
                        <Icon type="question-circle-o" style={{ color: tooltipColor }} />
                    </Tooltip>
                }
            </label>

            <div className="row">
                <div className={'col-sm-2'}>
                    <Select
                        value={!value ? null : value} menuContainerStyle={{ zIndex: 1000 }} openOnFocus
                        valueRenderer={valueRenderer} valueComponent={valueComponent} searchable simpleValue
                        onChange={onChange} options={options} disabled={disabled} valueKey={valueKey} labelKey={labelKey1 ? labelKey1 : valueKey}
                        loadingPlaceholder="Carregando..." placeholder="" noResultsText="Nenhum registro" searchPromptText="Digite para pesquisar"
                    />
                </div>
                <div className={`col-sm-10`}>
                    <Select
                        value={!value ? null : value} menuContainerStyle={{ zIndex: 1000 }} openOnFocus
                        valueRenderer={valueRenderer} valueComponent={valueComponent} searchable simpleValue
                        onChange={onChange} options={options} disabled={disabled} valueKey={valueKey} labelKey={labelKey}
                        loadingPlaceholder="Carregando..." placeholder="" noResultsText="Nenhum registro" searchPromptText="Digite para pesquisar"
                    />
                </div>
            </div>
            <div className="row">
                <div className={`col-sm-12`}>
                    {errorMessage(visited, value, touched, warning, error)}
                </div>
            </div>
        </div>
    </div>
);

const AppSelectReadonly = ({ input: { onChange, value }, valueKey, labelKey, options, disabled, valueRenderer, valueComponent, meta: { touched, error, visited, warning } }) => (
    <div>
        <Select
            value={!value ? null : value} disabled="disabled" menuContainerStyle={{ zIndex: 1000 }} openOnFocus valueRenderer={valueRenderer} valueComponent={valueComponent} searchable simpleValue onChange={onChange} options={options} disabled={disabled} valueKey={valueKey} labelKey={labelKey}
            loadingPlaceholder="Carregando..."
            placeholder="" noResultsText="Nenhum registro" searchPromptText="Digite para pesquisar"
        />
        {errorMessage(visited, value, touched, warning, error)}
    </div>
);

const AppSelectPagination = ({ input: { onChange, value }, valueKey, labelKey, loadOptions, meta: { touched, error, pristine } }) => (
    <div>
        <Select.Async
            value={!value ? null : value} openOnFocus searchable onChange={onChange} valueKey={valueKey} labelKey={labelKey}
            loadingPlaceholder="Carregando..." loadOptions={loadOptions} pagination
            placeholder="Selecione ou digite..." noResultsText="Nenhum registro" searchPromptText="Digite para pesquisar"
        />
        <small className="help-block">
            {touched && pristine && (error && <span className="text-danger">{error}</span>)}
        </small>
    </div>
);

const AppSelectWithIcon = ({ input: { onChange, value }, tooltip, label, col, required, icon, onClick, valueKey, labelKey, options, disabled, valueRenderer, valueComponent, meta: { touched, error, visited, warning } }) => (
    <div className={`col-md-${col}`}>
        <div className="form-group">
            <label className="control-label">
                {label}
                <span style={{ color: 'transparent' }}>.</span>
                {required &&
                    <span className="text-danger"> * </span>
                }
                {
                    tooltip &&
                    <Tooltip title={tooltip}>
                        <Icon type="question-circle-o" style={{ color: tooltipColor }} />
                    </Tooltip>
                }
            </label>
            <div className="input-group">
                <Select
                    value={!value ? null : value} menuContainerStyle={{ zIndex: 1000 }} openOnFocus
                    valueRenderer={valueRenderer} valueComponent={valueComponent} searchable simpleValue
                    onChange={onChange} options={options} disabled={disabled} valueKey={valueKey} labelKey={labelKey}
                    loadingPlaceholder="Carregando..." placeholder="" noResultsText="Nenhum registro" searchPromptText="Digite para pesquisar"
                />
                <span className="input-group-btn">
                    <button type="button" onClick={onClick} className="btn btn-primary">
                        <i className={`fa fa-${icon}`} />
                    </button>
                </span>
            </div>
            {errorMessage(visited, value, touched, warning, error)}
        </div>
    </div>
);

const AppInputMask = ({ input: { onChange, value, onFocus }, mask, disabled, meta: { touched, error, visited, dirty, pristine, warning } }) => (
    <div>
        <InputMask value={!value ? '' : value} disabled={disabled} onFocus={onFocus} onChange={onChange} type="text" mask={mask} className="form-control" />
        {errorMessage(visited, value, touched, warning, error)}
    </div>
);

const AppInput = ({ disabled, input, maxLength, meta: { touched, error, warning } }) => (
    <div>
        <input {...input} value={!input.value ? '' : input.value} disabled={disabled} onChange={input.onChange} onBlur={input.onBlur} maxLength={maxLength} type="text" className="form-control" />
        <small className="help-block">
            {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))}
        </small>
    </div>
);

const AppTextArea = ({ input, maxLength, meta: { touched, error, warning } }) => (
    <div>
        <textarea {...input} value={!input.value ? '' : input.value} onChange={input.onChange} onBlur={input.onBlur} maxLength={maxLength} type="text" className="form-control" rows="6" />
        <small className="help-block">
            {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))}
        </small>
    </div>
);

const AppInputButton = ({ input: { onChange, value, onFocus }, onKeyUp, onClick, col, mask, label, required, icon, disabled, tooltip, meta: { touched, error, visited, warning } }) => (
    <div className={`col-md-${col}`}>
        <div className="form-group">
            <label className="control-label">
                {label}
                {required &&
                    <span className="text-danger">*</span>
                }
                {' '}
                {
                    tooltip &&
                    <Tooltip title={tooltip}>
                        <Icon type="question-circle-o" style={{ color: tooltipColor }} />
                    </Tooltip>
                }
            </label>
            <div className="input-group">
                <InputMask value={!value ? '' : value} onChange={onChange} onFocus={onFocus} onKeyUp={onKeyUp} type="text" mask={mask} className="form-control" disabled={disabled} />
                <span className="input-group-btn">
                    <button type="button" onClick={onClick} className="btn btn-primary">
                        <i className={`fa fa-${icon}`} />
                    </button>
                </span>
            </div>
            {errorMessage(visited, value, touched, warning, error)}
        </div>
    </div>
);

const AppInputFile = ({ input, meta: { touched, error, warning } }) => (
    <div>
        <input {...input} onChange={input.onChange} type="file" className="form-control" />
        <small className="help-block">
            {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))}
        </small>
    </div>
);


const AppInputPassword = ({ input: { onChange, value }, meta: { touched, error } }) => (
    <div>
        <input value={!value ? '' : value} onChange={onChange} type="password" className="form-control" />
        <small className="help-block">
            {touched && (error && <span className="text-danger">{error}</span>)}
        </small>
    </div>
);

const AppInputReadOnly = ({ input, meta: { touched, error } }) => (
    <div>
        <input {...input} value={input.value ? input.value : ''} onChange={input.onChange} type="text" className="form-control" readOnly />
        <small className="help-block">
            {touched && (error && <span className="text-danger">{error}</span>)}
        </small>
    </div>
);

const AppNumber = ({ input, max, min, meta: { touched, error } }) => (
    <div>
        <NumberPicker {...input} onChange={input.onChange} value={!input.value ? null : Number(input.value)} max={max} min={min} />
        <small className="help-block">
            {touched && (error && <span className="text-danger">{error}</span>)}
        </small>
    </div>
);

const AppInputTelefoneFixo = ({ input, disabled, meta: { touched, error } }) => (
    <div>
        <InputMask {...input} type="text" mask="(99) 9999-9999" disabled={disabled} className="form-control" />
        <small className="help-block">
            {touched && (error && <span className="text-danger">{error}</span>)}
        </small>
    </div>
);

const AppInputTelephone = ({ input, meta: { touched, error } }) => (
    <div>
        <InputMask {...input} type="text" mask="(99) 9999-99999" className="form-control" />
        <small className="help-block">
            {touched && (error && <span className="text-danger">{error}</span>)}
        </small>
    </div>
);

const AppInputCelular = ({ input, disabled, meta: { touched, error } }) => (
    <div>
        <InputMask {...input} type="text" mask="(99) 99999-9999" className="form-control" disabled={disabled} />
        <small className="help-block">
            {touched && (error && <span className="text-danger">{error}</span>)}
        </small>
    </div>
);

const AppInputHour = ({ input, meta: { touched, error } }) => (
    <div>
        <InputMask {...input} type="text" mask="99:99" className="form-control" />
        <small className="help-block">
            {touched && (error && <span className="text-danger">{error}</span>)}
        </small>
    </div>
);

const AppDatePicker = ({ input, disabled, meta: { touched, error } }) => (
    <div>
        <DateTimePicker {...input} disabled={disabled} format="DD/MM/YYYY" time={false} onChange={input.onChange} onBlur={() => input.onBlur(input.value)} value={!input.value ? null : new Date(input.value)} />
        <small className="help-block">
            {touched && (error && <span className="text-danger">{error}</span>)}
        </small>
    </div>
);

const AppTimePicker = ({ input, meta: { touched, error } }) => (
    <div>
        <DateTimePicker {...input} format="HH:mm" date={false} onChange={input.onChange} onBlur={() => input.onBlur(input.value)} value={!input.value ? null : input.value} />
        <small className="help-block">
            {touched && (error && <span className="text-danger">{error}</span>)}
        </small>
    </div>
);

export {
    AppDropDownList,
    AppSelect,
    AppSelectReadonly,
    AppSelectPagination,
    AppSelectWithCode,
    AppSelectWithIcon,
    AppInput,
    AppInputFile,
    AppInputButton,
    AppInputPassword,
    AppInputMask,
    AppInputReadOnly,
    AppInputTelephone,
    AppInputCelular,
    AppInputHour,
    AppDatePicker,
    AppTimePicker,
    AppInputTelefoneFixo,
    AppNumber,
    AppTextArea
};
