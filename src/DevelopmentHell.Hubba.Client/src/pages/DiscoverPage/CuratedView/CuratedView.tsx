import CollaboratorCard from "../CollaboratorCard/CollaboratorCard";
import { ICuratedData } from "../DiscoverPage";
import ListingCard from "../ListingCard/ListingCard";
import ProjectShowcaseCard from "../ProjectShowcaseCard/ProjectShowcaseCard";
import "./CuratedView.css";
interface ICuratedViewProps {
    data?: ICuratedData;
}

const CuratedView: React.FC<ICuratedViewProps> = (props) => {
    return (
        <div id="curated-view-wrapper" className="curated-view-wrapper">
            <h3 className="category">Listings</h3>
            <div className="catalogue">
                {props.data && props.data.listings.length == 0 &&
                    <p id="empty-listings">No Listings found.</p>
                }
                {props.data && props.data.listings && props.data.listings.length > 0 &&
                    props.data.listings.map(item => {
                        return (
                            <ListingCard key={`listing-card-${item.ListingId}`} data={item} />
                        );
                    })
                }
            </div>

            <h3 className="category">Project Showcases</h3>
            <div className="catalogue">
                {props.data?.showcases.length == 0 &&
                    <p id="empty-project-showcases">No Project Showcases found.</p>
                }
                {props.data && 
                    props.data.showcases.map(item => {
                        return (
                            <ProjectShowcaseCard key={`showcase-card-${item.Id}`} data={item} />
                        );
                    })
                }
            </div>

            <h3 className="category">Collaborators</h3>
            <div className="catalogue">
                {props.data?.collaborators.length == 0 &&
                    <p id="empty-collaborators">No Collaborators found.</p>
                }
                {props.data && 
                    props.data.collaborators.map(item => {
                        return (
                            <CollaboratorCard key={`collaborator-card-${item.CollaboratorId}`} data={item} />
                        );
                    })
                }
            </div>
        </div>
    );
}

export default CuratedView;