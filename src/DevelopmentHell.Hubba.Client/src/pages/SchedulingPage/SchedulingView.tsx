import React, { useEffect, useRef, useState } from 'react';
import {
    EventApi,
    DateSelectArg,
    EventClickArg,
    EventContentArg,
    formatDate,
} from '@fullcalendar/core';
import FullCalendar from '@fullcalendar/react';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';
import { INITIAL_EVENTS, createEventId } from './event-utils';
import Button, { ButtonTheme } from '../../components/Button/Button';
import Footer from '../../components/Footer/Footer';
import NavbarGuest from '../../components/NavbarGuest/NavbarGuest';
import { Calendar } from 'fullcalendar';
import "./SchedulingView.css";
import { Ajax } from '../../Ajax';

interface ISchedulingViewProp {

}
interface IOpenSlotsDaily {
    [hour: number]: boolean
}
interface IBookedTimeFrame {
    availabilityId: number,
    startDateTime: Date,
    endDateTime: Date
}
interface IBookingData {
    userId: number,
    listingId: number,
    chosenTimeFrames: IBookedTimeFrame
}
interface IListingAvailabilityData {
    listingId?: number,
    ownerId?: number,
    availabilityId?: number,
    startTime?: Date,
    endTime?: Date
}
interface IListingAvailabilityDataConversion {
    [item: string]: string
}

const ListingAvailability: IListingAvailabilityDataConversion = {
    "listingId": "ListingId",
    "ownerId": "OwnerId",
    "availabilityId": "Availability",
    "startTime": "StartTime",
    "endTime": "EndTime"
}
interface SchedulingViewState {
    currentEvents: EventApi[];
}

