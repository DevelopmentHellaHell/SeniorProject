import React, { useEffect, useRef, useState } from 'react';
import Button, { ButtonTheme } from '../../components/Button/Button';
import Footer from '../../components/Footer/Footer';
import NavbarGuest from '../../components/NavbarGuest/NavbarGuest';
import { Ajax } from '../../Ajax';
import "./OpenSlotsView.css";

interface IOpenTimeSlotsProp {
    listingId: number,

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
const OpenSlotsView: React.FC<IOpenTimeSlotsProp> = (props) => {
    const [listingId, setListingId] = useState(props.listingId);
    const [initialDate, setInitialDate] = useState("");
    const [month, setMonth] = useState("");
    const [year, setYear] = useState("");

    const [availabilityId, setAvailabilityId] = useState("");
    const [listingAvailabilityData, setListingAvailabilityData] = useState<IListingAvailabilityData | null>(null);

    const [loaded, setLoaded] = useState(false);
    const [error, setError] = useState("");
    const onError = (message: string) => {
        setError(message);
        setLoaded(true);
    }
    const renderSidebar = () => {
        return (
            <div className="opentimeslots-view-card">
                <div className="input-field">
                    <label>Select date to start:</label>
                </div>
                <div className="input-field">
                    <input type="date" id="month" value={initialDate} onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                        let date = new Date(event.target.value);
                        setMonth((date.getMonth()+1).toString());
                        setYear(date.getFullYear().toString());
                    }} />
                    
                </div>
                <div className="buttons">
                    <Button title="Find availability" theme={ButtonTheme.DARK} onClick={async () => {
                        setLoaded(false);
                        
                        Ajax.get<IListingAvailabilityData>("/scheduling/findlistingavailabilitybymonth/?listingId=@{listingId}&month=@{month}&year=@{year}").then(response => {
                            if (response.error) {
                                onError(response.error);
                                return;
                            }
                            console.log(response.data);
                            setListingAvailabilityData(response.data);
                            setLoaded(true);
                            console.log(response.error);
                        });

                        
                    }}/>
                </div>
            </div>

        )
    }


    return (
        <div className="opentimeslots-container">
            <NavbarGuest />

            <div className="opentimeslots-content">
                <div className="opentimeslots-wrapper">
                    {renderSidebar()}
                </div>
            </div>
            <Footer />
        </div>
    )
}

export default OpenSlotsView
