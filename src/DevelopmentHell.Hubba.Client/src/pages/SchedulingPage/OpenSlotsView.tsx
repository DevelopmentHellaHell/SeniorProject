import React, { JSXElementConstructor, useEffect, useRef, useState } from 'react';
import Button, { ButtonTheme } from '../../components/Button/Button';
import DateButton, { DateButtonTheme } from './DateButton';
import Footer from '../../components/Footer/Footer';
import NavbarGuest from '../../components/NavbarGuest/NavbarGuest';
import { Ajax } from '../../Ajax';
import "./OpenSlotsView.css";
import HourBarButton, { HourBarButtonTheme } from './HourBarButton';
import { initial } from 'cypress/types/lodash';
import { Auth } from '../../Auth';
import NavbarUser from '../../components/NavbarUser/NavbarUser';
import { start } from 'repl';

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
    bookedTimeFrames: IBookedTimeFrame[]
}

const OpenSlotsView: React.FC<IOpenTimeSlotsProp> = (props) => {
    const [listingId, setListingId] = useState(20);
    const [initialDate, setInitialDate] = useState((new Date()).toLocaleDateString());

    const [month, setMonth] = useState<number>((new Date(initialDate)).getMonth() + 1);
    const [year, setYear] = useState<number>(new Date(initialDate).getFullYear());

    const [reserveTimeSlots, setReserveTimeSlots] = useState<IReservedTimeSlots | null>(null);
    const [readyToSubmit, setReadyToSubmit] = useState<boolean>(false);

    const [chosenDate, setChosenDate] = useState("");
    const [bookedTimeFrames, setBookedTimeFrames] = useState<IBookedTimeFrame[]>([]);
    const [listingAvailabilityData, setListingAvailabilityData] = useState<IListingAvailabilityData[] | null>(null);
    const [selectedHours, setSelectedHours] = useState<number[]>([]);

    const [printedDates, setPrintedDate] = useState<string[]>([]);
    const [loaded, setLoaded] = useState(false);
    const [error, setError] = useState("");

    const authData = Auth.getAuthData();

    useEffect(() => {
        setReserveTimeSlots(null);
        setBookedTimeFrames([]);
        setSelectedHours([]);
        setReadyToSubmit(false);
        setError("");
    }, [initialDate, chosenDate]);

    useEffect(() => {
        if (!readyToSubmit) {
            return;
        }

        setReserveTimeSlots({
            userId: parseInt(authData!.sub),
            listingId: listingId,
            fullPrice: 100,
            bookedTimeFrames: bookedTimeFrames
        } as IReservedTimeSlots);
    }, [readyToSubmit]);

    const onError = (message: string) => {
        setError(message);
        setLoaded(true);
    }
    const renderSummary = (bookedTimeFrames: IBookedTimeFrame[], error: string) => {
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
                    {authData && authData.sub &&
                        <Button title="Reserve" theme={ButtonTheme.DARK}/>
                    }
                    

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
                            onError("Please select a date");
                        }
                        if (month && year) {
                            await Ajax.post("/scheduling/findlistingavailabilitybymonth/", {
                                listingId: listingId,
                                month: month,
                                year: year
                            }).then(response => {
                                if (response.error) {
                                    onError(response.error);
                                    return;
                                }
                                {
                                    !listingAvailabilityData &&
                                        <p className="info"> No open time slots found </p>
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
                <div>
                    {renderSummary(bookedTimeFrames, error)}
                </div>
                <div>
                    {error &&
                        <p className='error'>{error}</p>
                    }
                </div>
            </div>
        );
    };

    const uniqueDates: string[] = Array.from(new Set(listingAvailabilityData?.map(data => (new Date(data.startTime)).toDateString()) ?? []));
    const renderDates = (dateString: string) => {
        let key = new Date(dateString).getFullYear() !== 1990 ? new Date(dateString).getDate().toString() : "";
        let theme = (uniqueDates.includes(dateString)) ? DateButtonTheme.LIGHT : DateButtonTheme.HOLLOW_DARK;
        if (chosenDate == dateString) {
            theme = DateButtonTheme.DARK;
        }
        return <>
            <div className='day'>
                <DateButton
                    id={dateString}
                    title={key}
                    theme={theme}
                    onClick={() => {
                        if (chosenDate !== dateString) {
                            setChosenDate(dateString);
                        }
                        else {
                            setChosenDate("");
                        }
                    }} />
            </div>
        </>
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
        </>;
    }

    const renderHourBars = (date: string) => {
        if (!date || !uniqueDates) {
            return;
        }

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
        //console.log(matchingAvailabilities);

        if (uniqueDates.includes(date)) {
            const hoursArrayAM = Array.from(Array(12).keys());
            const hoursArrayPM = Array.from({ length: 12 }, (_, i) => i + 12);
            let theme = HourBarButtonTheme.HOLLOW_DARK;
            return <>
                <div className='opentimeslots-view-wrapper'>
                    <div className='opentimeslots-hours'>
                        <div className='halfday'>
                            <h4> AM </h4>
                            {hoursArrayAM.map((index) => {
                                //console.log("AM");
                                let matchingTimeSlot: IBookedTimeFrame | undefined = undefined;
                                matchingAvailabilities?.forEach(timeSlot => {
                                    // console.log(new Date(timeSlot.endTime).getHours())
                                    // console.log(new Date(timeSlot.startTime).getHours())
                                    if (new Date(timeSlot.endDateTime).getHours() >= index && new Date(timeSlot.startDateTime).getHours() <= index) {
                                        matchingTimeSlot = timeSlot;
                                    }
                                })
                                let theme = matchingTimeSlot ? HourBarButtonTheme.LIGHT : HourBarButtonTheme.HOLLOW_DARK;
                                theme = selectedHours.includes(index) ? HourBarButtonTheme.DARK : theme;

                                return <>
                                    <div className='buttons'>
                                        <HourBarButton key={index} theme={theme} title={index.toString().concat(":00")} onClick={() => {
                                            if (authData && authData.sub) {
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
                                let matchingTimeSlot: IBookedTimeFrame | undefined = undefined;
                                matchingAvailabilities?.forEach(timeSlot => {
                                    // console.log(new Date(timeSlot.endTime).getHours())
                                    // console.log(new Date(timeSlot.startTime).getHours())
                                    if (new Date(timeSlot.endDateTime).getHours() >= index && new Date(timeSlot.startDateTime).getHours() <= index) {
                                        matchingTimeSlot = timeSlot;
                                    }
                                })
                                let theme = matchingTimeSlot ? HourBarButtonTheme.LIGHT : HourBarButtonTheme.HOLLOW_DARK;
                                theme = selectedHours.includes(index) ? HourBarButtonTheme.DARK : theme;

                                return <>
                                    <div className='buttons'>
                                        <HourBarButton key={index} theme={theme} title={index.toString().concat(":00")} onClick={() => {
                                            if (authData && authData.sub) {
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
                                                onError("You must be logged in.");
                                            }
                                        }} />
                                    </div>
                                </>
                            })}
                        </div>
                    </div>
                </div>
            </>
        }
        else {
            return <>
                <div className='opentimeslots-view-wrapper'>
                    <div className='opentimeslots-hours'>
                        No open time slots found
                    </div>
                </div>
            </>
        }
    };

    return (
        <div className="opentimeslots-view-container">
            {authData && authData.role !== Auth.Roles.DEFAULT_USER ?
                <NavbarUser /> : <NavbarGuest />
            }

            <div className="opentimeslots-view-content">
                {renderSidebar()}
                <div className="opentimeslots-view-main-wrapper">
                    <h1>{props.listingTitle}</h1>
                    <h2>${props.price}/hour</h2>
                    <div className='opentimeslots-view-second-wrapper'>
                        <div className="opentimeslots-view-wrapper">
                            <div className='opentimeslots-calendar'>
                                <h1> {getMonth((new Date(initialDate)).getMonth() + 1)} {(new Date(initialDate)).getFullYear()}</h1>

                                <div className='month'>
                                    {renderCalendar(initialDate)}
                                </div>

                            </div>
                        </div>
                        <div>

                        </div>
                        {renderHourBars(chosenDate)}

                    </div>
                </div>
            </div>
            <Footer />
        </div>
    );
}

export default OpenSlotsView
