import { useState } from "react";
import { Auth } from "../../Auth";
import Footer from "../../components/Footer/Footer";
import NavbarGuest from "../../components/NavbarGuest/NavbarGuest";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import Sidebar from "../../components/Sidebar/Sidebar";
import MyListingsView from "./MyListingsView/MyListingsView";
import './ListingProfilePage.css';
import BookingHistoryView from "./BookingHistoryView/BookingHistoryView";


interface IListingProfilePageProps {

}

enum ListingProfileViews {
    MY_LISTINGS = "My Listings",
    BOOKING_HISTORY = "Booking History"
}

const ListingProfilePage: React.FC<IListingProfilePageProps> = (props) => {
    const [view, setView] = useState<ListingProfileViews>(ListingProfileViews.MY_LISTINGS);
    const authData = Auth.getAccessData();

    const renderView = (view: ListingProfileViews) => {
        switch(view) {
            case ListingProfileViews.MY_LISTINGS:
                return <MyListingsView />;
            case ListingProfileViews.BOOKING_HISTORY:
                return <BookingHistoryView/>;
        }
    }

    return (
        <div className="listingprofile-container">
            <NavbarUser /> 

            <div className="listingprofile-content">

                <Sidebar>
                    <li><p onClick={() => { setView(ListingProfileViews.MY_LISTINGS) }}> My Listings</p></li>
                    <li><p onClick={() => { setView(ListingProfileViews.BOOKING_HISTORY) }}> Booking History</p></li>
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