import React from "react";
import { IDiscoveryListing } from "../DiscoverPage";
import "./ListingCard.css";

interface IListingCardProps {
    data: IDiscoveryListing;
    onClick?: () => void;
}

const ListingCard: React.FC<IListingCardProps> = (props) => {
    return (
        <div className="listing-card" onClick={() => { alert(props.data.ListingId) }}>
            <img className="thumbnail" src="https://upload.wikimedia.org/wikipedia/commons/f/f9/Phoenicopterus_ruber_in_S%C3%A3o_Paulo_Zoo.jpg" alt="alternatetext" />
            <div className="info-block">
                <p className="title">{props.data.Title}</p>
                <div className="rating-block">
                    <div className="rating">
                        <div className="upper" style={{
                            width: (props.data.AvgRatings/5)*100 + "%",
                        }}>
                            <span>★★★★★</span>
                        </div>
                        <div className="lower">
                            <span>★★★★★</span>
                        </div>
                    </div>
                    <p>{props.data.TotalRatings}</p>
                </div>
                <div className="description">
                    <p>{props.data.Location}</p>
                    <p>${props.data.Price}</p>
                </div>
            </div>
        </div> 
    );
}

export default ListingCard;