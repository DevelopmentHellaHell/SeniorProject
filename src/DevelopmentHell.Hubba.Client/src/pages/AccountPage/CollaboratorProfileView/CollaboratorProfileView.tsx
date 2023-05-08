import { redirect } from "react-router-dom";
import Button from "../../../components/Button/Button";
import "./CollaboratorProfileView.css";

interface ICollaboratorProfileView {
    onViewClick: () => void;
    onEditClick: () => void;
    onRemoveClick: () => void;
    onDeleteCollabClick: () => void;
}

const CollaboratorProfileView: React.FC<ICollaboratorProfileView> = (props) => {
    return(
        <div className="collaborator-wrapper">
            <h1 id="collaborator-profile-header">Collaborator Profile</h1>

            <h2>View Collaborator Profile</h2>
            <div className ="view-collaborator">
                <p className="inline-text">View profile, or create if none exists.</p>
                <div id="view-button" className="buttons">
                    <Button title="View" onClick={ props.onViewClick }/>
                </div>
            </div>

            <h2>Edit Collaborator Profile</h2>
            <div className ="edit-collaborator">
                <p className="inline-text">Edit profile and change visibility.</p>
                <div id="edit-button" className="buttons">
                    <Button title="Edit" onClick={ props.onEditClick }/>
                </div>
            </div>

            <h2>Remove Collaborator Profile</h2>
            <div className="remove-collaborator">
                <p className="inline-text">Remove your collaborator profile. This will permanently clear all data about your collaborator profile.</p>
                <div id = "remove-button" className="buttons">
                    <Button title="Remove" onClick={ props.onRemoveClick }/>
                </div>
            </div>

            <h2>Delete Collaborator Profile</h2>
            <div className="delete-collaborator">
                <p className="inline-text">Delete your collaborator profile. This will permanently delete your collaborator profile.</p>
                <div id = "delete-button" className="buttons">
                    <Button title="Delete" onClick={ props.onDeleteCollabClick }/>
                </div>
            </div>

        </div>
    );
}

export default CollaboratorProfileView;