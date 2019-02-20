import React, { Component } from 'react'
import InputMask from 'react-input-mask'
import { Tooltip, Icon, DatePicker, Select, Input, InputNumber, Checkbox, TimePicker, Radio } from 'antd'
const Option = Select.Option;
const { TextArea } = Input;

import NumberFormat from 'react-number-format';

import moment from 'moment'

moment.locale('pt-br')
var tooltipColor = 'rgb(201, 195, 195)'

const RadioGroup = Radio.Group

const FormInput = ({ input, label, tooltip, note, col, required, maxLength, disabled, meta: { touched, error, warning } }) => (
    <div className={`col-md-${col}`}>
        <div className="form-group">
            <label className="control-label">
                {label}
                <span style={{ color: 'transparent' }}>.</span>
                {required &&
                    <span className="text-danger"> * </span>
                }
                {note &&
                    <label style={{ fontWeight: 'normal' }}>({note})</label>
                }
                {
                    tooltip &&
                    <Tooltip title={tooltip}>
                        <Icon type="question-circle-o" style={{ color: tooltipColor }} />
                    </Tooltip>
                }
            </label>
            <Input {...input} maxLength={maxLength} disabled={disabled} style={{ width: '100%' }} />
            <small className="help-block">
                {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))} <span style={{ color: 'transparent' }}>.</span>
            </small>
        </div>
    </div>
)

const FormInputButton = ({ input: { onChange, value, onFocus }, onKeyUp, onClick, col, mask, label, required, icon, disabled, tooltip, meta: { touched, error, warning } }) => (
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
    </div>
)

const FormInputTextArea = ({ input, label, tooltip, col, required, maxLength, disabled, minRows, maxRows, meta: { touched, error, warning } }) => (
    <div className={`col-md-${col}`}>
        <div className="form-group">
            <label className="control-label">
                {label}
                <span style={{ color: 'transparent' }}></span>
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
            <TextArea {...input} maxLength={maxLength} disabled={disabled} style={{ width: '100%' }} autosize={{ minRows, maxRows }} />
            <small className="help-block">
                {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))} <span style={{ color: 'transparent' }}>.</span>
            </small>
        </div>
    </div>
)

const FormSelect = ({ input, label, tooltip, clear, col, required, disabled, values, meta: { touched, error, warning } }) => (
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
            <Select {...input} allowClear={clear} style={{ width: '100%' }} disabled={disabled}>
                {values.map(d => <Option key={d.id}>{d.descricao}</Option>)}
            </Select>
            <small className="help-block">
                {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))} <span style={{ color: 'transparent' }}>.</span>
            </small>
        </div>
    </div>
)

const children = [];
for (let i = 10; i < 36; i++) {
    children.push(<Option key={i.toString(36) + i}>{i.toString(36) + i}</Option>);
}

const FormSelectTags = ({ input, label, tooltip, col, required, values, meta: { touched, error, warning } }) => (
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
            <Select {...input} mode="tags" style={{ width: '100%' }} tokenSeparators={[',']}>
                {children}
                {/* { values.map(d => <Option key={d.id}>{d.descricao}</Option>)} */}
            </Select>
            <small className="help-block">
                {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))} <span style={{ color: 'transparent' }}>.</span>
            </small>
        </div>
    </div>
)


const FormSelectChild = ({ input, label, tooltip, col, required, children, clear, meta: { touched, error, warning } }) => (
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
            <Select allowClear={clear} {...input} showSearch
                filterOption={(input, option) => option.props.children.toLowerCase().indexOf(input.toLowerCase()) >= 0} style={{ width: '100%' }} >
                {children}
            </Select>
            <small className="help-block">
                {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))} <span style={{ color: 'transparent' }}>.</span>
            </small>
        </div>
    </div>
)

const FormDatePicker = ({ input, desabilitaDatasAnteriores, formatoData, label, tooltip, col, required, disabled, meta: { touched, error, warning } }) => (
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
            <DatePicker {...input} value={input.value ? moment(input.value, formatoData ? formatoData : 'DD/MM/YYYY') : null} disabled={disabled} format="DD/MM/YYYY" style={{ width: '100%' }} disabledDate={desabilitaDatasAnteriores ? disabledDate : undefined} />
            <small className="help-block">
                {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))} <span style={{ color: 'transparent' }}>.</span>
            </small>
        </div>
    </div>
)

const FormInputNumber = ({ input, label, label1, label2, tooltip, col, required, min, max, meta: { touched, error, warning } }) => (
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
            <label>{label1}</label><InputNumber {...input} min={min} max={max} style={{ width: '100%' }} /><label>{label2}</label>
            <small className="help-block">
                {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))} <span style={{ color: 'transparent' }}>.</span>
            </small>
        </div>
    </div>
)

