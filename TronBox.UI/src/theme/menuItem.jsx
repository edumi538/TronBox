import React from 'react'

export default props => (
    <li>
        <a href={props.path}>
            {props.tree ?
                props.icon &&
                <i className={`fa fa-${props.icon}`} style={{ marginLeft: -23 }}></i>
                :
                props.icon &&
                <i className={`fa fa-${props.icon}`}></i>
            }
            {props.tree ?
                props.label
                :
                <span className="nav-label">{props.label}</span>
            }
        </a>
    </li>
)