import React, { Component } from 'react'
import InputMask from 'react-input-mask'
import { Tooltip, Icon, DatePicker, Select, Input, InputNumber, Checkbox, TimePicker } from 'antd'
const Option = Select.Option;
const { TextArea } = Input;

import moment from 'moment'

moment.locale('pt-br')
var tooltipColor = 'rgb(201, 195, 195)'

const FormInput = ({ input, label, tooltip, col, required, maxLength, disabled, meta: { touched, error, warning }}) => (
    <div className="form-group">
            <Input {...input} maxLength={maxLength} disabled={disabled} style={{ width: '100%' }} />
            <small className="help-block">
                {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))} <span style={{ color: 'transparent' }}>.</span>
            </small>
    </div>
)

const FormInputButton = ({ input: { onChange, value, onFocus }, onKeyUp, onClick, col, mask, label, required, icon, disabled, tooltip, meta: { touched, error, warning } }) => (
    <div className="form-group">
        <div className="input-group">
            <InputMask value={!value ? '' : value} onChange={onChange} onFocus={onFocus} onKeyUp={onKeyUp} type="text" mask={mask} className="form-control" disabled={disabled} />
            <span className="input-group-btn">
                <button type="button" onClick={onClick} className="btn btn-primary">
                    <i className={`fa fa-${icon}`} />
                </button>
            </span>
        </div>
        {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))} <span style={{ color: 'transparent' }}>.</span>
    </div>
)

const FormInputTextArea = ({ input, label, tooltip, col, required, maxLength, disabled, minRows, maxRows, meta: { touched, error, warning }}) => (
    <div className="form-group">
        <TextArea {...input} maxLength={maxLength} disabled={disabled} style={{ width: '100%' }} autosize ={{minRows, maxRows}}/>
        <small className="help-block">
            {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))} <span style={{ color: 'transparent' }}>.</span>
        </small>
    </div>
)

const FormSelect = ({ input, label, tooltip, col, required, disabled, values, meta: { touched, error, warning }}) => (
    <div className="form-group">
            <Select {...input} style={{ width: '100%' }} disabled={disabled}>
                { values.map(d => <Option key={d.id}>{d.descricao}</Option>)}
            </Select>
            <small className="help-block">
                {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))} <span style={{ color: 'transparent' }}>.</span>
            </small>
    </div>
)

const children = [];
for (let i = 10; i < 36; i++) {
  children.push(<Option key={i.toString(36) + i}>{i.toString(36) + i}</Option>);
}

const FormSelectTags = ({ input, label, tooltip, col, required, values, meta: { touched, error, warning }}) => (
        <div className="form-group">
            <Select {...input} mode="tags" style={{ width: '100%' }} tokenSeparators = {[',']}>
                { children }
                {/* { values.map(d => <Option key={d.id}>{d.descricao}</Option>)} */}
            </Select>
            <small className="help-block">
                {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))} <span style={{ color: 'transparent' }}>.</span>
            </small>
        </div>
    )


const FormSelectChild = ({ input, label, tooltip, col, required, children, clear, meta: { touched, error, warning }}) => (
    <div className="form-group">
        <Select allowClear={clear} {...input} showSearch filterOption={(input, option) => option.props.children.toLowerCase().indexOf(input.toLowerCase()) >= 0} style={{ width: '100%' }} >
            {children}
        </Select>
        <small className="help-block">
            {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))} <span style={{ color: 'transparent' }}>.</span>
        </small>
    </div>
)

const FormDatePicker = ({ input, label, tooltip, col, required, meta: { touched, error, warning } }) => (
    <div className="form-group">
        <DatePicker {...input} value={input.value ? moment(input.value, 'DD/MM/YYYY') : null} format="DD/MM/YYYY" style={{ width: '100%' }} />
        <small className="help-block">
            {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))} <span style={{ color: 'transparent' }}>.</span>
        </small>
    </div>
)

const FormInputNumber = ({ input, label, tooltip, col, required, min, max, meta: { touched, error, warning } }) => (
    <div className="form-group">
        <InputNumber {...input} min={min} max={max} style={{ width: '100%' }} />
        <small className="help-block">
            {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))} <span style={{ color: 'transparent' }}>.</span>
        </small>
    </div>
)

const FormInputCheckbox = ({ input, label, tooltip, col, required, meta: { touched, error, warning } }) => (
    <div className="form-group">
        <Checkbox {...input} />
        <small className="help-block">
            {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))} <span style={{ color: 'transparent' }}>.</span>
        </small>
    </div>
)

const FormTimePicker = ({ input, label, tooltip, col, required, meta: { touched, error, warning } }) => (
    <div className="form-group">
        <TimePicker {...input} value={input.value ? input.value : null} format="HH:mm" />
        <small className="help-block">
            {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))} <span style={{ color: 'transparent' }}>.</span>
        </small>
    </div>
)

const FormInputMask = ({ input, label, tooltip, col, required, mask, meta: { touched, error, warning } }) => (
    <div className="form-group">
        <InputMask value={input.value ? input.value : ''} onChange={input.onChange} type="text" mask={mask} className="form-control" />
        <small className="help-block">
            {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))} <span style={{ color: 'transparent' }}>.</span>
        </small>
    </div>
)

export {
    FormInput,
    FormInputButton,
    FormSelect,
    FormSelectTags,
    FormSelectChild,
    FormDatePicker,
    FormInputNumber,
    FormInputCheckbox,
    FormTimePicker,
    FormInputMask,
    FormInputTextArea
}
