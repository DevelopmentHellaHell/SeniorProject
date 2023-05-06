import { useState } from "react";
import { Auth } from "../../Auth";
import Footer from "../../components/Footer/Footer";
import NavbarGuest from "../../components/NavbarGuest/NavbarGuest";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import Sidebar from "../../components/Sidebar/Sidebar";
import MyListingsView from "./MyListingsView/MyListingsView";
import './ListingProfilePage.css';
import ReservationView from "./ReservationView/ReservationView";


interface IListingProfilePageProps {

}

enum ListingProfileViews {
    MY_LISTINGS = "My Listings",
    RESERVATIONS = "Reservations",
}

const ListingProfilePage: React.FC<IListingProfilePageProps> = (props) => {
    const [view, setView] = useState<ListingProfileViews>(ListingProfileViews.MY_LISTINGS);
    const authData = Auth.getAccessData();

    const renderView = (view: ListingProfileViews) => {
        switch(view) {
            case ListingProfileViews.MY_LISTINGS:
                return <MyListingsView />;
            case ListingProfileViews.RESERVATIONS:
                return <ReservationView/>;
        }
    }

    return (
        <div className="listingprofile-container">
            <NavbarUser /> 

            <div className="listingprofile-content">

                <Sidebar>
                    <li><p onClick={() => { setView(ListingProfileViews.MY_LISTINGS) }}> My Listings</p></li>
                    <li><p onClick={() => { setView(ListingProfileViews.RESERVATIONS) }}> Reservation</p></li>
                </Sidebar>

                <div className="listingprofile-wrapper">
                    { renderView(view)}
                        
                    
                </div>
               
            </div>
            <Footer />
        </div>
    );
}

export default ListingProfilePage;