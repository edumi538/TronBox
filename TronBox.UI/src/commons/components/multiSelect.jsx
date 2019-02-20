import React, { Component } from 'react'

import { MultiSelect } from '@progress/kendo-react-dropdowns';

const sports = [ "Baseball", "Basketball", "Cricket", "Field Hockey", "Football", "Table Tennis", "Tennis", "Volleyball" ];

export default class MultiSelectTelerik extends Component {

    constructor(props) {
        super(props);

        this.state = {
            value: []
        }
        
    }

    render() {
        const onChange = event =>{
            this.setState({
                value: [ ...event.target.value ]
            })
        }
        return (
            <div>
                <div>Favorite sports:</div>
                <MultiSelect
                    data={sports}
                    onChange={onChange}
                    value={this.state.value}
                />
            </div>
        )
    }
}