const FormInputNumberDecimal = ({ input, label, label1, label2, disabled, fixedDecimalScale, tooltip, col, allowNegative, maxValue, minValue, required, prefix, suffix, precision, width, meta: { touched, error, warning } }) => (
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
            <label>{label1}</label>
            <div>
                <NumberFormat

                    style={{
                        borderBottomLeftRadius: 4,
                        borderTopLeftRadius: 4,
                        borderBottomRightRadius: 4,
                        borderTopRightRadius: 4,
                        width: '100%',
                        height: 38,
                        borderColor: '#c9d3dd',
                        borderWidth: 1,
                        fontSize: 13,
                        lineHeight: 1.5,
                        outlineColor: '#18a689',
                        paddingTop: 4,
                        paddingBottom: 4,
                        paddingLeft: 11,
                        paddingRight: 11,
                        backgroundColor: '#fff',
                        backgroundImage: 'none',
                        color: '#676a6c',
                        position: 'relative',
                        display: 'inline-block',
                        transitionProperty: 'transform, opacity',
                        transitionDuration: '0.15s',
                        transitionTimingFunction: 'ease, step-end'
                    }}

                    allowNegative={allowNegative}
                    suffix={suffix}
                    prefix={prefix}
                    thousandSeparator="."
                    decimalSeparator=','
                    disabled={disabled}

                    isAllowed={(maxValue || minValue) || (minValue && maxValue)  ?
                        (values) => {
                            const { formattedValue, floatValue } = values;
                            if(minValue && maxValue)
                                return formattedValue === '' || (floatValue <= maxValue && floatValue >= minValue);
                            else if(minValue)
                                return formattedValue === '' || floatValue >= minValue;
                            else
                                return formattedValue === '' || floatValue <= maxValue;
                        } :
                        undefined
                    }

                    decimalScale={precision}
                    fixedDecimalScale={fixedDecimalScale}
                    value={input.value ? input.value : ''}
                    onValueChange={(values, e) => input.onChange(values.floatValue === undefined ? 0 : values.floatValue)}
                />
            </div>
            <label>{label2}</label>
            <small className="help-block">
                {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))} <span style={{ color: 'transparent' }}>.</span>
            </small>
        </div>
    </div>
)

const MultiSelectTelerik = ({ input, data, placeholder, children, label, minRows, label1, label2, col, tooltipColor, tooltip, required, meta: { touched, error, warning } }) => {
    return (
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
                <label>{label1}</label>

                <Select
                    {...input}
                    allowClear={true}
                    //children={children}
                    mode="multiple"
                    style={{ width: '100%' }}
                    labelInValue={true}
                    value={data}
                    placeholder={placeholder}

                    // defaultValue={{
                    //     key: input.id,
                    //     label: input.descricao
                    // }}

                    //value={!data ? [] : data}
                    //{...input.value}
                    //value={input.value}
                    //onChange={input.onChange}
                    showSearch
                    filterOption={(input, option) => option.props.children.toLowerCase().indexOf(input.toLowerCase()) >= 0} style={{ width: '100%' }}
                >
                    {children}
                </Select>

                <label>{label2}</label>
                <small className="help-block">
                    {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))} <span style={{ color: 'transparent' }}>.</span>
                </small>
            </div>
        </div>
    )
}

const FormInputCheckbox = ({ input, label, checked, tooltip, children, col, required, meta: { touched, error, warning } }) => (
    <div className={`col-md-${col}`}>
        <div className="form-group">
            <Checkbox {...input} checked={checked} /> <label style={{ marginLeft: 5 }}>{label} {required && <span className="text-danger"> * </span> }</label>
            {children &&
                children
            }
            {
                tooltip &&
                <Tooltip title={tooltip}>
                    <Icon type="question-circle-o" style={{ color: tooltipColor, marginLeft: 5 }} />
                </Tooltip>
            }
            <small className="help-block">
                {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))} <span style={{ color: 'transparent' }}>.</span>
            </small>
        </div>
    </div>
)

const FormRadioGroup = ({ input, label, label1, checked, tooltip, defaultValue, children, col, required, meta: { touched, error, warning } }) => (
    <div className={`col-md-${col}`}>
        <div className="form-group">
            <label className="control-label">
                {label}
                <span style={{ color: 'transparent' }}>.</span>
                {required &&
                    <span className="text-danger"> * </span>
                }
            </label>
            <div>
                <RadioGroup
                    //onChange={this.onChange}
                    //value={this.state.value}
                    {...input}
                    //value={1}
                >
                    {
                        children
                    }
                </RadioGroup>

            </div>

            {
                tooltip &&
                <Tooltip title={tooltip}>
                    <Icon type="question-circle-o" style={{ color: tooltipColor, marginLeft: 5 }} />
                </Tooltip>
            }
            <small className="help-block">
                {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))} <span style={{ color: 'transparent' }}>.</span>
            </small>
        </div>
    </div>
)

const FormTimePicker = ({ input, label, tooltip, col, required, meta: { touched, error, warning } }) => (
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
            <TimePicker {...input} value={input.value ? moment(input.value, 'HH:mm') : null} format="HH:mm" />
            <small className="help-block">
                {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))} <span style={{ color: 'transparent' }}>.</span>
            </small>
        </div>
    </div>
)

const FormInputMask = ({ input, label, tooltip, disabled, col, required, mask, meta: { touched, error, warning } }) => (
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
            <InputMask disabled={disabled} value={input.value ? input.value : ''} onChange={input.onChange} type="text" mask={mask} className="form-control" />
            <small className="help-block">
                {touched && ((error && <span className="text-danger">{error}</span>) || (warning && <span className="text-warning">{warning}</span>))} <span style={{ color: 'transparent' }}>.</span>
            </small>
        </div>
    </div>
)

//Functions

export function disabledDate(current) {
    return current && current < moment(moment(new Date()).utc().add('days', -1)).endOf('day');
}

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
    FormInputTextArea,
    FormInputNumberDecimal,
    MultiSelectTelerik,
    FormRadioGroup,
}
