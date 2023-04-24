import { useEffect, useState } from "react";
import { Ajax } from "../../../Ajax";
import './MyListingsView.css';
import ListingCard from "./ListingCard/ListingCard";
import Button, { ButtonTheme } from "../../../components/Button/Button";
import { useNavigate } from "react-router-dom";

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
    const [error, setError] = useState<string | undefined>(undefined);
    const [loaded, setLoaded] = useState<boolean>(false);
    const [data, setData] = useState<IListing[] | null>(null);
    const [title, setTitle] = useState<string | null>(null);
    const [showTitleField, setShowTitleField] = useState<boolean>(false);
    const [showSubmitButton, setShowSubmitButton] = useState<boolean>(false);
    const navigate = useNavigate();


    useEffect(() => {
        const getData = async () => {
            const response = await Ajax.get<IListing[]>('/listingprofile/viewMyListings')
            setData(response.data);
            if (response.error) {
                setError(response.error);
                console.log(response.error);
            }
            
            setLoaded(response.loaded);
            console.log(response.data);
        }

        getData();
    }, []);

    useEffect(() => {
        if (error !== undefined) {
          alert(error);
          setError(undefined);
        }
        
      }, [error]);

    return (
        <div className="my-listing-view-container">
            
            {data && data.length > 0 && 
            <div className="listings-found">
             <table>
                <thead>
                    <tr>
                        <th></th>
                        <th className="Listing Title">Listing Title</th>
                        <th className="Rating">Average Rating</th>
                        <th className="Status">Status</th>
                    </tr>
                </thead>
                <tbody>
                    {loaded && data && data.map(value => {
                        return <ListingCard key={`${value.listingId}-listing-card`} listing={value}/>
                    })}
                </tbody>
            </table>
            </div>
            }
            
            <div className = "no-listings"> 
                { data && data.length == 0  &&
                    <h2>You have no listings</h2>
                }
                {(title==undefined || title=='') && !showSubmitButton &&
                <Button theme={ButtonTheme.DARK} onClick={() => { setShowTitleField(true) }} title={"Create Listing"} />
                
                    }
                    {
                            title!==undefined && showSubmitButton && title!== '' &&
                            <Button theme={ButtonTheme.DARK} onClick={async () => { 
                                title!==undefined;
                                const response = await Ajax.post("/listingprofile/createListing", {title} );
                                if (response.error) {
                                    setError(response.error);
                                }
                                setShowTitleField(false);
                                window.location.reload();
                                return;
                            }} title={"Submit"} />
                        }       
                { showTitleField && 
                    <div className="title-input-field">
                        <label>Title</label>
                        <input id="title" type="text" maxLength={50} placeholder="Title" onChange={
                            (event: React.ChangeEvent<HTMLInputElement>) => {
                            setTitle(event.target.value);
                            setShowSubmitButton(true);
                            }
                        }/>
                        
                    </div>
                }
            </div>
        </div>

    )
}

export default MyListingsView;

