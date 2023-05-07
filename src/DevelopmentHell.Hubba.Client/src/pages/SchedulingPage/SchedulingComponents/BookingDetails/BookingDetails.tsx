import React, { useEffect, useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import "./BookingDetails.css";
import { Auth } from '../../../../Auth';
import { IBookedTimeFrame } from '../../OpenSlotsView';
import { Ajax } from '../../../../Ajax';
import Button, { ButtonTheme } from '../../../../components/Button/Button';
import { IBookingHistoryData } from '../../../AccountPage/BookingHistoryView/BookingHistoryView';
import NavbarUser from '../../../../components/NavbarUser/NavbarUser';
import NavbarGuest from '../../../../components/NavbarGuest/NavbarGuest';
import Footer from '../../../../components/Footer/Footer';
import { AccountViews } from '../../../AccountPage/AccountPage';

interface IBookingDetailsProp {
}

interface IBookingDetails {
    userId: number;
    bookingId: number;
    bookedTimeFrames: IBookedTimeFrame[];
}

const BookingDetails: React.FC<IBookingDetailsProp> = (props) => {
    const getBookingDetailsRoute: string = "/scheduling/getbookingdetails";
    const localeUSLanguage: string = "en-US";
    const localeUSCurrency: object = { style: "currency", currency: "USD" };
    const localeUSTime: object = { hour: "2-digit", minute: "2-digit" };
   
    const { state } = useLocation();
    const [loaded, setLoaded] = useState(false);
    const [refresh, setRefresh] = useState(false);
    const [error, setError] = useState("");
    const [bookingDetailsData, setBookingDetailsData] = useState<IBookingDetails | null>(null);

    const navigate = useNavigate();

    const authData = Auth.getAuthData();

    const onError = (message: string) => {
        setError(message);
        setLoaded(true);
    }
    const getBookingDetails = async () => {
        await Ajax.post(getBookingDetailsRoute, { userId: authData?.sub, bookingId: state.bookingId })
            .then(response => {
                if (response.error) {
                    onError(response.error);
                    return;
                }
                const data = response.data as IBookingDetails;
                setBookingDetailsData(data);
                setLoaded(true);
            });
    };
    useEffect(() => {
        getBookingDetails()
    }, [])

    const renderSummary = (bookedTimeFrames: IBookedTimeFrame[]) => {
        if (!bookedTimeFrames) {
            return;
        }
        const printedDate: string[] = Array.from(new Set(bookedTimeFrames.map((date) => new Date(date.startDateTime).toDateString()) ?? []));
        return <>
            {printedDate.map((date) => {
                return <>
                    {/* display chosen date and hours */}
                    <div className='details'>
                        {new Date(date).toLocaleDateString(localeUSLanguage)} ({bookedTimeFrames.length / 2} hours)
                        {bookedTimeFrames.sort((a, b) => new Date(a.startDateTime).getTime() - new Date(b.startDateTime).getTime()).map((time) => {
                            return <>
                                {new Date(time.startDateTime).toDateString() == date &&
                                    <div className='info'>
                                        + {new Date(time.startDateTime).toLocaleTimeString(localeUSLanguage, localeUSTime)} - {new Date(time.endDateTime).toLocaleTimeString(localeUSLanguage, localeUSTime)}
                                    </div>}
                            </>
                        })}
                    </div>
                    {/* display price */}
                    <div className='info-bold'>
                        <div className='info-left'>
                            <div className='block'>Hour(s)</div>
                            <div className='block'>Total</div>
                        </div>
                        <div className='info-right'>
                            <div className='block'>{bookedTimeFrames.length / 2} hr(s)</div>
                            <div className='block'>{(state.fullPrice).toLocaleString(localeUSLanguage, localeUSCurrency)}</div>
                        </div>
                    </div>
                </>
            })}
        </>
    }
    return (
        <div className='bookingdetails-view-container'>
            {authData && authData.role !== Auth.Roles.DEFAULT_USER ?
                <NavbarUser /> : <NavbarGuest />
            }

            <div className='bookingdetails-view-content'>
                <div className='bookingdetails-wrapper'>
                    <div className='confirmation-card'>
                        <div className='h1'>Booking Details</div>
                        <div className='h2'>
                            Confirmation number: {state.bookingId}
                        </div>
                        <div className='h2'>Listing: {state.listingTitle}</div>
                        {loaded && renderSummary(bookingDetailsData!.bookedTimeFrames)}
                        {error &&
                            <div className="error">
                                {error}
                            </div>}

                        <div className='buttons'>
                            <Button title="Close" theme={ButtonTheme.DARK} onClick={() => {
                                setRefresh(true);
                                navigate("/account", { state: { view: AccountViews.BOOKING_HISTORY }})
                            }} />
                        </div>
                    </div>
                </div>
            </div>

            <Footer />
        </div>

    );
};

export default BookingDetails;