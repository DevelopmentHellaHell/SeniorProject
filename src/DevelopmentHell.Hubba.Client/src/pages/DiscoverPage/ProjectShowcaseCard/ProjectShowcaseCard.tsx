import React, { useState, useEffect } from "react";
import { IDiscoveryProjectShowcase } from "../DiscoverPage";
import "./ProjectShowcaseCard.css";
import { useNavigate } from "react-router-dom";
import { Ajax } from "../../../Ajax";

interface IProjectShowcaseCardProps {
    data: IDiscoveryProjectShowcase;
    onClick?: () => void;
}

const ProjectShowcaseCard: React.FC<IProjectShowcaseCardProps> = (props) => {
    const [thumbnail, setThumbnail] = useState<string | undefined>(undefined);
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(false);
    const navigate = useNavigate();

    useEffect(() => {
        const getFile = async () => {
            const response = await Ajax.get<string[]>(`/showcases/files?s=${props.data.Id}`);
            setError(response.error);
            setLoaded(response.loaded);

            if (response.data) {
                setThumbnail(response.data[0]);
            }
        }

        getFile();
    }, [props.data]);

    return (
        <div className="project-showcase-card" onClick={() => { navigate(`/showcases/p/view?s=${props.data.Id}`) }}>
            {!error &&
                <div>
                     <img className="thumbnail" src={thumbnail} />
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
            }
            {error && 
                <div className="error">
                    <p>{error}</p>
                </div>
            }
        </div> 
    );
}

export default ProjectShowcaseCard;