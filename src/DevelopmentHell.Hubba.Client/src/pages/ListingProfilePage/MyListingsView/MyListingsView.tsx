import { useEffect, useState } from "react";
import { Ajax } from "../../../Ajax";
import './MyListingsView.css';
import ListingCard from "./ListingCard/ListingCard";

interface IMyListingsViewProps {
    
}

export interface IListing {
    ownerId: number,
    title: string,
    ownerUsername?: string,
    listingId: number,
    description?: string,
    location?: string,
    price?: number,
    lastEdited: Date,
    published: boolean,
    averageRating?: number
}



const MyListingsView: React.FC<IMyListingsViewProps> = (props) => {
    const [error, setError] = useState<string>("Error");
    const [loaded, setLoaded] = useState<boolean>(false);
    const [data, setData] = useState<IListing[] | null>(null);


    useEffect(() => {
        const getData = async () => {
            const response = await Ajax.get<IListing[]>('/listingprofile/viewMyListings')
            setData(response.data);
            setError(response.error);
            setLoaded(response.loaded);
        }

        getData();
    }, []);
    return (
        <div className="my-listing-view">
             <table>
                <thead>
                    <tr>
                        <th></th>
                        <th>Listing Title</th>
                        <th>Ratings</th>
                        <th>Status</th>
                    </tr>
                </thead>
                <tbody>
                    {loaded && data && data.map(value => {
                        return <ListingCard key={`${value.listingId}-listing-card`} listing={value}/>
                    })}
                </tbody>
            </table>
        </div>
    )
}

export default MyListingsView;

