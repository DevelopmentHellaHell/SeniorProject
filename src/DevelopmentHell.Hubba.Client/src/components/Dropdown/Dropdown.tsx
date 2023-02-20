import React from "react";
import './Dropdown.css';

interface Props {
    children?: React.ReactNode;
    title: string;
}

const Dropdown: React.FC<Props> = (props: React.PropsWithChildren<Props>) => {
    return (
        // Code adapted from w3schools.
        // Link: https://www.w3schools.com/howto/howto_css_dropdown.asp
        <div className="dropdown">
            <button className="drop-btn">{props.title}</button>
            <div className="dropdown-content">
                {props.children}
            </div>
        </div>
    );
}

export default Dropdown;