import { useEffect, useState } from "react";
import { Ajax } from "../../../Ajax";
import Button, { ButtonTheme } from "../../../components/Button/Button";
import Dropdown from "../../../components/Dropdown/Dropdown";

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
    const [success, setSuccess] = useState("");
    const [loaded, setLoaded] = useState(false);

    const [sort, setSort] = useState("");
    const [search, setSearch] = useState("");

    const [reservationPage, setReservationPage] = useState(1);
    const [reservationCount, setReservationCount] = useState(10);



    const getData = async () => {
        await Ajax.post<IReservationData[]>("/accountsystem/getreservations", {sort: sort, reservationCount: reservationCount, page: reservationPage}).then((response) => {
            setData(response.data && response.data.length ? response.data : [] );
            setError(response.error);
            setLoaded(response.loaded);
        });
    }

    const getQuery = async () => {
        await Ajax.post<IReservationData[]>("/accountsystem/getreservationquery", {query: search}).then((response) => {
            setData(response.data && response.data.length ? response.data : [] );
            setError(response.error);
            setLoaded(response.loaded);
        })
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
                <div className="input-field">
                    <label>Search: </label>
                    <input id='search-query' type='text' placeholder="Search: " onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                    setSearch(event.target.value);
                    }}/>
                    <Button title="Search"theme={ButtonTheme.DARK} onClick={async () => {
                        if(search == "")
                        {
                            setError("Please enter a query. ")
                            return;
                        }
                        getQuery();
                        if(!error){
                            setSuccess("Search Completed!")
                        }
                    }}/>
                </div>
                
                <p>Sort by: </p>
                <Dropdown title={sort}>
                    <p id="default" onClick={() => {setSort("")}}>Select</p>
                    <p id="title" onClick={() => {setSort("Title")}}>Title</p>
                    <p id="userId" onClick={() => {setSort("UserId")}}>User Id</p>
                    <p id="listingId" onClick={() => {setSort("ListingId")}}>Title</p>
                </Dropdown>
                <Button theme={ButtonTheme.DARK} title="Sort" onClick={() => getData()}/>
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
            <div className="reservation-control">
                <div className="reservation-count-control">
                    <p>reservations per page:</p>
                    <div className="h-stack">
                        <Button theme={reservationCount == 10 ? ButtonTheme.DARK : ButtonTheme.HOLLOW_DARK} title="10"
                        onClick={() => {
                            if (reservationCount != 10){
                                setLoaded(false);
                                setReservationPage(1);
                                setReservationCount(10);
                                getData();
                            }
                        }}></Button>
                        <Button theme={reservationCount == 20 ? ButtonTheme.DARK : ButtonTheme.HOLLOW_DARK} title="20" 
                        onClick={() => {
                            if (reservationCount != 20){
                                setLoaded(false);
                                setReservationPage(1);
                                setReservationCount(20);
                                getData();
                            }
                        }}/>
                        <Button theme={reservationCount == 50 ? ButtonTheme.DARK : ButtonTheme.HOLLOW_DARK} title="50" 
                        onClick={() => {
                            if (reservationCount != 50){
                                setLoaded(false);
                                setReservationPage(1);
                                setReservationCount(50);
                                getData();
                            }
                        }}/>
                    </div>
                    <div className="booking-page-control">
                        <p>page #{reservationPage}:</p>
                        <div className="h-stack">
                            {reservationPage > 1 &&
                                <Button theme={ButtonTheme.DARK} title="prev" onClick={() => {
                                    setReservationPage(reservationPage - 1);
                                    getData();
                                }}></Button>
                            }
                            {data.length == reservationCount &&
                                <Button theme={ButtonTheme.DARK} title="next" onClick={() => {
                                    setReservationPage(reservationPage + 1);
                                    getData();
                                }}/>
                            }
                        </div>
                    </div>
                </div>
            {error && !success &&
                <p className="error">{error}</p>
            }
            {!error && success &&
                <p className="success">{success}</p>
            }           
            </div> 
        </div>
        
    )
}

export default ReservationView;