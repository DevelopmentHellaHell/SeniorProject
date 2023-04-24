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
            setError("Listing failed to load. Refresh page or try again later.\n" +response.error);
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
            <NavbarUser /> 
            <div className="listing-content">
                <div className="listing-wrapper">
                    { data && !error && loaded &&
                        <div className="listing-page">
                            <div className="listing-page-status"> { isPublished ? 'Public' : 'Draft'  } </div> 
                            <p> { isPublished && authData?.sub==data.Listing.ownerId.toString() && <div>
                                    <Button theme={ButtonTheme.DARK} onClick={() => { navigate("/editlisting", { state: { listingId: data.Listing.listingId }})} } title={"Edit Listing"} />
                                    <Button theme={ButtonTheme.DARK} onClick={async () => { 
                                        const response = await Ajax.post("/listingprofile/unpublishListing", { listingId: data.Listing.listingId })
                                        if (response.error) {
                                            setError("Publishing listing error. Refresh page or try again later.\n" + response.error);
                                            return;
                                        }
                                        setIsPublished(false);
                                    }} title={"Unpublish Listing"} />
                                    <Button theme={ButtonTheme.DARK} onClick={() => { handleDeleteClick() }} title={"Delete Listing"} />
                                </div>}
                                { !isPublished && authData?.sub==data.Listing.ownerId.toString() && <div>
                                    <Button theme={ButtonTheme.DARK} onClick={() => { navigate("/editlisting", { state: { listingId: data.Listing.listingId }})} } title={"Edit Listing"} />
                                    <Button theme={ButtonTheme.DARK} onClick={() => { handleDeleteClick() }} title={"Delete Listing"} />
                                </div>} 
                             </p>
                            <h2 className="listing-page__title">{data.Listing.title}</h2>
                            <h3 className="listing-username">{data.Listing.ownerUsername}</h3>
                            {data.Files && data.Files.length > 0 && (
                                <div className="listing-page__image-wrapper">
                                    { data!.Files![currentImage].toString().substring(data!.Files![currentImage].toString().lastIndexOf('/') + 1) } {currentImage + 1} / {data!.Files!.length}
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
                            <p>{"Location: " + (data.Listing.location ?? "")}</p> 
                            <p>{"Price: " + (data.Listing.price ?? "")}</p>
                            <p>{"Description: " + (data.Listing.description ?? "")}</p>
                            <div>
                                <table>
                                    <thead>
                                        <tr>
                                            <th>StartTime</th>
                                            <th>EndTime</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        { data && data.Availabilities && data.Availabilities.map((value: IAvailability) => {
                                            return <ListingAvailabilityCard key={`${value.listingId}-listing-card`} availability={value}/>
                                        })}
                                    </tbody>
                                </table>
                            </div>

                            <Button theme={ButtonTheme.DARK} onClick={() => { 
                                navigate("/viewlistingratings", 
                                {
                                    state : 
                                    { 
                                        listingId: state.listingId, 
                                    }
                                });
                                }} title={"View Ratings"} />
                        </div>
                    }
                    {error && loaded &&
                        <p className="error">{error}</p>
                    }   
                </div>
            </div>
        <Footer />
        </div>
    );
}

export default ViewListingPage;
