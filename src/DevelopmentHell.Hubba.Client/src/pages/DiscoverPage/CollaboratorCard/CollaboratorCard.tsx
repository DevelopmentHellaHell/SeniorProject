import React, { useEffect, useState } from "react";
import { IDiscoveryCollaborator } from "../DiscoverPage";
import "./CollaboratorCard.css";
import { Ajax } from "../../../Ajax";
import { useNavigate } from "react-router-dom";

interface ICollaboratorCardProps {
    data: IDiscoveryCollaborator;
    onClick?: () => void;
}

const CollaboratorCard: React.FC<ICollaboratorCardProps> = (props) => {
    const [thumbnail, setThumbnail] = useState<string | undefined>(undefined);
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(false);
    const navigate = useNavigate();
    
    useEffect(() => {
        const getFile = async () => {
            const response = await Ajax.post<string[]>("/collaborator/getFiles", { CollaboratorId: props.data.CollaboratorId });

            setError(response.error);
            setLoaded(response.loaded);

            if (response.data) {
                setThumbnail(response.data[0]);
            }
        }

        getFile();
    }, [props.data]);

    return (
        <div className="collaborator-card" onClick={() => { navigate("/collaborators", { state: { CollaboratorId: props.data.CollaboratorId } })}}>
            {!error && 
                <div>
                    <img className="thumbnail" src={thumbnail} />
                    <div className="info-block">
                        <p className="title">{props.data.Name}</p>
                        <div className="rating-block">
                            <span className="star">★</span><p className="count">{props.data.TotalVotes}</p>
                        </div>
                        <div className="description">

                        </div>
                    </div>
                </div>
            }
            {error && 
                <div className="error">
                    <p>{error}</p>
                </div>
            }
        </div> 
    );
}

export default CollaboratorCard;