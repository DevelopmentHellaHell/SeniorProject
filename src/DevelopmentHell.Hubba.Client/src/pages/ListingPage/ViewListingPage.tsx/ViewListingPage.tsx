import { useState, useEffect } from "react";
import { Ajax } from "../../../Ajax";
import { IListing } from "../../ListingProfilePage/MyListingsView/MyListingsView";
import { useLocation, useNavigate } from "react-router-dom";
import NavbarUser from "../../../components/NavbarUser/NavbarUser";
import Footer from "../../../components/Footer/Footer";
import "./ViewListingPage.css";
import Button, { ButtonTheme } from "../../../components/Button/Button";
import { Auth } from "../../../Auth";
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
    const [rating, setRating] = useState<IRating>({
        listingId: state.listingId,
        userId: Number(authData?.sub),
        rating: 0,
        comment: "",
        anonymous: true,
        lastEdited: new Date(),
      });
    const [ratingError, setRatingError] = useState<string | undefined>(undefined);
    const [hasBooked, setHasBooked] = useState<boolean>(false);
    const [hasRating, setHasRating] = useState<boolean>(false);

    // TODO: Check if rating is owner to hide rating, Check if they having booking history with the rating
    useEffect(() => {
        const getData = async () => {
            const response = await Ajax.post<IViewListingData>('/listingprofile/viewListing', { listingId: state.listingId });
            
            if (response.data) {
                setData(response.data);
                setIsPublished(response.data.Listing.published!);
                if (authData?.role !== Auth.Roles.DEFAULT_USER)
                {
                    const userHistory = await Ajax.post<string>('/listingprofile/hasListingHistory',  { listingId: state.listingId } );
                    if (userHistory.data) {
                        console.log(userHistory.data);
                        if (userHistory.data == "none") {
                            setHasBooked(false);
                            setHasRating(false);
                        }
                        else if (userHistory.data == "history") {
                            setHasBooked(true);
                            setHasRating(false);
                        }
                        else if (userHistory.data == "rating") {
                            setHasBooked(true);
                            setHasRating(true);
                        }
                    }
                }
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

    const validateComment = (comment?: string) => {

        if (comment && comment.length > 200) {
            setRatingError("Comment is too long.");
            return false;
        }

        return true;
    }

    const validateRating = (rating?: number) => {
        if (!rating) {
            setRatingError("You must provide a rating.");
            return false;
        }

        if (rating < 0 || rating > 5) {
            setRatingError("Rating must be between 0 to 5.");
            return false;
        }

        return true;
    }

    useEffect(() => {
        if (error !== undefined) {
            alert(error);
            setError(undefined);
        }

    }, [error]);

    const handleDeleteClick = async () => {
        const response = await Ajax.post<null>('/listingprofile/deleteListing', { ListingId: data?.Listing.listingId })
        
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
                            {isPublished && authData?.sub != data.Listing.ownerId &&
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
                            {authData?.sub == data.Listing.ownerId && <div>
                                { data.Listing.published && <p><Button theme={ButtonTheme.HOLLOW_DARK} onClick={async () => {
                                    const response = await Ajax.post("/listingprofile/unpublishListing", { listingId: data.Listing.listingId })
                                    if (response.error) {
                                        setError("Publishing listing error. Refresh page or try again later.\n" + response.error);
                                        return;
                                    }
                                    setIsPublished(false);
                                    window.location.reload();
                                    }} title={"Unpublish Listing"} /></p>
                                }
                                <p><Button theme={ButtonTheme.HOLLOW_DARK} onClick={() => { navigate("/editlisting", { state: { listingId: data.Listing.listingId } }) }} title={"Edit Listing"} /></p>
                                
                                <p><Button theme={ButtonTheme.DARK} onClick={() => { handleDeleteClick() }} title={"Delete Listing"} /></p>

                                <p><Button theme={ButtonTheme.DARK} onClick={() => {navigate(`/showcases/listing?l=${data.Listing.listingId}`)}} title={"View Showcases"}/></p>
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
                            {authData && authData.role !== Auth.Roles.DEFAULT_USER && data.Listing.ownerId != authData.sub && hasBooked &&
                                <div className="rating-inputs">
                                    <input id="listing-page-ratings-comment" placeholder={data.Ratings?.find(rating => rating.userId == authData.sub)?.comment ? data.Ratings?.find(rating => rating.userId == authData.sub)?.comment : "Comment"} type="text" onChange={(event) => {
                                        setRating({
                                            ...rating,
                                            comment: event.target.value,
                                        });
                                    }}/>
                                    <input id="listing-page-ratings-comment" placeholder={data.Ratings?.find(rating => rating.userId == authData.sub)?.rating ? data.Ratings?.find(rating => rating.userId == authData.sub)?.rating.toString() : "0-5"} type="number" onChange={(event) => {
                                        setRating({
                                            ...rating,
                                            rating: parseInt(event.target.value),
                                        });
                                    }}/>
                                    <Button theme={rating.anonymous ? ButtonTheme.DARK : ButtonTheme.HOLLOW_DARK} title={"Anonymous"} onClick={() => {
                                        setRating({
                                            ...rating,
                                            anonymous:  !rating.anonymous
                                        });
                                    }}/>

                                    
                                    {data && data.Ratings && hasRating ? // TODO: REPLACE THIS CODE WITH: Check a rating already exists
                                        <div>
                                            <Button theme={ButtonTheme.DARK} title={"Edit Comment"} loading={!loaded} onClick={async () => {
                                                setLoaded(false);
                                                if (!validateComment(rating.comment)) {
                                                    setLoaded(true);
                                                    return;
                                                }

                                                if (!validateRating(rating.rating)) {
                                                    setLoaded(true);
                                                    return;
                                                }
                                                
                                                // TODO: Ajax call to edit
                                                const response = await Ajax.post<null>('/listingprofile/editRating', { ListingId: data.Listing.listingId, Rating: rating.rating, Comment: rating.comment, Anonymous: rating.anonymous })
                                                console.log(response)
                                                if (response.error) {
                                                    setError("Listing review editing error. Refresh page or try again later.\n" + response.error);
                                                    return;
                                                }
                                                window.location.reload();
                                                setLoaded(true);
                                                
                                            }}/> 
                                            <Button theme={ButtonTheme.DARK} title={"Delete Comment"} loading={!loaded} onClick={async () => {
                                                const response = await Ajax.post<null>('/listingprofile/deleteRating', { ListingId: data.Listing.listingId })
                                                console.log(response)
                                                if (response.error) {
                                                    setError("Review deletion error. Refresh page or try again later.\n" + response.error);
                                                    return;
                                                }
                                                window.location.reload();
                                            }} />
                                        </div> :
                                        <Button theme={ButtonTheme.DARK} title={"Submit"} loading={!loaded} onClick={async () => {
                                            setLoaded(false);
                                            if (!validateComment(rating.comment)) {
                                                setLoaded(true);
                                                return;
                                            }
                                            
                                            if (!validateRating(rating.rating)) {
                                                setLoaded(true);
                                                return;
                                            }

                                            // TODO: Ajax call to submit rating
                                            const response = await Ajax.post<null>("/listingprofile/addRating", { ListingId: state.listingId, Rating: rating.rating, Comment: rating.comment, Anonymous: rating.anonymous });
                                            if (response.error) {
                                                setError("Listing rating and comment error. Ratings failed to upload. Refresh page or try again later.\n" +response.error);
                                                return;
                                            }
                                            window.location.reload();
                                            setLoaded(true);
                                        }}/>
                                    }
                                    {ratingError && loaded &&
                                        <p className="error">{ratingError}</p>
                                    }
                                </div>
                            }
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
