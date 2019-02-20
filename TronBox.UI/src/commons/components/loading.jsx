import React from 'react'
import ReactLoading from 'react-loading';
import { Section, Title, Article } from '../../util/generic'

export default props => (
    <div>
        <Section className="react-loading">
            <Title>{props.title}</Title>
            <Article>
                <ReactLoading type={props.type} />
            </Article>
        </Section>
    </div>
)