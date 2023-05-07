import { useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Ajax } from "../../../Ajax"
import Button, { ButtonTheme } from "../../../components/Button/Button";
import BookingDetails from "../../SchedulingPage/SchedulingComponents/BookingDetails/BookingDetails";

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
    PENDING = 2,
    MOTIFIED = 3,
    CANCELLED = 4,
}

const Filters: {
    [type in BookingStatus]: string
} = {
    [BookingStatus.CONFIRMED]: "Confirmed",
    [BookingStatus.PENDING]: "Pending",
    [BookingStatus.MOTIFIED]: "Motified",
    [BookingStatus.CANCELLED]: "Cancelled",
}

const BookingHistoryView: React.FC<IBookingHistoryProps> = (props) => {
    const navigate = useNavigate();
    const [data, setData] = useState<IBookingHistoryData[]>([]);
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(false);
    const [showDetails, setShowDetails] = useState(false);
    const [showBookingId, setShowBookingId] = useState(0);
    const [selectBooking, setSelectedBooking] = useState<number[]>([]);
    const prevDataRef = useRef<IBookingHistoryData[]>();

    const getData = async () => {
        await Ajax.get<IBookingHistoryData[]>("/accountsystem/getbookinghistory").then((response) => {
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
                <td>{bookingHistoryData.title}</td>
                <td>{bookingHistoryData.listingId}</td>
                <td>{bookingHistoryData.location}</td>
                <td>{bookingHistoryData.fullPrice}</td>
                <td>{BookingStatus[bookingHistoryData.bookingStatusId]}</td>
                <td className="view-btn-cell">
                    {bookingHistoryData.bookingStatusId == BookingStatus.CONFIRMED &&
                        <div className="view-btn-wrapper">
                            <Button theme={ButtonTheme.DARK}
                                onClick={() => {
                                    navigate("/bookingdetails", {
                                        state: {
                                            bookingId: bookingId,
                                            listingTitle: bookingHistoryData.title,
                                            listingLocation: bookingHistoryData.location,
                                            fullPrice: bookingHistoryData.fullPrice,
                                        }
                                    })
                                }}
                                title={"View"}
                            />
                        </div>
                    }
                </td>
            </tr>
        );
    }

    return (
        <div className="booking-history-container">
            <div className="booking-history-content">
                {!showDetails &&
                    <div className="booking-history-wrapper">
                        <h1>Booking History</h1>
                        <table>
                            <thead>
                                <tr>
                                    <th></th>
                                    <th>Title</th>
                                    <th>Listing Id</th>
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
            {error &&
                <p className="error">{error}</p>
            }
        </div>
    )
}

export default BookingHistoryView;