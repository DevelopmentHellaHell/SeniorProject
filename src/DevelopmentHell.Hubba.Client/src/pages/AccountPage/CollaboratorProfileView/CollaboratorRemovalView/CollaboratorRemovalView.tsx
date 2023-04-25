import React from "react";
import { useState } from "react";
import { Ajax } from "../../../../Ajax";
import { Auth } from "../../../../Auth";
import Button from "../../../../components/Button/Button";
import "./CollaboratorRemovalView.css";

interface ICollaboratorRemoval {
    onCancelClick: () => void;
}

const CollaboratorRemovalView: React.FC<ICollaboratorRemoval> = (props) => {
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(true);

    const [success, setSuccess] = useState("");

    const authData = Auth.getAccessData();
    const accountId = authData?.sub;

    const onError = (message: string) => {
        setError(message);
        setLoaded(true);
    }

    return (
        <div className="remove-collaborator-wrapper">
            <h1 id="remove-collaborator-header">Collaborator Removal</h1>

            <p>Are you sure you want to remove your collaborator profile?
                <br />This action can't be undone.
            </p>
            <div id="buttons" className="buttons">
                <Button title="Cancel" onClick={ props.onCancelClick }/>           
                
                <Button title="Remove" loading={!loaded} onClick={async () => {
                    setLoaded(false);
                    if(!accountId){
                        onError("Unable to find selected account of collaborator.");
                        return;
                    }

                    const response = await Ajax.post("/collaborator/removeowncollaborator", ({}));
                    if (response.error) {
                        onError(response.error);
                        return;
                    }
                    setSuccess("Successfully removed all collaborator details.");
                    setLoaded(true);
                }}/>
            </div>
            {error &&
                <p id='error' className="error">{error}</p>
            }
            {success &&
                <p id='success' className="success">{success}</p>
            }
            
        </div> 
    );
    
}

export default CollaboratorRemovalView;