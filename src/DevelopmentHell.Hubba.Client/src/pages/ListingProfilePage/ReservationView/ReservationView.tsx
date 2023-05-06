import { useEffect, useState } from "react";
import { Ajax } from "../../../Ajax";

interface IReservationViewProps{

}

export interface IReservationData{
    ownerId: number,
    userId: number,
    listingId: number,
    title: string,
}

const ReservationView: React.FC<IReservationViewProps> = (props) => {
    const [data, setData] = useState<IReservationData[]>([]);
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(false);

    const getData = async () => {
        await Ajax.get<IReservationData[]>("/accountsystem/getreservations").then((response) => {
            setData(response.data && response.data.length ? response.data : [] );
            setError(response.error);
            setLoaded(response.loaded);
        });
    }

    useEffect(() => {
        getData();
    }, []);

    const createReservationTableRow = (reservationData: IReservationData) => {
        return (
            <tr key={`reservation-${reservationData.listingId}`}>1
                <td>{reservationData.userId}</td>
                <td>{reservationData.listingId}</td>
                <td>{reservationData.title}</td>
            </tr>
        )
    };
    
    return(
        <div className="reservation-container">
            <div className="reservation-content">
                <h1>Reservations</h1>
            </div>
            <table>
                <thead>
                    <tr>
                        <th>User Id</th>
                        <th>Listing Id</th>
                        <th>Title</th>
                    </tr>
                </thead>
                <tbody>
                    {data.length == 0 &&
                        <tr>
                            <td></td>
                            <td></td>
                            <td>You have no reservations</td>
                        </tr>
                    }
                    {loaded && data && data.map(value => {
                        return createReservationTableRow(value);
                    })}
                </tbody>
            </table>
            {error &&
                <p className="error">{error}</p>
            }
        </div>
    )
}

export default ReservationView;