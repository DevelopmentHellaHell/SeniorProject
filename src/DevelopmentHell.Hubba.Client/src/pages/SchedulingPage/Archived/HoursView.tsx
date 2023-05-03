import { useState } from "react";
import Button from "../../../components/Button/Button";


interface IHoursViewProp {
    startTime : Date,
    endTime: Date
}

interface ITimeSlot {
    date: Date,
    enable: boolean
}

const HoursView : React.FC<IHoursViewProp> = (props) => {
    const[date, setDate] = useState(null);
    const[enable, setEnable] = useState(false);


    return (
        <div className='hours-view'>
            <Button title='0'/>
            <Button title='12'/>
        </div>

    )
};

export default HoursView;