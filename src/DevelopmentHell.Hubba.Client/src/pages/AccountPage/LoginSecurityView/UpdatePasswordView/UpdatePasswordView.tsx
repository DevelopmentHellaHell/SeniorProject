import React from "react";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Ajax } from "../../../../Ajax";
import { Auth } from "../../../../Auth";
import Button from "../../../../components/Button/Button";

interface IUpdatePasswordViewProps {
onCancelClick: () => void;
}

const UpdatePasswordView: React.FC<IUpdatePasswordViewProps> = (props) => {

    return(
        <div className="update-password-wrapper">
            <h1 id="update-password-header">Change Password</h1>

            <p>Press Update to begin process. Press cancel to return.</p>
            <div id="buttons" className="buttons">
                <Button title="Press Me" onClick={ props.onCancelClick}/>
            </div>
        </div>
    );
}

export default UpdatePasswordView;