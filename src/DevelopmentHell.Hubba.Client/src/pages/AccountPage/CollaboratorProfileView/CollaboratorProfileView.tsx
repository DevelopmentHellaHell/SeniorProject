import { redirect } from "react-router-dom";
import { Auth } from "../../../Auth";

interface ICollaboratorProfileView {
    onRemoveClick: () => void;
}

const CollaboratorProfileView: React.FC<ICollaboratorProfileView> = (props) => {
    const authData = Auth.getAccessData();
    if(!authData){
        redirect("/login")
        return null;
    }
    return(
        <div className="collaborators-container">
            
            <div className="collaborator-content">
                <div className="collaborator-wrapper">

                </div>
            </div>
        </div>
    );
}

export default CollaboratorProfileView