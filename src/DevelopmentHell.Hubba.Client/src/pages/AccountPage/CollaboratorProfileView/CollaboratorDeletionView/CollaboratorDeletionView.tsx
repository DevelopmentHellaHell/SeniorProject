import React from "react";
import { useState } from "react";
import { Ajax } from "../../../../Ajax";
import { Auth } from "../../../../Auth";
import Button from "../../../../components/Button/Button";
import "./CollaboratorDeletionView.css";
import { AccountViews } from "../../AccountPage";
import { useNavigate } from "react-router-dom";

interface ICollaboratorDeletion {
    onCancelClick: () => void;
}

const CollaboratorDeletionView: React.FC<ICollaboratorDeletion> = (props) => {
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(true);

    const [success, setSuccess] = useState("");

    const navigate = useNavigate();

    const authData = Auth.getAccessData();
    const accountId = authData?.sub;

    const onError = (message: string) => {
        setError(message);
        setLoaded(true);
    }

    return (
        <div className="deletion-collaborator-wrapper">
            <h1 id="deletion-collaborator-header">Collaborator Profile Deletion</h1>

            <p>Are you sure you want to delete your collaborator profile?
                <br />This action can't be undone.
            </p>
            <div id="buttons" className="buttons">
                <Button title="Cancel" onClick={ props.onCancelClick }/>           
                
                <Button title="Delete" loading={!loaded} onClick={async () => {
                    setLoaded(false);
                    if(!accountId){
                        onError("Unable to find selected account of collaborator.");
                        return;
                    }

                    const response = await Ajax.post("/collaborator/deletecollaboratorwithaccountid", ({accountId}));
                    if (response.error) {
                        onError(response.error);
                        return;
                    }
                    setSuccess("Successfully deleted collaborator profile.");
                    setLoaded(true);
                    window.location.reload();
                    navigate("/account", { state: { view: AccountViews.COLLABORATOR_PROFILE }});
                }}/>
            </div>
            {error &&
                <p className="error">{error}</p>
            }
            {success &&
                <p className="success">{success}</p>
            }
            
        </div> 
    );
    
}

export default CollaboratorDeletionView;