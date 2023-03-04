import React from "react";
import './Sidebar.css';

interface ISidebarProps {
    children?: React.ReactNode;
}

const Sidebar: React.FC<ISidebarProps> = (props: React.PropsWithChildren<ISidebarProps>) => {
    return (
        <div className="sidebar">
            <div className="links">
                {props.children}
            </div>
        </div>
    );
}

export default Sidebar;