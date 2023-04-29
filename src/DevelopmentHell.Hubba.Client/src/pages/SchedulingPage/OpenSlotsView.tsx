import React, { JSXElementConstructor, useEffect, useRef, useState } from 'react';
import Button, { ButtonTheme } from '../../components/Button/Button';
import Footer from '../../components/Footer/Footer';
import NavbarGuest from '../../components/NavbarGuest/NavbarGuest';
import { Ajax } from '../../Ajax';
import { Auth } from '../../Auth';
import NavbarUser from '../../components/NavbarUser/NavbarUser';
import DateButton, { DateButtonTheme } from './SchedulingComponents/DateButton';
import HourBarButton, { HourBarButtonTheme } from './SchedulingComponents/HourBarButton';
import "./OpenSlotsView.css";
import ConfirmationCard from './SchedulingComponents/ConfirmationCard';
import { initial } from 'cypress/types/lodash';

interface IOpenTimeSlotsProp {
    listingId: number,
    listingTitle: string,
    ownerId: number,
    price: number,
    fee?: number,
    tax?: number
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

export interface IReservedTimeSlots {
    userId: number,
    listingId: number,
    listingTile: string,
    fullPrice: number,
    chosenTimeFrames: IBookedTimeFrame[]
}

const OpenSlotsView: React.FC<IOpenTimeSlotsProp> = (props) => {
    const [listingId, setListingId] = useState(20);
    const [initialDate, setInitialDate] = useState((new Date()).toDateString());
    const [month, setMonth] = useState<number>((new Date(initialDate)).getMonth() + 1);
    const [year, setYear] = useState<number>(new Date(initialDate).getFullYear());

    const [reserveTimeSlots, setReserveTimeSlots] = useState<IReservedTimeSlots | null>(null);

    const [chosenDate, setChosenDate] = useState("");
    const [bookedTimeFrames, setBookedTimeFrames] = useState<IBookedTimeFrame[]>([]);
    const [listingAvailabilityData, setListingAvailabilityData] = useState<IListingAvailabilityData[] | null>(null);
    const [selectedHours, setSelectedHours] = useState<number[]>([]);
    const [fullPrice, setFullPrice] = useState(bookedTimeFrames.length * props.price);
    const [fee, setFee] = useState(props.fee || 10);
    const [tax, setTax] = useState(fullPrice * props.tax! || 0);

    const [readyToSubmit, setReadyToSubmit] = useState<boolean>(false);
    const [loaded, setLoaded] = useState(false);
    const [onSuccess, setOnSuccess] = useState(false);
    const [sidebarError, setSideBarError] = useState("");
    const [error, setError] = useState("");
    const [showConfirmation, setShowConfirmation] = useState(false);

    const authData = Auth.getAuthData();

    useEffect(() => {
        setReserveTimeSlots(null);
        setBookedTimeFrames([]);
        setSelectedHours([]);
        setReadyToSubmit(false);
        setOnSuccess(false);
        setSideBarError("");
        setError("");
    }, [initialDate, chosenDate, onSuccess]);

    useEffect(() => {
        setMonth((new Date(initialDate)).getMonth() + 1);
        setShowConfirmation(false);
        setChosenDate("");
        setListingAvailabilityData([]);
    }, [initialDate, onSuccess]);

    useEffect(() => {
        if (!readyToSubmit) {
            return;
        }

        setReserveTimeSlots({
            userId: parseInt(authData!.sub),
            listingId: listingId,
            listingTile: props.listingTitle,
            fullPrice: fullPrice,
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

    const getMonth = (initialDate: number) => {
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
    const getDay = (index: number) => {
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

        // console.log("DATE: " + date);
        // console.log("UNIQUEDATES:" + uniqueDates);
        const matchingAvailabilities = listingAvailabilityData?.filter(availability => {
            // console.log("\n AVAILABILITIES");
            // console.log(new Date(availability.startTime).toDateString());
            // console.log(new Date(date).toDateString());
            // console.log("MATCHING DATE:" + (new Date(availability.startTime).toDateString() == new Date(date).toDateString()));
            return (new Date(availability.startTime).toDateString() == new Date(date).toDateString()) as boolean
        }).map(availability => {
            return {
                availabilityId: availability.availabilityId,
                startDateTime: availability.startTime,
                endDateTime: availability.endTime
            } as IBookedTimeFrame
        });
        // console.log(matchingAvailabilities);
        const renderHalfDay = (slotsArray: number[]) => {
            return <>
                {slotsArray.map((index) => {
                    let matchingTimeSlot: IBookedTimeFrame | undefined = undefined;
                    matchingAvailabilities?.forEach(timeSlot => {
                        // console.log(new Date(timeSlot.endTime).getHours())
                        // console.log(new Date(timeSlot.startTime).getHours())
                        if (new Date(timeSlot.endDateTime).getHours() > index && new Date(timeSlot.startDateTime).getHours() <= index) {
                            matchingTimeSlot = timeSlot;
                        }
                    })
                    let theme = matchingTimeSlot ? HourBarButtonTheme.LIGHT : HourBarButtonTheme.GREY;
                    theme = selectedHours.includes(index) ? HourBarButtonTheme.DARK : theme;

                    return <>
                        <div className='buttons'>
                            <HourBarButton key={index} theme={theme} title={index.toString().concat(":00")} onClick={() => {
                                if (authData && authData.role !== Auth.Roles.DEFAULT_USER) {
                                    if (matchingTimeSlot) {
                                        const startTime = new Date(date);
                                        startTime.setHours(index);
                                        const endTime = new Date(date);
                                        endTime.setHours(index + 1);
                                        const selectedBookedTimeFrame: IBookedTimeFrame = {
                                            availabilityId: matchingTimeSlot!.availabilityId,
                                            startDateTime: startTime,
                                            endDateTime: endTime
                                        };
                                        if (!selectedHours.includes(index)) {
                                            setBookedTimeFrames((previous) => [...previous, selectedBookedTimeFrame]);
                                            setSelectedHours((previous) => [...previous, index]);
                                        } else {
                                            setBookedTimeFrames((previous) => [...(previous.filter(timeFrame => new Date(selectedBookedTimeFrame.startDateTime).getHours() !== new Date(timeFrame.startDateTime).getHours()))]);
                                            setSelectedHours((previous) => [...(previous.filter(hour => hour !== index))]);
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
            const hoursArrayAM = Array.from(Array(12).keys());
            const hoursArrayPM = Array.from({ length: 12 }, (_, i) => i + 12);
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
        else {
            return <>
                {chosenDate == "" &&
                    <div className='info'>
                        Select a date to check open slots
                    </div>
                }
            </>
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
                    id={dateString}
                    title={key}
                    theme={theme}
                    onClick={() => {

                        if (chosenDate !== dateString && uniqueDates.includes(dateString)) {
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
        const chosenMonth = new Date(initialDate).getMonth();
        const weeks: string[][] = [];
        const date: Date = new Date(year, chosenMonth, 1);
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
                            {getDay(dayOfWeek)}
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
                    setInitialDate(new Date().toDateString())
                }} />
            </div>
        </>;
    }
    const renderSummary = (bookedTimeFrames: IBookedTimeFrame[], error: string) => {
        if (!bookedTimeFrames || error) {
            return;
        }
        const printedDate: string[] = Array.from(new Set(bookedTimeFrames.map((date) => date.startDateTime.toDateString()) ?? []));
        return <>
            <div className='opentimeslots-sidebar'>
                {printedDate.map((date) => {
                    return <>
                        {/* display chosen date and hours */}
                        <div className='details'>
                            {new Date(date).toLocaleDateString()} ({bookedTimeFrames.length} hours)
                            {bookedTimeFrames.sort((a, b) => a.startDateTime.getHours() - b.startDateTime.getHours()).map((time) => {
                                return <>
                                    {time.startDateTime.toDateString() == date &&
                                        <div className='info'>
                                            + {time.startDateTime.toLocaleTimeString()} - {time.endDateTime.toLocaleTimeString()}
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
                                <div className='block'>{reserveTimeSlots?.chosenTimeFrames.length} hr(s)</div>
                                <div className='block'>{(bookedTimeFrames.length * props.price).toLocaleString("en-US", { style: 'currency', currency: 'USD' })}</div>
                                <div className='block'>{tax.toLocaleString("en-US", { style: 'currency', currency: 'USD' })}</div>
                                <div className='block'>{fee.toLocaleString("en-US", { style: 'currency', currency: 'USD' })}</div>
                                <div className='block'>{(bookedTimeFrames.length * props.price + fee + tax).toLocaleString("en-US", { style: 'currency', currency: 'USD' })}</div>
                            </div>
                        </div>
                        {/* render Confirmation card */}
                    </>
                })}
            </div>
        </>
    }
    const renderSidebar = () => {
        return (
            <div className="opentimeslots-sidebar">
                <div className="input-field">
                    <label>Find Listing Availability</label>
                </div>
                <div className="input-field">
                    <label>Select date to start</label>
                    <input type="date" id="month" value={initialDate} placeholder={initialDate} onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                        setInitialDate(event.target.value);
                        setMonth(new Date(event.target.value).getMonth() + 1);
                        setYear(new Date(event.target.value).getFullYear());
                    }} />
                </div>
                <div className="buttons">
                    <Button title="Find availability" theme={ButtonTheme.DARK} onClick={async () => {
                        setLoaded(false);
                        if (!initialDate) {
                            onSideBarError("Please select a date");
                        }
                        if (month && year) {
                            await Ajax.post("/scheduling/findlistingavailabilitybymonth/", {
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
                    }} />
                </div>
            </div>
        );
    };
    const renderConfirmation = () => {
        console.log("BOOKED TIME FRAMES = ", bookedTimeFrames)
        return <>
            <div className='confirmation-card'>
                <div className='h1'>Confirm Your Booking</div>
                <div className='h2'>Listing: {props.listingTitle}</div>
                <p className='h2'>Booking details:</p>
                {renderSummary(bookedTimeFrames, error)}
                <div className='buttons'>
                    <Button title="No" theme={ButtonTheme.HOLLOW_DARK} onClick={() => history.go(-1)} />
                    <Button title="Yes" theme={ButtonTheme.DARK} onClick={async () => {
                        if (authData && authData.role !== Auth.Roles.DEFAULT_USER) {
                            await Ajax.post("/scheduling/reserve", {
                                userId: reserveTimeSlots?.userId,
                                listingId: reserveTimeSlots?.listingId,
                                fullPrice: reserveTimeSlots?.fullPrice,
                                chosenTimeFrames: reserveTimeSlots?.chosenTimeFrames
                            }).then(response => {
                                if (response.error) {
                                    onSideBarError("Scheduling Error. Please refresh page or try again later.")
                                }
                                setOnSuccess(true);
                                setShowConfirmation(true);
                            })
                        }
                        else {
                            onError("User must be logged in to make reservation.")
                        }
                    }} />
                </div>
            </div>

            {error &&
                <div className="error">
                    {error}
                </div>}
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
                        {renderSidebar()}
                        {renderSummary(bookedTimeFrames, error)}
                        {readyToSubmit &&
                            <div className='opentimeslots-sidebar'>
                                <div className='buttons'>
                                    <Button title="Reserve" theme={ButtonTheme.DARK} onClick={async () => {
                                        setFullPrice(bookedTimeFrames.length * props.price);
                                        setShowConfirmation(true);
                                    }} />
                                </div>
                            </div>
                        }
                    </div>
                }

                {/** Render calendar and hour bars after chosing the date*/}
                {!showConfirmation &&
                    <div className="main-wrapper">
                        <h1>{props.listingTitle}</h1>
                        <h2>${props.price}/hour</h2>
                        <div className='second-wrapper'>
                            <div className="view-wrapper">
                                <div className='calendar'>
                                    <div className='header'>
                                        <Button title="<" onClick={() => {
                                            let prevDate = new Date(initialDate);
                                            const prevYear = prevDate.getFullYear();
                                            const prevMonth = prevDate.getMonth(); // January is 0
                                            setInitialDate(new Date(prevYear, prevMonth-1, 1).toDateString());
                                            setMonth(prevMonth-1);
                                            setYear(prevYear);
                                        }} />
                                        <div className='text'>
                                            {getMonth((new Date(initialDate)).getMonth() + 1)} {(new Date(initialDate)).getFullYear()}
                                        </div>
                                        <Button title=">" onClick={() => {
                                            let nextDate = new Date(initialDate);
                                            const nextYear = nextDate.getFullYear();
                                            const nextMonth = nextDate.getMonth(); // January is 0
                                            setInitialDate(new Date(nextYear, nextMonth+1, 1).toDateString());
                                            setMonth(nextMonth+1);
                                            setYear(nextYear);
                                        }}/>
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
            </div>

            {/** Render confirmation card after clicking reserve button */}
            {showConfirmation &&
                // <ConfirmationCard onSuccess={() => { }} reserveTimeSlots={reserveTimeSlots ?? null} />
                <div className='confirmation-card-wrapper'>
                    {renderConfirmation()}
                </div>
            }
            <Footer />
        </div>
    );
}

export default OpenSlotsView;