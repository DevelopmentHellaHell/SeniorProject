import { ICuratedData } from "../DiscoverPage";
import ListingCard from "../ListingCard/ListingCard";
import "./CuratedView.css";
interface ICuratedViewProps {
    data?: ICuratedData;
}

const CuratedView: React.FC<ICuratedViewProps> = (props) => {
    return (
        <div className="curated-view-wrapper">
            <h3 className="category">Listings</h3>
            <div className="catalogue">
                {props.data && 
                    props.data.listings.map(item => {
                        return (
                            <ListingCard data={item} />
                        );
                    })
                }
            </div>

            <h3 className="category">Project Showcases</h3>
            <div className="catalogue">
                {props.data && 
                    props.data.listings.map(item => {
                        return (
                            <ListingCard data={item} />
                        );
                    })
                }
            </div>

            <h3 className="category">Collaborators</h3>
            <div className="catalogue">
                {props.data && 
                    props.data.listings.map(item => {
                        return (
                            <ListingCard data={item} />
                        );
                    })
                }
            </div>
        </div>
    );
}

export default CuratedView;