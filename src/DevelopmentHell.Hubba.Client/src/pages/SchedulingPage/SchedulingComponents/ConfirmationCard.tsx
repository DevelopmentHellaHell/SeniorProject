import React, { useState } from "react";
import OpenSlotsView, {IBookedTimeFrame, IReservedTimeSlots} from "../OpenSlotsView";
import Button from "../../../components/Button/Button";
import { Auth } from "../../../Auth";


interface IConfirmationCardProp {
    onSuccess: () => void;
    reserveTimeSlots? : IReservedTimeSlots;
}
const ConfirmationCard: React.FC<IConfirmationCardProp> = (props) => {
    const [readyToSubmit, setReadyToSubmit] = useState<boolean>(false);
    const [error, setError] = useState("");
    const authData = Auth.getAuthData();
    const onError = (message: string) => {
        setError(message);
    }
    console.log(props)
    const renderSummary = (bookedTimeFrames : IBookedTimeFrame[], error: string) => {
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
                        Total = {props.reserveTimeSlots?.fullPrice}
                    </div>

                    {/* render Confirmation card */}
                </>
            })}
        </>
    }
    return (
        <div id="confirmation-card" className="confirmation-card">
            <h1>Confirm Your Booking</h1>
            <p className="info">Listing: {props.reserveTimeSlots?.listingTile}</p>
            <div>{props.reserveTimeSlots &&
                renderSummary(props.reserveTimeSlots.chosenTimeFrames, error)}
            </div>
            <div className="buttons">
                <Button title="No"/>
                <Button title="Yes"/>
            </div>
        </div>
    );
};

export default ConfirmationCard;