const SchedulingView: React.FC<ISchedulingViewProp> = (props) => {
    const [currentEvents, setCurrentEvents] = useState<EventApi[]>([]); //fullcalendar

    const [workspace, setWorkspace] = useState("");
    const [startDate, setStartDate] = useState("");
    const [endDate, setEndDate] = useState("");
    const [startTime, setStartTime] = useState("");
    const [endTime, setEndTime] = useState("");
    const [listingId, setListingId] = useState(0);
    const [month, setMonth] = useState(0);
    const [year, setYear] = useState(0);
    const [availabilityId, setAvailabilityId] = useState(0);

    const [listingAvailabilityData, setListingAvailabilityData] = useState<IListingAvailabilityData | null>(null);
    const [bookingData, setBookingData] = useState<IBookingData | null>(null);
    const [bookingConfirmation, setBookingConfirmation] = useState(false);

    const [loaded, setLoaded] = useState(false);
    const [error, setError] = useState("");

    useEffect( () => {
        let loadedResponses = true;
        let errorResponses = "";
        const response =  Ajax.post("/scheduling/findlistingavailabilitybymonth", ({ listingId: listingId, month: month, year: year }));

        
        setError(errorResponses);
        setLoaded(loadedResponses);
    }, [listingAvailabilityData]);

    useEffect(() => {
        setLoaded(false);
        setError("");
        setBookingConfirmation(false);
    }, [bookingConfirmation]);

    const getItems = (items: IListingAvailabilityDataConversion) => {
        return <>
            {loaded && listingAvailabilityData &&
                setListingAvailabilityData({
                    listingId: listingAvailabilityData?.listingId,
                    ownerId: listingAvailabilityData?.ownerId,
                    availabilityId: listingAvailabilityData?.availabilityId,
                    startTime: listingAvailabilityData?.startTime,
                    endTime: listingAvailabilityData?.endTime
                })
            }
        </>
    };

    const handleReserve = () => {
        // Implement the submit logic here
        Ajax.post("/scheduling/reserve", bookingData).then(response => {
            if (response.error) {
                setError(response.error);
            }
            setBookingConfirmation(true);
        });
    };

    const handleDateSelect = (selectInfo: DateSelectArg) => {
        let title = prompt('Please enter a new title for your event')
        let calendarApi = selectInfo.view.calendar

        calendarApi.unselect() // clear date selection

        if (title) {
            calendarApi.addEvent({
                id: createEventId(),
                title,
                start: selectInfo.startStr,
                end: selectInfo.endStr,
                allDay: selectInfo.allDay
            })
        }
    }

    const handleEventClick = (clickInfo: EventClickArg) => {
        if (confirm(`Are you sure you want to remove the time slot '${clickInfo.event.title}'`)) {
            clickInfo.event.remove()
        }
    }

    const handleEvents = (events: EventApi[]) => {
        setCurrentEvents(events);
    }

    const renderEventContent = (eventContent: EventContentArg) => {
        return (
            <>
                <b>{eventContent.timeText}</b>
                <i>{eventContent.event.title}</i>
            </>
        )
    }

    const renderSidebarEvent = (event: EventApi) => {
        return (
            <li key={event.id}>
                <b>{formatDate(event.start!, { year: 'numeric', month: 'short', day: 'numeric' })}</b>
                <i>{event.title}</i>
            </li>
        )
    }
    const renderCalendar = () => {
        const calendarRef = useRef<HTMLDivElement>(null);

        useEffect(() => {
            const calendarEl = calendarRef.current;
            if (!calendarEl) {
                return;
            }

            const calendar = new Calendar(calendarEl, {
                plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
                headerToolbar: {
                    left: 'prev',
                    center: 'title',
                    right: 'next'
                },
                initialView: 'dayGridMonth',
                editable: true,
                selectable: true,
                selectMirror: true,
                dayMaxEvents: true,
                weekends: true,
                initialEvents: INITIAL_EVENTS,
                select: handleDateSelect,
                eventContent: renderEventContent,
                eventClick: handleEventClick,
                eventsSet: handleEvents
            });

            calendar.render();
            calendar.updateSize();
            return () => {
                calendar.destroy();
            }
        }, []);

        return (
            <div ref={calendarRef}></div>
        );
    };

    const renderSidebar = () => {
        return (

            <div className="scheduling-view-card">
                <h2>Instructions</h2>
                <ul>
                    <li>Select dates and you will be prompted to choose open time slots</li>
                    <li>Click an time slot to remove it</li>
                </ul>


                {/* <div className="input-field">
                    <input type="text" id="workspace" value={workspace} onChange={(e) => setWorkspace(e.target.value)} />
                </div>
                <div className="input-field">
                    <label>Start Date:</label>
                    <label>Date:</label>
                </div>
                <div className="input-field">
                    <input type="date" id="start-date" value={startDate} onChange={(e) => setStartDate(e.target.value)} />
                    <input type="date" id="end-date" value={endDate} onChange={(e) => setEndDate(e.target.value)} />
                </div>

                <div className="input-field">
                    <label>Start Time:</label>
                    <label>End Time:</label>
                </div>
                
                <div className="input-field">
                    <input type="time" id="start-time" value={startTime} onChange={(e) => setStartTime(e.target.value)} />
                    <input type="time" id="end-time" value={endTime} onChange={(e) => setEndTime(e.target.value)} />
                </div> */}

                {/* <div className='calendar-view-sidebar-section'>
                    <label>
                        <input
                            type='checkbox'
                            checked={weekendsVisible}
                            onChange={handleWeekendsToggle}
                        ></input>
                        toggle weekends
                    </label>
                </div> */}
                <div className='calendar-view-sidebar-section'>
                    <h2>All time frames ({currentEvents.length})</h2>
                    <ul>
                        {currentEvents.map(renderSidebarEvent)}
                    </ul>
                </div>
                <div className="buttons">
                    <Button title="Reserve" theme={ButtonTheme.DARK} onClick={handleReserve} />
                </div>
            </div>

        )
    }

    return (
        <div className="scheduling-view-container">
            <NavbarGuest />

            <div className="scheduling-view-content">
                {renderSidebar()}

                <div className="scheduling-view-wrapper">
                    <div className='calendar-view'>
                        <div className='calendar-view-main'>
                            {renderCalendar()}
                        </div>
                    </div>
                </div>
            </div>
            <Footer />
        </div>
    )
}

export default SchedulingView;
