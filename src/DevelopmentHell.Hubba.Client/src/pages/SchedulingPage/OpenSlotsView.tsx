import React, { useEffect, useState } from 'react';
import { useLocation} from 'react-router-dom';
import Button, { ButtonTheme } from '../../components/Button/Button';
import Footer from '../../components/Footer/Footer';
import NavbarGuest from '../../components/NavbarGuest/NavbarGuest';
import { Ajax } from '../../Ajax';
import { Auth } from '../../Auth';
import NavbarUser from '../../components/NavbarUser/NavbarUser';
import DateButton, { DateButtonTheme } from './SchedulingComponents/DateButton/DateButton';
import HourBarButton, { HourBarButtonTheme } from './SchedulingComponents/HourBarButton/HourBarButton';
import "./OpenSlotsView.css";

const findListingAvailabilityRoute: string = "/scheduling/findlistingavailabilitybymonth/";
const reserveRoute: string = "/scheduling/reserve";
const localeUSLanguage: string = "en-US";
const localeUSCurrency: object = { style: "currency", currency: "USD" };
const localeUSTime: object = { hour: "2-digit", minute: "2-digit"};
const defaultTax: number = 0.0785;
const defaultFee: number = 0.15;

interface IOpenTimeSlotsProp {

}

interface IListingAvailabilityData {
    listingId: number,
    ownerId: number,
    availabilityId: number,
    startTime: Date,
    endTime: Date
}

export interface IBookedTimeFrame {
    availabilityId: number,
    startDateTime: Date,
    endDateTime: Date
}

interface IReservedTimeSlots {
    userId: number,
    listingId: number,
    listingTile: string,
    fullPrice: number,
    chosenTimeFrames: IBookedTimeFrame[]
}

interface IBookingDetails {
    userId: number,
    bookingId: number,
    fullPrice: number,
    bookedTimeFrames: IBookedTimeFrame
}

