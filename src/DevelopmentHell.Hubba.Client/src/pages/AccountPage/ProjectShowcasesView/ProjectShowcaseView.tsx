import React from "react";
import Button from "../../../components/Button/Button";
import "./ProjectShowcase.css";

interface ILoginSecurityViewProps {
    onUpdateClick: () => void;
    onDeleteClick: () => void;
}

const ProjectShowcaseView: React.FC<ILoginSecurityViewProps> = (props) => {
    return (
        <div className="login-security-wrapper">
            <h1>My Project Showcases</h1>
        </div> 
    );
}

export default ProjectShowcaseView;