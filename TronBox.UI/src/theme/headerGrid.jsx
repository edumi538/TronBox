import React, { Component } from 'react'

import { Row, Col, Popover, Icon } from 'antd'

export default class HeaderGrid extends Component {

    renderContentPopover() {
        return (
            <div>
                <p>{this.props.popover}</p>
            </div>
        )
    }

    renderPopover() {
        if (this.props.popover) {
            return (
                <Col span={12}>
                    <div className="pull-right">
                        <Popover placement="rightTop" content={this.renderContentPopover()} title="Saiba mais">
                            <Icon type="question-circle-o" style={{ fontSize: 26, marginRight: 16 }} />
                        </Popover>
                    </div>
                </Col>
            )
        }
    }

    render() {
        return (
            <div className="ibox-title">
                <Row>
                    <Col span={12}>
                        <h5>{this.props.title}</h5>
                    </Col>

                    {this.renderPopover()}
                </Row>
            </div>
        )
    }
}
