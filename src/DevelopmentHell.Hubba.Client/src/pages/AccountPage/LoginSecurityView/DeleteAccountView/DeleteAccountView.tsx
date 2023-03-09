import React from "react";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Ajax } from "../../../../Ajax";
import { Auth } from "../../../../Auth";
import Button from "../../../../components/Button/Button";
import "./DeleteAccountView.css";

interface IDeleteAccountViewProps {
    onCancelClick: () => void;
}

const DeleteAccountView: React.FC<IDeleteAccountViewProps> = (props) => {
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(true);

    const navigate = useNavigate();
    const authData = Auth.getAccessData();
    const accountId = authData?.sub;

    const onError = (message: string) => {
        setError(message);
        setLoaded(true);
    }

    return (
        <div className="delete-account-wrapper">
            <h1>Account Deletion</h1>

            <p>Are you sure you want to delete your account?
                <br />This action can't be undone.
            </p>

            <div className="buttons">
                <Button title="Cancel" onClick={ props.onCancelClick }/>
                <Button title="Delete" loading={!loaded} onClick={async () => {
                    setLoaded(false);
                    if(!accountId){
                        onError("Unable to find selected account.");
                        return;
                    }

                    const response = await Ajax.post("/accountdeletion/deleteaccount", ({ accountId: accountId }));
                    if (response.error) {
                        onError(response.error);
                        return;
                    }

                    setLoaded(true);
                    Auth.removeCookie("access_token");
                    navigate("/");
                }}/>
            </div>
            {error &&
                <p className="error">{error}</p>
            }
        </div> 
    );
}

export default DeleteAccountView;