const OpenSlotsView: React.FC<IOpenTimeSlotsProp> = (props) => {
    const { state } = useLocation();
    const [bookingView, setBookingView] = useState<IBookingDetails | null>(null);

    const [listingId, setListingId] = useState<number>(state.listingId);
    const [initialDate, setInitialDate] = useState<string>("");

    const [reserveTimeSlots, setReserveTimeSlots] = useState<IReservedTimeSlots | null>(null);

    const [chosenDate, setChosenDate] = useState<string>("");
    const [bookedTimeFrames, setBookedTimeFrames] = useState<IBookedTimeFrame[]>([]);
    const [listingAvailabilityData, setListingAvailabilityData] = useState<IListingAvailabilityData[] | null>(null);

    const [selectedHours, setSelectedHours] = useState<Date[]>([]);
    const [fullPrice, setFullPrice] = useState<number>((bookedTimeFrames.length / 2) * state.price);
    const [fee, setFee] = useState<number>(state.fee ?? defaultFee);
    const [tax, setTax] = useState<number>(state.tax ?? defaultTax);

    const [readyToSubmit, setReadyToSubmit] = useState<boolean>(false);
    const [loaded, setLoaded] = useState<boolean>(false);
    const [onSuccess, setOnSuccess] = useState<boolean>(false);
    const [sidebarError, setSideBarError] = useState<string>("");
    const [error, setError] = useState<string>("");
    const [showConfirmation, setShowConfirmation] = useState<boolean>(false);
    const [onDoneBooking, setOnDoneBooking] = useState<boolean>(false);

    const authData = Auth.getAuthData();

    useEffect(() => {
        setReserveTimeSlots(null);
        setBookedTimeFrames([]);
        setSelectedHours([]);
        setReadyToSubmit(false);
        setOnSuccess(false);
        setOnDoneBooking(false);
        setSideBarError("");
        setError("");
    }, [initialDate, chosenDate]);

    useEffect(() => {
        setShowConfirmation(false);
        setChosenDate("");
        setBookingView(null);
        setError("");
    }, [initialDate, onDoneBooking, loaded]);

    useEffect(() => {
        setFullPrice((bookedTimeFrames.length / 2) * state.price);
    }, [bookedTimeFrames])

    useEffect(() => {
        if (!readyToSubmit) {
            return;
        }
        const total = bookedTimeFrames.length * state.price + (bookedTimeFrames.length * state.price * tax) + bookedTimeFrames.length * state.price * fee;
        setReserveTimeSlots({
            userId: authData!.sub,
            listingId: listingId,
            listingTile: state.listingTitle,
            fullPrice: total,
            chosenTimeFrames: bookedTimeFrames
        } as IReservedTimeSlots);
    }, [readyToSubmit, selectedHours]);

    const onError = (message: string) => {
        setError(message);
        setLoaded(true);
    }
    const onSideBarError = (message: string) => {
        setSideBarError(message);
        setLoaded(true);
    };

    const getListingAvailabilityData = async (year: number, month: number) => {
        if (month && year) {
            await Ajax.post(findListingAvailabilityRoute, {
                listingId: listingId,
                month: month,
                year: year
            }).then(response => {
                if (response.error) {
                    onSideBarError(response.error);
                    return;
                }
                {
                    !listingAvailabilityData &&
                        <div className='info'>
                            No open time slots found
                        </div>
                }
                const data = response.data as IListingAvailabilityData[];
                const listingAvailability: IListingAvailabilityData[] = data.map(item => ({
                    listingId: item.listingId,
                    ownerId: item.ownerId,
                    availabilityId: item.availabilityId,
                    startTime: item.startTime,
                    endTime: item.endTime
                }));
                setLoaded(true);
                setListingAvailabilityData(listingAvailability);
            });
        }
    };

    useEffect(() => {
        const today: Date = new Date();
        getListingAvailabilityData(today.getFullYear(), today.getMonth() + 1);
        setInitialDate(today.toDateString());
    },[])
    
    const convertToMonthName = (initialDate: number) => {
        switch (initialDate) {
            case 1: return "January";
            case 2: return "February";
            case 3: return "March";
            case 4: return "April";
            case 5: return "May";
            case 6: return "June";
            case 7: return "July";
            case 8: return "August";
            case 9: return "September";
            case 10: return "October";
            case 11: return "November";
            case 12: return "December";
        };
    };

    const convertToDayName = (index: number) => {
        switch (index) {
            case 0: return "SUN";
            case 1: return "MON";
            case 2: return "TUE";
            case 3: return "WED";
            case 4: return "THU";
            case 5: return "FRI";
            case 6: return "SAT";
        }
    }

    const renderHourBars = (date: string) => {
        if (!date || !uniqueDates) { return; }
        const matchingAvailabilities = listingAvailabilityData?.filter(availability => {
            return (new Date(availability.startTime).toDateString() == new Date(date).toDateString()) as boolean
        }).map(availability => {
            return {
                availabilityId: availability.availabilityId,
                startDateTime: availability.startTime,
                endDateTime: availability.endTime
            } as IBookedTimeFrame
        });
        const renderHalfDay = (slotsArray: Date[]) => {
            return <>
                {slotsArray.map((halfhour) => {
                    let matchingTimeSlot: IBookedTimeFrame | undefined = undefined;
                    matchingAvailabilities?.forEach(timeSlot => {
                        if (new Date(timeSlot.endDateTime).getTime() > halfhour.getTime() && new Date(timeSlot.startDateTime).getTime() <= halfhour.getTime()) {
                            matchingTimeSlot = timeSlot;
                        }
                    })
                    let theme = matchingTimeSlot ? HourBarButtonTheme.LIGHT : HourBarButtonTheme.GREY;
                    theme = selectedHours.some(date => date.getTime() == halfhour.getTime()) ? HourBarButtonTheme.DARK : theme;

                    return <>
                        <div className='buttons'>
                            <HourBarButton key={halfhour.toLocaleTimeString()} theme={theme} title={halfhour.toLocaleTimeString(localeUSLanguage, localeUSTime)} onClick={() => {
                                if (authData && authData.role !== Auth.Roles.DEFAULT_USER) {
                                    if (matchingTimeSlot) {
                                        const startTime = new Date(date);
                                        startTime.setHours(halfhour.getHours(), halfhour.getMinutes());
                                        const endTime = new Date(date);
                                        endTime.setHours(halfhour.getHours(), halfhour.getMinutes() + 30);
                                        const selectedBookedTimeFrame: IBookedTimeFrame = {
                                            availabilityId: matchingTimeSlot!.availabilityId,
                                            startDateTime: startTime,
                                            endDateTime: endTime
                                        };

                                        if (!selectedHours.some(date => date.getTime() == halfhour.getTime())) {
                                            setBookedTimeFrames((previous) => [...previous, selectedBookedTimeFrame]);
                                            setSelectedHours((previous) => [...previous, halfhour]);
                                        } else {
                                            setBookedTimeFrames((previous) => [...(previous.filter(timeFrame => new Date(selectedBookedTimeFrame.startDateTime).getTime() !== new Date(timeFrame.startDateTime).getTime()))]);
                                            setSelectedHours((previous) => [...(previous.filter(hour => hour.getTime() !== halfhour.getTime()))]);
                                        }

                                        setReadyToSubmit(true);
                                    }
                                } else {
                                    onError("You must be logged in to make selection.");
                                }
                            }} />
                        </div>
                    </>
                })}
            </>
        };

        if (uniqueDates.includes(date)) {
            const dayStart = new Date(date);
            dayStart.setHours(0, 0, 0, 0);

            const hoursArrayAM: Date[] = [];
            for (let i = 0; i < 24; i++) { // 24 half-hour time slots in half-day
                const newTimeSlot = new Date(dayStart.getTime() + (i * 30 * 60 * 1000));
                hoursArrayAM.push(newTimeSlot);
            }

            const hoursArrayPM: Date[] = [];
            for (let i = 24; i < 48; i++) { // 24 half-hour time slots in half-day
                const newTimeSlot = new Date(dayStart.getTime() + (i * 30 * 60 * 1000));
                hoursArrayPM.push(newTimeSlot);
            }
            return <>
                {error &&
                    <div className='error'>
                        {error}
                    </div>}
                <div className='halfdaywrapper'>
                    <div className='halfday'>
                        <h4> AM </h4>
                        {renderHalfDay(hoursArrayAM)}
                    </div>
                    <div className='halfday'>
                        <h4> PM </h4>
                        {renderHalfDay(hoursArrayPM)}
                    </div>
                </div>
            </>
        }
        {
            error &&
                <div className='error'>
                    {error}
                </div>
        }
    };

    const uniqueDates: string[] = Array.from(new Set(listingAvailabilityData?.map(data => (new Date(data.startTime)).toDateString()) ?? []));
    const renderDates = (dateString: string) => {
        let key = new Date(dateString).getFullYear() !== 1990 ? new Date(dateString).getDate().toString() : "";
        let theme = (uniqueDates.includes(dateString) && (new Date(dateString) >= new Date())) ? DateButtonTheme.LIGHT : DateButtonTheme.GREY;
        if (chosenDate == dateString && uniqueDates.includes(chosenDate)) {
            theme = DateButtonTheme.DARK;
        }
        return <>
            <div className='day'>
                <DateButton
                    id={key}
                    title={key}
                    theme={theme}
                    onClick={() => {

                        if (chosenDate !== dateString && uniqueDates.includes(dateString) && (new Date(dateString) >= new Date())) {
                            setChosenDate(dateString);
                        }
                        else {
                            setChosenDate("");
                        }
                    }} />
            </div>
        </>
    };

    const renderCalendar = (initialDate: string) => {
        const chosenYear = new Date(initialDate).getFullYear();
        const chosenMonth = new Date(initialDate).getMonth();
        const weeks: string[][] = [];
        const date: Date = new Date(chosenYear, chosenMonth, 1);
        while (date.getMonth() === chosenMonth) {
            const week: string[] = new Array(7);
            for (let dayOfWeek = 0; dayOfWeek < 7; dayOfWeek++) {
                if (date.getMonth() === chosenMonth && date.getDay() === dayOfWeek) {
                    week[dayOfWeek] = date.toDateString();
                    date.setDate(date.getDate() + 1);
                }
                else {
                    week[dayOfWeek] = new Date(1990, 1, 1).toDateString();
                }
            }
            weeks.push(week);
        }
        const daysOfWeek = Array.from(Array(7).keys());
        const todayButtonTheme = new Date(initialDate).getMonth() !== new Date().getMonth() ? HourBarButtonTheme.HOLLOW_DARK : HourBarButtonTheme.LIGHT
        return <>
            <div className='daysofweek'>
                {daysOfWeek.map((dayOfWeek => {
                    return <>
                        <div className='day'>
                            {convertToDayName(dayOfWeek)}
                        </div>
                    </>
                }))}
            </div>
            {weeks.map((week => {
                return <>
                    <div className='week'>
                        {week.map((day) => {
                            return <>
                                {renderDates(day)}
                            </>
                        })}
                    </div>
                </>
            }))}
            <div className='today'>
                <HourBarButton title="Today" theme={todayButtonTheme} onClick={() => {
                    const today = new Date();
                    setInitialDate(today.toDateString())
                    getListingAvailabilityData(today.getFullYear(), today.getMonth() + 1)
                }} />
            </div>
        </>;
    }

    const renderSummary = (bookedTimeFrames: IBookedTimeFrame[]) => {
        if (!bookedTimeFrames) {
            return;
        }
        const printedDate: string[] = Array.from(new Set(bookedTimeFrames.map((date) => date.startDateTime.toDateString()) ?? []));
        return <>
                {printedDate.map((date) => {
                    return <>
                        {/* display chosen date and hours */}
                        <div className='details'>
                            {new Date(date).toLocaleDateString(localeUSLanguage)} ({bookedTimeFrames.length / 2} hours)
                            {bookedTimeFrames.sort((a, b) => a.startDateTime.getTime() - b.startDateTime.getTime()).map((time) => {
                                return <>
                                    {time.startDateTime.toDateString() == date &&
                                        <div className='info'>
                                            + {time.startDateTime.toLocaleTimeString(localeUSLanguage, localeUSTime)} - {time.endDateTime.toLocaleTimeString(localeUSLanguage, localeUSTime)}
                                        </div>}
                                </>
                            })}
                        </div>
                        {/* display price */}
                        <div className='info-bold'>
                            <div className='info-left'>
                                <div className='block'>Hour(s)</div>
                                <div className='block'>Sub-Total</div>
                                <div className='block'>Tax</div>
                                <div className='block'>Fee</div>
                                <div className='block'>Total</div>
                            </div>
                            <div className='info-right'>
                                <div className='block'>{bookedTimeFrames.length / 2} hr(s)</div>
                                <div className='block'>{(fullPrice).toLocaleString(localeUSLanguage, localeUSCurrency)}</div>
                                <div className='block'>{(fullPrice * tax).toLocaleString(localeUSLanguage, localeUSCurrency)}</div>
                                <div className='block'>{(fullPrice * fee).toLocaleString(localeUSLanguage, localeUSCurrency)}</div>
                                <div className='block'>{(fullPrice + fullPrice * fee + fullPrice * tax).toLocaleString(localeUSLanguage, localeUSCurrency)}</div>
                            </div>
                        </div>
                    </>
                })}
        </>
    }

    const renderSidebar = (error: string) => {

        return (
            <div className="opentimeslots-sidebar">
                {!chosenDate &&
                    <p className='info'>Click '&lt;' or '&gt;' on the calendar to find listing's availability</p>
                }
                {chosenDate &&
                    <div>
                        Your Booking Details:
                    </div>
                }
                {sidebarError &&
                    <div className='error'>
                        {sidebarError}
                    </div>
                }
            </div>

        );
    };

    const renderConfirmation = () => {
        let confirmationHeader = !onDoneBooking ? "Confirm Your Booking" : "Thank Your For Your Booking";
        return <>
            <div className='confirmation-card'>

                <div className='h1'>{confirmationHeader}</div>
                {bookingView &&
                    <div className='h2'>
                        Confirmation number: {bookingView?.bookingId}
                    </div>
                }
                <div className='h2'>Listing: {state.listingTitle}</div>
                <p className='h2'>Booking details:</p>

                {renderSummary(bookedTimeFrames)}
                {!onSuccess && !error &&
                    <div className='buttons'>
                        <Button title="No" theme={ButtonTheme.HOLLOW_DARK} onClick={() => history.go(-1)} />
                        <Button title="Yes" theme={ButtonTheme.DARK} onClick={async () => {
                            if (authData && authData.role !== Auth.Roles.DEFAULT_USER) {
                                await Ajax.post(reserveRoute, {
                                    userId: reserveTimeSlots?.userId,
                                    listingId: reserveTimeSlots?.listingId,
                                    fullPrice: reserveTimeSlots?.fullPrice,
                                    chosenTimeFrames: reserveTimeSlots?.chosenTimeFrames
                                }).then(response => {
                                    if (response.error || response.data == null) {
                                        onError("Scheduling Error. Please refresh page or try again later.")
                                    }
                                    const data = response.data as IBookingDetails;
                                    if (data !== null) {
                                        setBookingView({
                                            userId: data?.userId,
                                            bookingId: data?.bookingId,
                                            fullPrice: data?.fullPrice,
                                            bookedTimeFrames: data?.bookedTimeFrames,
                                        });
                                        setOnSuccess(true);
                                    }
                                })
                            }
                            else {
                                onError("User must be logged in to make reservation.")
                            }
                        }} />
                    </div>
                }
                {error &&
                    <div className='buttons'>
                        <Button title="Close" theme={ButtonTheme.DARK} onClick={() => history.go(-1)} />
                    </div>
                }
                {onSuccess &&
                    <div className='info'>
                        <p> Booking confirmed. </p>
                        <div className='buttons'>
                            <Button title="Close" theme={ButtonTheme.DARK} onClick={() => {
                                setOnDoneBooking(true);
                            }} />
                        </div>
                    </div>
                }
                {error &&
                    <div className="error">
                        {error}
                    </div>}
            </div>
        </>
    }
    
    return (
        <div className="opentimeslots-view-container">
            {authData && authData.role !== Auth.Roles.DEFAULT_USER ?
                <NavbarUser /> : <NavbarGuest />
            }

            <div className="opentimeslots-view-content">
                {/* Render sidebar for selection summary */}
                {!showConfirmation &&
                    <div className='opentimeslots-sidebar-wrapper'>
                        {renderSidebar(sidebarError)}
                        {renderSummary(bookedTimeFrames)}
                        {readyToSubmit &&
                            <div className='buttons'>
                                <Button title="Reserve" theme={ButtonTheme.DARK} onClick={async () => {
                                    if (state.ownerId.toString() == authData?.sub) {
                                        onSideBarError("Owner can not book their own listing");
                                        return;
                                    }
                                    setFullPrice((bookedTimeFrames.length / 2) * state.price);
                                    setShowConfirmation(true);
                                }} />
                            </div>
                        }
                    </div>
                }

                {/** Render calendar and hour bars after chosing the date*/}
                {!showConfirmation && loaded &&
                    <div className="main-wrapper">
                        <h1>{state.listingTitle}</h1>
                        <h2>{state.price.toLocaleString(localeUSLanguage, localeUSCurrency)}/hour</h2>
                        <div className='second-wrapper'>
                            <div className="view-wrapper">
                                <div className='calendar'>
                                    <div className='header'>
                                        <Button title="<"
                                            onClick={() => {
                                                let prevDate = new Date(initialDate);
                                                const prevYear = prevDate.getFullYear();
                                                const prevMonth = prevDate.getMonth(); // January is 0
                                                setInitialDate(new Date(prevYear, prevMonth - 1, 1).toDateString());
                                                getListingAvailabilityData(prevYear, prevMonth);
                                            }}
                                        />
                                        <div className='text'>
                                            {convertToMonthName((new Date(initialDate)).getMonth() + 1)} {(new Date(initialDate)).getFullYear()}
                                        </div>
                                        <Button title=">"
                                            onClick={() => {
                                                let nextDate = new Date(initialDate);
                                                const nextYear = nextDate.getFullYear();
                                                const nextMonth = nextDate.getMonth(); // January is 0
                                                setInitialDate(new Date(nextYear, nextMonth + 1, 1).toDateString());
                                                getListingAvailabilityData(nextYear, nextMonth + 2);
                                            }}
                                        />
                                    </div>
                                    <div className='month'>
                                        {renderCalendar(initialDate)}
                                    </div>

                                </div>
                            </div>
                            <div className="slots-card">
                                <h3> Open Time Slots</h3>
                                {chosenDate == "" &&
                                    <div className='info'>
                                        Click on highlighted dates to see open time slots.
                                    </div>
                                }
                                {chosenDate !== "" && uniqueDates.includes(chosenDate) && renderHourBars(chosenDate)}
                            </div>
                        </div>
                    </div>
                }
                {/** Render confirmation card after clicking reserve button */}
                {showConfirmation &&
                    <div className='confirmation-card-wrapper'>
                        {renderConfirmation()}
                    </div>
                }
            </div>

            <Footer />
        </div>
    );
}

export default OpenSlotsView;