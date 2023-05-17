import React from "react";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Ajax } from "../../../../Ajax";
import { Auth } from "../../../../Auth";
import Button, { ButtonTheme } from "../../../../components/Button/Button";

interface IUpdatePasswordViewProps {
    onCancelClick: () => void;
    onSuccess: () => void;
}

const UpdatePasswordView: React.FC<IUpdatePasswordViewProps> = (props) => {

    const [oldPassword, setOldPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [newPasswordDupe, setNewPasswordDupe] = useState("");
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(true);
    const [success, setSuccess] = useState(false);

    const navigate = useNavigate();

    const onError = (message: string) => {
        setError(message);
        setLoaded(true);
    }

    return(
        <div className="update-password-wrapper">
            <h1 id="update-password-header">Change Password</h1>

            <p>Enter in the proper information to complete changing your password.</p>
            <div>
                <div className="input-field" id="old-password">
                    <label>Password</label>
                    <input id='oldPassword' type="password" placeholder="Password" onChange={(event: React.ChangeEvent<HTMLInputElement>) =>{
                        setOldPassword(event.target.value);
                    }}/>
                </div>
                <div className="input-field" id="new-password">
                    <label>New Password</label>
                    <input id='newPassword' type="password" placeholder="New Password" onChange={(event: React.ChangeEvent<HTMLInputElement>) =>{
                        setNewPassword(event.target.value);
                    }}/>
                </div>
                <div className="input-field" id="new-password-dupe">
                    <label>New Password</label>
                    <input id='newPasswordDupe' type="password" placeholder="New Password" onChange={(event: React.ChangeEvent<HTMLInputElement>) =>{
                        setNewPasswordDupe(event.target.value);
                    }}/>
                </div>
            </div>
            <div className="buttons">
                <Button title="Cancel" onClick={ props.onCancelClick}/>
                <Button title="Submit" theme={ButtonTheme.DARK} loading={!loaded} onClick={async () => {
                    setLoaded(false);
                    setSuccess(false);
                    if (!oldPassword){
                        onError("Please enter in your original password.");
                        return;
                    }
                    if (!newPassword || !newPasswordDupe){
                        onError("Please enter in your new password. ");
                        return;
                    };
                    if (newPassword != newPasswordDupe){
                        onError("Please ensure that both entries for your new password match. ");
                        return;
                    }

                    const response = await Ajax.post("/accountsystem/updatepassword", ({ oldPassword: oldPassword, newPassword: newPassword, newPasswordDupe: newPasswordDupe }));
                    if (response.error){
                        onError(response.error);
                        return;
                    }

                    setSuccess(true);
                    setLoaded(true);
                    
                    props.onSuccess();
                }}/>
            </div>
            {error && 
                <p className="error">{error}</p>
            }
            {success &&
                <p className="success">Success</p>
            }
        </div>
    );
}

export default UpdatePasswordView;