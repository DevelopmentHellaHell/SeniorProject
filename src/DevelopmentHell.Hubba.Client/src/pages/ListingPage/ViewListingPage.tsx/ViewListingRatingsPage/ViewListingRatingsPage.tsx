import { useState, useEffect } from "react";
import { IListing } from "../../../ListingProfilePage/MyListingsView/MyListingsView";
import NavbarUser from "../../../../components/NavbarUser/NavbarUser";
import { useLocation, useNavigate } from "react-router-dom";
import { Auth } from "../../../../Auth";
import NavbarGuest from "../../../../components/NavbarGuest/NavbarGuest";
import ListingRatingCard from "./ListingRatingCard/ListingRatingCard";
import Button, { ButtonTheme } from "../../../../components/Button/Button";
import { IRating, IViewListingData } from "../ViewListingPage";
import { Ajax } from "../../../../Ajax";
import './ViewListingRatingsPage.css';

interface IViewListingRatingsProps {

}


const ViewListingRatingsPage: React.FC<IViewListingRatingsProps> = (props) => {
    const [error, setError] = useState<string | undefined>(undefined);
    const [loaded, setLoaded] = useState<boolean>(false);
    const authData = Auth.getAccessData();
    const { state } = useLocation();
    const [hasBooked, setHasBooked] = useState<boolean>(false);
    const [hasRating, setHasRating] = useState<boolean>(false);
    const [isInputOpen, setIsInputOpen] = useState(false);
    const [data, setData] = useState<IViewListingData | null>(null);
    const [rating, setRating] = useState<IRating>({
        listingId: state.listingId,
        userId: Number(authData?.sub),
        rating: 0,
        comment: "",
        anonymous: true,
        lastEdited: new Date(),
      });

    useEffect(() => {
        const getData = async () => {
            const response = await Ajax.post<IViewListingData>('/listingprofile/viewListing', { listingId: state.listingId });
            if (response.data) {
                setData(response.data)
            }
            if (response.error) {
                setError(response.error)
            }
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
            if (response.error) {
                setError(response.error);
            }
            setLoaded(response.loaded);
            };

        getData();
        }, []);

    const handleButtonClick = () => {
        setIsInputOpen(true);
    };

    useEffect(() => {
        if (error !== undefined) {
          alert(error);
          setError(undefined);
        }
        
      }, [error]);
    
    const handleAddSubmit = async () => {
        setIsInputOpen(false);
        const response = await Ajax.post<null>("/listingprofile/addRating", { ListingId: state.listingId, Rating: rating.rating, Comment: rating.comment, Anonymous: rating.anonymous });
        if (response.error) {
            setError(response.error);
            return;
        }
        window.location.reload();
    };

    const handleEditSubmit = async () => {
        setIsInputOpen(false);
        const response = await Ajax.post<null>('/listingprofile/editRating', { ListingId: rating.listingId, Rating: rating.rating, Comment: rating.comment, Anonymous: rating.anonymous })
        console.log(response)
        if (response.error) {
            setError(response.error);
            return;
        }
        window.location.reload();
    };

    const handleDeleteClick = async () => {
        const response = await Ajax.post<null>('/listingprofile/deleteRating', { ListingId: rating.listingId })
        console.log(response)
        if (response.error) {
            setError(response.error);
            return;
        }
        window.location.reload();
    };

    const navigate = useNavigate();
    return (
        <div className="listing-ratings-container">
            {authData && authData.role !== Auth.Roles.DEFAULT_USER  ? 
                <NavbarUser /> : <NavbarGuest /> 
            }
            <div className="listing-ratings-view">
                <h2>Listing: { data?.Listing.title } </h2>
                    <div>
                        <Button theme={ButtonTheme.DARK} onClick={() => { navigate("/viewlisting", { state: { listingId: state.listingId }})} } title={"Back"} />
                        {authData && authData.role !== Auth.Roles.DEFAULT_USER && hasBooked && !hasRating && (
                            <>
                                <Button theme={ButtonTheme.DARK} onClick={() => { setIsInputOpen(true); }} title={"Add Rating"} />
                                {isInputOpen && (
                                <div className="modal">
                                    <form onSubmit={handleAddSubmit}>
                                    <input type="number" name="rating" min="0" max="5" placeholder="5" value={rating.rating} onChange={(e) => setRating({
                                            ...rating,
                                            rating: parseInt(e.target.value),
                                            })
                                        }
                                        />
                                        <input type="text" name="comment" maxLength={250} placeholder="Comment" value={rating.comment} onChange={(e) => setRating({
                                            ...rating,
                                            comment: e.target.value,
                                            })
                                        }
                                        /><label>Hide username: </label>
                                        <input type="checkbox" name="anonymous" id="anonymous" checked={rating.anonymous} onChange={(e) => setRating({
                                            ...rating,
                                            anonymous: e.target.checked,
                                            })
                                        }
                                        />
                                    <button type="submit">Submit</button>
                                    </form>
                                </div>
                                )}
                            </>
                            )}
                                {authData && authData.role !== Auth.Roles.DEFAULT_USER && hasBooked && hasRating && (
                                <>
                                    <Button theme={ButtonTheme.DARK} onClick={() => { setIsInputOpen(true); }} title={"Edit Rating"} /> <Button theme={ButtonTheme.DARK} onClick={() => { handleDeleteClick() }} title={"Delete Rating"} />
                                    {isInputOpen && (
                                    <div className="modal">
                                        <form onSubmit={handleEditSubmit}>
                                        <input
                                            type="number"
                                            name="rating"
                                            min="0"
                                            max="5"
                                            placeholder="5"
                                            value={rating.rating}
                                            onChange={(e) =>
                                                setRating({
                                                ...rating,
                                                rating: parseInt(e.target.value),
                                                })
                                            }
                                            />
                                            <input
                                            type="text"
                                            name="comment"
                                            maxLength={250}
                                            placeholder="Comment"
                                            value={rating.comment}
                                            onChange={(e) =>
                                                setRating({
                                                ...rating,
                                                comment: e.target.value,
                                                })
                                            }
                                            />
                                            <input
                                            type="checkbox"
                                            name="anonymous"
                                            id="anonymous"
                                            checked={rating.anonymous}
                                            onChange={(e) =>
                                                setRating({
                                                ...rating,
                                                anonymous: e.target.checked,
                                                })
                                            }
                                            />
                                        <button type="submit">Submit</button>
                                        </form>
                                        
                                    </div>
                                    )}
                                </>
                                )}
                </div>
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
    )
}

export default ViewListingRatingsPage;