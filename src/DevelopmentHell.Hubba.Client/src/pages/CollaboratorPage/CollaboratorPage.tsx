import { redirect, useLocation, useNavigate } from "react-router-dom";
import { Auth } from "../../Auth";
import Footer from "../../components/Footer/Footer";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import { Ajax } from "../../Ajax";
import { useEffect, useState } from "react";
import "./CollaboratorPage.css"

interface ICollaboratorPage {
    
}

export interface ICollaboratorData {
    name: string,
    pfpUrl?: string,
    contactInfo: string,
    tags?: string,
    description: string,
    availability?: string,
    votes?: number,
    collabUrls: string[],
    published: boolean,
}

const CollaboratorPage: React.FC<ICollaboratorPage> = (props) => {
    const {state}=useLocation();
    const [error, setError] = useState<string>("Error");
    const [loaded, setLoaded] = useState<boolean>(false);
    const [data, setData] = useState<ICollaboratorData | null>(null);
    const [voted, setVoted] = useState<boolean | null>(null);
    const navigate = useNavigate();
    const authData = Auth.getAccessData();
    const isPublished = data?.published;
    const [selectedImageUrl, setSelectedImageUrl] = useState<string>('');
    const collabId = state.CollaboratorId;


    if(!authData){
        redirect("/login")
        return null;
    }
    useEffect(() => {
        const getData = async () => {
            const response = await Ajax.post<ICollaboratorData>("collaborator/getcollaborator", {collaboratorId: state.CollaboratorId});
            setData(response.data);
            setError(response.error);
            setLoaded(response.loaded);
            if(response.data){
                setSelectedImageUrl(response.data.collabUrls[0]);
            }
        };
        getData();
    }, []);

    const handleUpvoteClick = async () => {
        await Ajax.post("/collaborator/votecollaborator", {
            CollaboratorId: state.CollaboratorId,
            Upvote: true
        }).then((response) => {
            setVoted(true);
            setError(response.error);
            setLoaded(response.loaded);
        });
    }
    
    const handleDownvoteClick = async () => {
        await Ajax.post("/collaborator/votecollaborator", {
            CollaboratorId: state.CollaboratorId,
            Upvote: false
        }).then((response) => {
            setVoted(false)
            setError(response.error);
            setLoaded(response.loaded);
        });
    }

    const handleImageClick = (imageUrl: string) => {
        setSelectedImageUrl(imageUrl);
    }

    return(
        <div className="collaborator-container">
            <NavbarUser />
            <div className="collaborator-content">
                <div className="collaborator-wrapper">
            { data && !error && loaded &&
                    <div className="collaborator-page-profile">
                        <div className="collaborator-page-information-wrapper">
                            <div className="collaborator-page-information-leftcolumn-wrapper">
                                <div className="collaborator-page-vote-title-wrapper">
                                    <div className="collaborator-page-votes-wrapper">
                                        <div className="vote-control">
                                                <div className="h-stack">
                                                    <p id='up-vote' className="up-vote" onClick={() => {
                                                        handleUpvoteClick();
                                                    }}>↑</p>
                                                    <p className="down-vote" onClick={() => {
                                                        handleDownvoteClick();
                                                    }}>↓</p> 
                                                </div>
                                            </div>
                                        <div className = "vote-count">
                                            <p id="vote-count-label" className="vote-count-label">
                                                {data.votes ? data.votes: 0}
                                            </p>
                                        </div>
                                    </div>
                                    <h2 id='collaborator-page-title' className="collaborator-page-title">{data.name}</h2>
                                </div>
                                <div className="collaborator-page-tags-wrapper">
                                    <p id="tags-label" className="tags-label">Tags: </p>
                                    <p id="tags-list" className="tags-list">{data.tags ? data.tags : "No tags provided."}</p>
                                </div>
                                <div className="collaborator-page-description-wrapper">
                                    <h3 className="collaborator-page-description-title">Description</h3>
                                    <p id="description-label" className="description-label">{data.description}</p>
                                </div>
                                <div className="collaborator-page-availability-wrapper">
                                    <h3 id="collaborator-page-availability-title" className="collaborator-page-availability-title">Availability</h3>
                                    <p id="availability-label" className="availability-label">{data.availability ? data.availability: "No availability provided."}</p>
                                </div>
                            </div>
                            <div className="collaborator-page-information-rightcolumn-wrapper">
                                {data.pfpUrl && (
                                    <div className="collaborator-page-pfp-wrapper">
                                        <img className="collaborator-page-pfp" src={data.pfpUrl} alt="Profile Picture"/>
                                    </div>
                                )}
                                <div className="collaborator-page-contact-information-wrapper">
                                    <h3 id="collaborator-page-contact-information-title" className="collaborator-page-contact-information-title">Contact Information</h3>
                                    <p>{data.contactInfo}</p>
                                </div>
                            </div>
                        </div>
                        <div className="collaborator-page-loaded-images-wrapper">
                            <h3>Previous work</h3>
                            {data.collabUrls && (
                                <div className="collaborator-page-image-wrapper">
                                    <div id="collaborator-page-images" className="collaborator-page-images">
                                        {data.collabUrls.map((url) => (
                                            <img id="collaborator-page-image" className="collaborator-page-image"
                                            key={url} src={url} 
                                            onClick={() => handleImageClick(url)}/>
                                        ))}
                                    </div>
                                </div>
                            )}
                        </div>
                        {selectedImageUrl && (
                            <div id="collaborator-page-spread" className="collaborator-page-spread">
                                <img id="collaborator-page-spread-image" className="collaborator-page-spread-image" src={selectedImageUrl} alt="spead-image" />
                            </div>
                        )}  

                    </div>
                } 
                </div>
                {error &&
                    <p className="error">{error}</p>
                }
            </div>
            <Footer />
        </div>
        
    );
    
}



export default CollaboratorPage