import { redirect } from "react-router-dom";
import { Auth } from "../../Auth";
import Footer from "../../components/Footer/Footer";
import NavbarUser from "../../components/NavbarUser/NavbarUser";

interface ICollaboratorProfilePage {

}

export interface ICollaboratorData {
    Name: string,
    ContactInfo: string,
    Tags: string | undefined,
    Description: string,
    Availability: string | undefined,
    Published: boolean,
    PfpFile: FormData | undefined,
    UploadedFiles: FormData[],
}

const CollaboratorProfilePage: React.FC<ICollaboratorProfilePage> = (props) => {
    const authData = Auth.getAccessData();

    if(!authData){
        redirect("/login")
        return null;
    }
    
    return(
        <div className="collaborators-container">
            <NavbarUser />
            <div className="collaborator-content">
                <div className="collaborator-wrapper">

                </div>
            </div>
            <Footer />
        </div>
    );
}

export default CollaboratorProfilePage