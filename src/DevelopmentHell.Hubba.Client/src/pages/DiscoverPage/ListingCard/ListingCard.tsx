import React, { useEffect, useState } from "react";
import { IDiscoveryListing } from "../DiscoverPage";
import "./ListingCard.css";
import { Ajax } from "../../../Ajax";

interface IListingCardProps {
    data: IDiscoveryListing;
    onClick?: () => void;
}

const ListingCard: React.FC<IListingCardProps> = (props) => {
    const [thumbnail, setThumbnail] = useState<string | undefined>(undefined);
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(false);

    useEffect(() => {
        const getFile = async () => {
            const response = await Ajax.post<string[]>("/listingprofile/getListingFiles", { "ListingId": props.data.ListingId });

            setError(response.error);
            setLoaded(response.loaded);

            if (response.data) {
                setThumbnail(response.data[0]);
            }
        }

        getFile();
    }, [props.data]);

    return (
        <div className="listing-card" onClick={() => { alert(props.data.ListingId) }}>
            {!error &&
                <div>
                    <img className="thumbnail" src={thumbnail} alt="alternatetext" />
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
            }
            {error && 
                <div className="error">
                    <p>{error}</p>
                </div>
            }
        </div> 
    );
}

export default ListingCard;