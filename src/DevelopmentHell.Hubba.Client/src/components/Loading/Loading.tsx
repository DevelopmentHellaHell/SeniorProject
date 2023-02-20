import React from 'react';
import "./Loading.css";

interface Props {
    title?: string;
}

const Loading: React.FC<Props> = (props) => {
    return (
        <div className="loader-wrapper">
            <div className="loader"/>
            {props.title &&
                <p className="loader-text">{props.title}</p>
            }
        </div>
    )
}

export default Loading;