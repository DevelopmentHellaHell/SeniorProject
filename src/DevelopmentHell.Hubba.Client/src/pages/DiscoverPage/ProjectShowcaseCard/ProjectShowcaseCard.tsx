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
            <img className="thumbnail" src="https://www.elitetreecare.com/wp-content/uploads/2019/01/oak-tree.jpg" alt="alternatetext" />
            <div className="info-block">
                <p className="title">{props.data.Title + "dklajsdlkajslkaaaaaaaaaad"}</p>
                <div className="rating-block">
                    <span className="star">★</span><p className="count">{props.data.Rating}</p>
                </div>
                <div className="description">
                    <p>{props.data.Description + "test test test test test 123q42r fa a asfasd aaaaaaaaaaaaaadklajsdlkajslkaaaaaaadklajsdlkajslkaaaaaaaaaaddklajsdlkajslkaaaaaaaaaadaaaddklajsdlkajslkaaaaaaaaaaddklajsdlkajslkaaaaaaaaaaddklajsdlkajslkaaaaaaaaaadaaaaaaa"}</p>
                </div>
            </div>
        </div> 
    );
}

export default ProjectShowcaseCard;