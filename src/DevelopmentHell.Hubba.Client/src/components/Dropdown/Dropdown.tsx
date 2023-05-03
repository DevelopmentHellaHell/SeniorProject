import React from "react";
import './Dropdown.css';

interface IDropdownProps {
    children?: React.ReactNode;
    title: string;
    id?: string;
}

const Dropdown: React.FC<IDropdownProps> = (props: React.PropsWithChildren<IDropdownProps>) => {
    return (
        // Code adapted from w3schools.
        // Link: https://www.w3schools.com/howto/howto_css_dropdown.asp
        <div className="dropdown">
            <button className="drop-btn">{props.title}</button>
            <div className="dropdown-content" id={props.id}>
                {props.children}
            </div>
        </div>
    );
}

export default Dropdown;