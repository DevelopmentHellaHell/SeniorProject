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

interface IOpenTimeSlotsProp {
    listingId: number,
    listingTitle: string,
    ownerId: number,
    price: number
}
interface IListingAvailabilityData {
    listingId: number,
    ownerId: number,
    availabilityId: number,
    startTime: Date,
    endTime: Date
}
interface IButtonData {
    date: Date;
    listingAvailabilityData?: IListingAvailabilityData[];
}

interface IBookedTimeFrame {
    availabilityId: number,
    startDateTime: Date,
    endDateTime: Date
}

interface IReservedTimeSlots {
    userId: number,
    listingId: number,
    fullPrice: number,
    chosenTimeFrames: IBookedTimeFrame[]
}

const OpenSlotsView: React.FC<IOpenTimeSlotsProp> = (props) => {
    const [listingId, setListingId] = useState(20);
    const [initialDate, setInitialDate] = useState((new Date()).toDateString());

    const [month, setMonth] = useState<number>((new Date(initialDate)).getMonth() + 1);
    const [year, setYear] = useState<number>(new Date(initialDate).getFullYear());

    const [fullPrice, setFullPrice] = useState(0);
    const [reserveTimeSlots, setReserveTimeSlots] = useState<IReservedTimeSlots | null>(null);
    const [readyToSubmit, setReadyToSubmit] = useState<boolean>(false);

    const [chosenDate, setChosenDate] = useState("");
    const [bookedTimeFrames, setBookedTimeFrames] = useState<IBookedTimeFrame[]>([]);
    const [listingAvailabilityData, setListingAvailabilityData] = useState<IListingAvailabilityData[] | null>(null);
    const [selectedHours, setSelectedHours] = useState<number[]>([]);

    const [printedDates, setPrintedDate] = useState<string[]>([]);
    const [loaded, setLoaded] = useState(false);
    const [onSuccess, setOnSuccess] = useState(false);
    const [sidebarError, setSideBarError] = useState("");
    const [error, setError] = useState("");

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
        console.log("326 - SELECTED HOURS", selectedHours)
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
            let theme = HourBarButtonTheme.GREY;
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
                {/* {hoursArrayAM.map((index) => {
                            //console.log("AM");
                            let matchingTimeSlot: IBookedTimeFrame | undefined = undefined;
                            matchingAvailabilities?.forEach(timeSlot => {
                                // console.log(new Date(timeSlot.endTime).getHours())
                                // console.log(new Date(timeSlot.startTime).getHours())
                                if (new Date(timeSlot.endDateTime).getHours() > index && new Date(timeSlot.startDateTime).getHours() <= index) {
                                    matchingTimeSlot = timeSlot;
                                }
                            })
                            console.log("367 - MATCHING AVAILABILITIES", matchingAvailabilities);
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
                    </div>
                    <div className='halfday'>
                        <h4> PM </h4>
                        {hoursArrayPM.map((index) => {
                            // console.log("PM");
                            // console.log("Matching availabilities", matchingAvailabilities);
                            let matchingTimeSlot: IBookedTimeFrame | undefined = undefined;
                            matchingAvailabilities?.forEach(timeSlot => {
                                // console.log("INDEX = ", index)
                                // console.log("Timeslot start = ", new Date(timeSlot.startDateTime).getHours())
                                // console.log("Timeslot end = ", new Date(timeSlot.endDateTime).getHours())

                                if (new Date(timeSlot.endDateTime).getHours() > index && new Date(timeSlot.startDateTime).getHours() <= index) {
                                    matchingTimeSlot = timeSlot;
                                }
                                // console.log("Matching timeslot = ", matchingTimeSlot)
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
                        })} */}
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
                            onError("Please select the available dates");
                        }
                    }} />
            </div>
            {/* {!uniqueDates.includes(chosenDate) &&
                onError("Please select the available dates")
            } */}
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
        console.log("102 - RESERVED TIME SLOTS", reserveTimeSlots)
        if (!bookedTimeFrames || error) {
            return;
        }
        const printedDate: string[] = Array.from(new Set(bookedTimeFrames.map((date) => date.startDateTime.toDateString()) ?? []));
        return <>
            {printedDate.map((date) => {
                return <>
                    {/* display chosen date and hours */}
                    <div className='info'>
                        <ul>
                            <li>{new Date(date).toLocaleDateString()} ({bookedTimeFrames.length} hours)</li>
                            {bookedTimeFrames.sort((a, b) => a.startDateTime.getHours() - b.startDateTime.getHours()).map((time) => {
                                return <>
                                    {time.startDateTime.toDateString() == date &&
                                        <ul>
                                            <li>{time.startDateTime.toLocaleTimeString()} - {time.endDateTime.toLocaleTimeString()}</li>
                                        </ul>}
                                </>
                            })}
                        </ul>
                    </div>

                    {/* display price */}
                    <div className='info'>
                        Total = {bookedTimeFrames.length * props.price}
                    </div>

                    {/* render Confirmation card */}
                </>
            })}
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
                                console.log(listingAvailabilityData)
                            });
                        }
                    }} />
                </div>
                <div>
                    {renderSummary(bookedTimeFrames, error)}
                </div>
                {readyToSubmit &&
                    <Button title="Reserve" theme={ButtonTheme.DARK} onClick={async () => {
                        setFullPrice(bookedTimeFrames.length * props.price);
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
                            })
                        }
                        else {
                            onSideBarError("User must be logged in to make reservation.")
                        }
                    }} />
                }
                <div>
                    {error &&
                        <p className='error'>{sidebarError}</p>
                    }
                </div>
            </div>
        );
    };

    return (
        <div className="opentimeslots-view-container">
            {authData && authData.role !== Auth.Roles.DEFAULT_USER ?
                <NavbarUser /> : <NavbarGuest />
            }

            <div className="opentimeslots-view-content">
                {renderSidebar()}
                <div className="main-wrapper">
                    <h1>{props.listingTitle}</h1>
                    <h2>${props.price}/hour</h2>
                    <div className='second-wrapper'>
                        <div className="view-wrapper">
                            <div className='calendar'>
                                <h1> {getMonth((new Date(initialDate)).getMonth() + 1)} {(new Date(initialDate)).getFullYear()}</h1>

                                <div className='month'>
                                    {renderCalendar(initialDate)}
                                </div>

                            </div>
                        </div>
                        <div className="slots-card">
                            <h3> Open Time Slots</h3>
                            {renderHourBars(chosenDate)}
                        </div>


                    </div>
                </div>
            </div>
            <Footer />
        </div>
    );
}

export default OpenSlotsView
