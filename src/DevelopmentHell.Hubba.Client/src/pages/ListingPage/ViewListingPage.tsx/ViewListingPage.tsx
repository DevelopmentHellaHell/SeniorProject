import { useState, useEffect } from "react";
import { Ajax } from "../../../Ajax";
import { IListing } from "../../ListingProfilePage/MyListingsView/MyListingsView";
import { useLocation, useNavigate } from "react-router-dom";
import NavbarUser from "../../../components/NavbarUser/NavbarUser";
import Footer from "../../../components/Footer/Footer";
import "./ViewListingPage.css";
import Button, { ButtonTheme } from "../../../components/Button/Button";
import { Auth } from "../../../Auth";
import ViewListingRatingsPage from "./ViewListingRatingsPage/ViewListingRatingsPage";
import ListingAvailabilityCard from "./ViewListingRatingsPage/ListingAvailabilityCard/ListingAvailabilityCard";
import NavbarGuest from "../../../components/NavbarGuest/NavbarGuest";
import ListingRatingCard from "./ViewListingRatingsPage/ListingRatingCard/ListingRatingCard";

interface IViewListingPageProps {

}

export interface IAvailability {
    availabilityId: number,
    startTime: Date,
    endTime: Date,
    listingId: number
}

interface IListingFile {
}

export interface IRating {
    listingId: number,
    userId?: number,
    username?: string,
    rating: number,
    comment?: string,
    anonymous: boolean,
    lastEdited?: Date
}

export interface IViewListingData {
    Listing: IListing,
    Availabilities?: IAvailability[],
    Files?: IListingFile[],
    Ratings?: IRating[]
}


const ViewListingPage: React.FC<IViewListingPageProps> = (props) => {
    const { state } = useLocation();
    const [error, setError] = useState<string | undefined>(undefined);
    const [loaded, setLoaded] = useState<boolean>(false);
    const [data, setData] = useState<IViewListingData | null>(null);
    const navigate = useNavigate();
    const authData = Auth.getAccessData();
    const [isPublished, setIsPublished] = useState<boolean>(false);
    const [currentImage, setCurrentImage] = useState<number>(0);
    const [defaultError] = useState<string>("Listings failed to load. Refresh page or try again later.")

    useEffect(() => {
        const getData = async () => {
            const response = await Ajax.post<IViewListingData>('/listingprofile/viewListing', { listingId: state.listingId });
            console.log(response.data);
            if (response.data) {
                setData(response.data);
                setIsPublished(response.data.Listing.published!);
            }
            if (response.error) {
                setError("Listing failed to load. Refresh page or try again later.\n" + response.error);
            }

            setLoaded(response.loaded);
        };
        getData();
    }, []);

    const handlePrevImage = () => {
        setCurrentImage((prevImage) =>
            prevImage === 0 ? data!.Files!.length! - 1 : prevImage - 1
        );
    };

    const handleNextImage = () => {
        setCurrentImage((prevImage) =>
            prevImage === (data?.Files?.length ?? 0) - 1 ? 0 : prevImage + 1
        );
    };

    useEffect(() => {
        if (error !== undefined) {
            alert(error);
            setError(undefined);
        }

    }, [error]);

    const handleDeleteClick = async () => {
        const response = await Ajax.post<null>('/listingprofile/deleteListing', { ListingId: data?.Listing.listingId })
        console.log(response)
        if (response.error) {
            setError("Listing deletion error. Refresh page or try again later.\n" + response.error);
            return;
        }
        navigate("/listingprofile");
    };

    return (
        <div className="listing-container">
            {authData && authData.role !== Auth.Roles.DEFAULT_USER ?
                <NavbarUser /> : <NavbarGuest />
            }
            
                
                {data && !error && loaded &&
                    <div className="listing-content">
                        <div className="Title">
                            <h1>{data.Listing.title}</h1>
                            <h3>Owner: {data.Listing.ownerUsername}</h3>
                            {data.Listing.averageRating && 
                                <h4>Average Rating: {data.Listing.averageRating}</h4>
                            }
                        </div>

                        <div className="Buttons">
                            <div className="listing-page-status"> {isPublished ? 'Public' : 'Draft'} </div>
                            {/** REDIRECT TO SCHEDULING FEATURE */}
                            {isPublished && authData?.sub != data.Listing.ownerId.toString() &&
                            <Button title="Check Calendar"
                                onClick={() => {
                                    navigate("/scheduling", {
                                        state: {
                                            listingId: data?.Listing.listingId,
                                            listingTitle: data?.Listing.title,
                                            ownerId: data?.Listing.ownerId,
                                            price: data?.Listing.price
                                        }
                                    })
                                }} />
                            }
                            {authData?.sub == data.Listing.ownerId.toString() && <div>
                                <p><Button theme={ButtonTheme.HOLLOW_DARK} onClick={async () => {
                                    const response = await Ajax.post("/listingprofile/unpublishListing", { listingId: data.Listing.listingId })
                                    if (response.error) {
                                        setError("Publishing listing error. Refresh page or try again later.\n" + response.error);
                                        return;
                                    }
                                    setIsPublished(false);
                                }} title={"Unpublish Listing"} /></p>
                                <p><Button theme={ButtonTheme.HOLLOW_DARK} onClick={() => { navigate("/editlisting", { state: { listingId: data.Listing.listingId } }) }} title={"Edit Listing"} /></p>
                                
                                <p><Button theme={ButtonTheme.DARK} onClick={() => { handleDeleteClick() }} title={"Delete Listing"} /></p>
                            </div>
                            }
                        </div>
                    
                        {data.Files && data.Files.length > 0 && (
                            <div className="Files">
                                {data!.Files![currentImage].toString().substring(data!.Files![currentImage].toString().lastIndexOf('/') + 1)} {currentImage + 1} / {data!.Files!.length}
                                <img
                                    className="listing-page__picture"
                                    src={data.Files[currentImage]?.toString()}
                                    alt={data.Listing.title}
                                />
                                {data.Files.length > 1 && (
                                    <>
                                        <button onClick={handlePrevImage}>
                                            &#10094;
                                        </button>
                                        <button onClick={handleNextImage}>
                                            &#10095;
                                        </button>
                                    </>
                                )}
                            </div>
                        )}
                    
                        <div className="Information">
                            <div className="Location">
                                <h3>{"Location: "}</h3> 
                                <p>{data.Listing.location ?? ""}</p>
                            </div>
                            <div className="Price">
                                <h3>{"Price: " }</h3>
                                <p>{data.Listing.price ?? ""}</p>
                            </div>
                            <div className="Description">
                                <h3>{"Description: "}</h3>
                                <p>{data.Listing.description ?? ""}</p>
                            </div>
                        </div>
                    
                                      
                        <div className="Ratings">
                            <h2>Ratings</h2>
                            <table>
                                <thead>
                                    <tr>
                                        <th>Ratings</th>
                                        <th>Comment</th>
                                        <th>Username</th>
                                        <th>Status</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    { data && data.Ratings && data.Ratings.map((value: IRating) => {
                                        return <ListingRatingCard key={`${value.listingId}-listing-card`} rating={value}/>
                                    })}
                                </tbody>
                            </table>
                        </div>
                    </div>
                }

                {error && loaded &&
                    <p className="error">{error}</p>
                }
                
                
            <Footer />
        </div>
    );
}

export default ViewListingPage;
