import { useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Ajax } from "../../../Ajax"
import Button, { ButtonTheme } from "../../../components/Button/Button";
import BookingDetails from "../../SchedulingPage/SchedulingComponents/BookingDetails/BookingDetails";
import "./BookingHistoryView.css";
interface IBookingHistoryProps {

}

export interface IBookingHistoryData {
    bookingId: number,
    listingId: number,
    fullPrice: number,
    bookingStatusId: number,
    title: string,
    location: string,
}

enum BookingStatus {
    CONFIRMED = 1,
    // PENDING = 2,
    // MODIFIED = 3,
    CANCELLED = 4,
}

const Filters: {
    [type in BookingStatus]: string
} = {
    [BookingStatus.CONFIRMED]: "Confirmed",
    // [BookingStatus.PENDING]: "Pending",
    // [BookingStatus.MODIFIED]: "Modified",
    [BookingStatus.CANCELLED]: "Cancelled",
}

const BookingHistoryView: React.FC<IBookingHistoryProps> = (props) => {
    const localeUSLanguage: string = "en-US";
    const localeUSCurrency: object = { style: "currency", currency: "USD" };
    const localeUSTime: object = { hour: "2-digit", minute: "2-digit" };

    const navigate = useNavigate();
    const [data, setData] = useState<IBookingHistoryData[]>([]);
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(false);
    const [showDetails, setShowDetails] = useState(false);
    const [filters, setFilters] = useState<BookingStatus[]>([]);
    const [showBookingId, setShowBookingId] = useState(0);


    const [bookingPage, setBookingPage] = useState(1);
    const [bookingCount, setBookingCount] = useState(10);

    const [selectBooking, setSelectedBooking] = useState<number[]>([]);
    const prevDataRef = useRef<IBookingHistoryData[]>();

    const getData = async () => {
        await Ajax.post<IBookingHistoryData[]>("/accountsystem/getbookinghistory", { bookingCount: bookingCount, page: bookingPage }).then((response) => {
            setData(response.data && response.data.length ? response.data : []);
            setError(response.error);
            setLoaded(response.loaded);
        });
    }

    useEffect(() => {
        getData();
    }, []);

    useEffect(() => {
        prevDataRef.current = data;
    }, [data]);

    const createBookingHistoryTableRow = (bookingHistoryData: IBookingHistoryData) => {
        const bookingId = bookingHistoryData.bookingId;
        return (
            <tr key={`booking-${bookingHistoryData.bookingId}`}>
                <td className="select-booking-button-cancel">
                    {data.find(datapoint => datapoint.bookingId == bookingId)?.bookingStatusId == BookingStatus.CONFIRMED &&
                        <Button theme={selectBooking.includes(bookingId) ? ButtonTheme.DARK : ButtonTheme.HOLLOW_DARK} onClick={() => {
                            if (selectBooking.includes(bookingId)) {
                                selectBooking.splice(selectBooking.indexOf(bookingId), 1);
                                setSelectedBooking([...selectBooking]);
                                return;
                            }
                            setSelectedBooking([...selectBooking, bookingId]);
                        }} title={""} />}
                </td>
                <td>{bookingHistoryData.bookingId}</td>
                <td className="table-listing-title" onClick={() => {navigate('/viewListing', { state: { listingId: bookingHistoryData.listingId} })}}>
                    <p>{bookingHistoryData.title}</p>
                </td>
                <td>{bookingHistoryData.location}</td>
                <td>{bookingHistoryData.fullPrice.toLocaleString(localeUSLanguage, localeUSCurrency)}</td>
                <td>{BookingStatus[bookingHistoryData.bookingStatusId]}</td>
                <td className="view-btn-cell">
                    {bookingHistoryData.bookingStatusId == BookingStatus.CONFIRMED &&
                        <div className="view-btn-wrapper">
                            <Button theme={ButtonTheme.DARK}
                                onClick={() => {
                                    navigate("/bookingdetails", {
                                        state: {
                                            bookingId: bookingId,
                                            listingId: bookingHistoryData.listingId,
                                            listingTitle: bookingHistoryData.title,
                                            listingLocation: bookingHistoryData.location,
                                            fullPrice: bookingHistoryData.fullPrice,
                                        }
                                    })
                                }}
                                title={"View Booking"}
                            />
                        </div>
                    }
                </td>
            </tr>
        );
    }

    const createFilterButton = (filter: BookingStatus) => {
        return (
            <div key={`${filter}-filter`}>
                <Button theme={filters.includes(filter) ? ButtonTheme.DARK : ButtonTheme.HOLLOW_DARK} title={Filters[filter]} onClick={() => {
                    if (filters.includes(filter)) {
                        filters.splice(filters.indexOf(filter), 1);
                        setFilters([...filters]);
                        return;
                    }

                    setFilters([...filters, filter]);
                }} />
            </div>
        );
    }

    return (
        <div className="booking-history-container">
            <div className="booking-history-content">
                {!showDetails &&
                    <div className="booking-history-wrapper">
                        <h1>Booking History</h1>
                        <div className="filters-wrapper">
                            <div className="filters">
                                <p>Filters:</p>
                                {filters.length > 0 &&
                                    <Button title="Clear" theme={ButtonTheme.DARK} onClick={() =>
                                        setFilters([])} />
                                }
                                {Object.keys(Filters).map(key => {
                                    return createFilterButton(+key as BookingStatus);
                                })}
                            </div>
                        </div>
                        <table>
                            <thead>
                                <tr>
                                    <th></th>
                                    <th>Confirmation #</th>
                                    <th>Title</th>
                                    <th>Location</th>
                                    <th>Full Price</th>
                                    <th>Status</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                {data.length == 0 &&
                                    <tr>
                                        <td></td>
                                        <td>You have no bookings in your history. Start booking!</td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                }
                                {loaded && data && data.map(value => {
                                    if (filters.length > 0 && !filters.includes(value.bookingStatusId)) return <></>;
                                    return createBookingHistoryTableRow(value);
                                })}
                            </tbody>
                        </table>

                        <div className="actions-wrapper">
                            <div className="actions">

                                <Button theme={(selectBooking.length) > 0 ? ButtonTheme.DARK : ButtonTheme.HOLLOW_DARK} title="Cancel Booking"
                                    onClick={async () => {
                                        if (selectBooking.length == 0) {
                                            setError("Select Scheduled Booking to Cancel");
                                            return;
                                        }
                                        if (selectBooking.length > 1) {
                                            setError("You can only select one Booking to Cancel at a time.");
                                            return;
                                        }
                                        alert("Warning: Once cancellation occurs, it cannot be reverted. ");
                                        const response = await Ajax.post("accountsystem/cancelbooking", { bookingId: selectBooking[0] });
                                        if (!response.error) {
                                            setData([]);
                                            setSelectedBooking([]);
                                            getData();
                                        } else {
                                            setError(response.error);
                                        }
                                    }} />
                            </div>
                        </div>

                    </div>
                }
            </div>
            <div className="booking-control">
                <div className="booking-count-control">
                    <p>Bookings per page:</p>
                    <div className="h-stack">
                        <Button theme={bookingCount == 10 ? ButtonTheme.DARK : ButtonTheme.HOLLOW_DARK} title="10"
                            onClick={() => {
                                if (bookingCount != 10) {
                                    setLoaded(false);
                                    setBookingPage(1);
                                    setBookingCount(10);
                                    getData();
                                }
                            }}></Button>
                        <Button theme={bookingCount == 20 ? ButtonTheme.DARK : ButtonTheme.HOLLOW_DARK} title="20"
                            onClick={() => {
                                if (bookingCount != 20) {
                                    setLoaded(false);
                                    setBookingPage(1);
                                    setBookingCount(20);
                                    getData();
                                }
                            }} />
                        <Button theme={bookingCount == 50 ? ButtonTheme.DARK : ButtonTheme.HOLLOW_DARK} title="50"
                            onClick={() => {
                                if (bookingCount != 50) {
                                    setLoaded(false);
                                    setBookingPage(1);
                                    setBookingCount(50);
                                    getData();
                                }
                            }} />
                    </div>
                    <div className="booking-page-control">
                        <p>Page #{bookingPage}:</p>
                        <div className="h-stack">
                            {bookingPage > 1 &&
                                <Button theme={ButtonTheme.DARK} title="prev" onClick={() => {
                                    setBookingPage(bookingPage - 1);
                                    getData();
                                }}></Button>
                            }
                            {data.length == bookingCount &&
                                <Button theme={ButtonTheme.DARK} title="next" onClick={() => {
                                    setBookingPage(bookingPage + 1);
                                    getData();
                                }} />
                            }
                        </div>
                    </div>
                </div>
            </div>
            {error &&
                <p className="error">{error}</p>
            }
        </div>
    )
}

export default BookingHistoryView;