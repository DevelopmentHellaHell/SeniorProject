import React from "react";
import Button from "../../../components/Button/Button";
import "./LoginSecurityView.css";

interface ILoginSecurityViewProps {
    onUpdateClick: () => void;
    onDeleteClick: () => void;
}

const LoginSecurityView: React.FC<ILoginSecurityViewProps> = (props) => {
    return (
        <div className="login-security-wrapper">
            <h1>Login & Security</h1>

            <h2>Login</h2>
            <div className ="login-password">
                <p className="inline-text">Change password</p>
                <div className="buttons">
                    <Button title="Update" onClick={ props.onUpdateClick }/>
                </div>
            </div>

            <h2>Account</h2>
            <div className="delete-account">
                <p className="inline-text">Delete your account</p>
                <div className="buttons">
                    <Button title="Delete" onClick={ props.onDeleteClick }/>
                </div>
            </div>
        </div> 
    );
}

export default LoginSecurityView;