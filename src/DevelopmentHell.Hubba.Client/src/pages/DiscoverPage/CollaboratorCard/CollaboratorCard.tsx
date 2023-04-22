import React from "react";
import { IDiscoveryCollaborator } from "../DiscoverPage";
import "./CollaboratorCard.css";

interface ICollaboratorCardProps {
    data: IDiscoveryCollaborator;
    onClick?: () => void;
}

const CollaboratorCard: React.FC<ICollaboratorCardProps> = (props) => {
    return (
        <div className="collaborator-card" onClick={() => { alert(props.data.CollaboratorId) }}>
            <img className="thumbnail" src="https://hips.hearstapps.com/hmg-prod/images/dog-puppy-on-garden-royalty-free-image-1586966191.jpg?crop=0.752xw:1.00xh;0.175xw,0&resize=1200:*" alt="alternatetext" />
            <div className="info-block">
                <p className="title">{props.data.Name + "dklajsdlkajslkaaaaaaaaaad"}</p>
                <div className="rating-block">
                    <span className="star">★</span><p className="count">{props.data.TotalVotes}</p>
                </div>
                <div className="description">
                    {/*<p>{props.data.Description + "test test test test test 123q42r fa a asfasd aaaaaaaaaaaaaaaaaaaaa"}</p>*/}
                </div>
            </div>
        </div> 
    );
}

export default CollaboratorCard;