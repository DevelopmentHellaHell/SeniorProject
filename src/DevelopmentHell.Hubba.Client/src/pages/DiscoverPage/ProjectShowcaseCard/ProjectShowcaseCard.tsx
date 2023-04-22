import React from "react";
import { IDiscoveryProjectShowcase } from "../DiscoverPage";
import "./ProjectShowcaseCard.css";

interface IProjectShowcaseCardProps {
    data: IDiscoveryProjectShowcase;
    onClick?: () => void;
}

const ProjectShowcaseCard: React.FC<IProjectShowcaseCardProps> = (props) => {
    return (
        <div className="project-showcase-card" onClick={() => { alert(props.data.Id) }}>
            <img className="thumbnail" src="http://104.187.196.233/Testing/TestDir/cecs448.png" />
            <div className="info-block">
                <p className="title">{props.data.Title}</p>
                <div className="rating-block">
                    <span className="star">★</span><p className="count">{props.data.Rating}</p>
                </div>
                <div className="description">
                    <p>{props.data.Description}</p>
                </div>
            </div>
        </div> 
    );
}

export default ProjectShowcaseCard;