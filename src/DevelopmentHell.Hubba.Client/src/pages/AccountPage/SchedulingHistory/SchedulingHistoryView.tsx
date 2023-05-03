import React, { useEffect, useRef, useState } from "react";
import { redirect } from "react-router-dom";
import { Ajax } from "../../../Ajax";
import { Auth } from "../../../Auth";
import Button, { ButtonTheme } from "../../../components/Button/Button";
import Footer from "../../../components/Footer/Footer";
import NavbarUser from "../../../components/NavbarUser/NavbarUser";
import "./SchedulingHistoryView.css";

interface ISchedulingHistoryPageProps {

}

export interface ISchedulingHistoryData { //TODO get Kevin's DTO
    DateCreated: Date,
    Hide: boolean,
    Message: string,
    SchedulingHistoryId: number,
    Tag: number,
}

enum SchedulingHistoryType {
    CONFIRMED = 1,
    // PENDING = 2,
    // MODIFIED = 3,
    CANCELLED = 4,
}

// const Filters: {
//     [type in SchedulingHistoryType]: string
// } = {
//     [SchedulingHistoryType.OTHER]: "Other",
//     [SchedulingHistoryType.PROJECT_SHOWCASE]: "Project Showcase",
//     [SchedulingHistoryType.WORKSPACE]: "Workspace",
//     [SchedulingHistoryType.SCHEDULING]: "Scheduling",
// }

const REFRESH_COOLDOWN_MILLISECONDS = 5000;

const SchedulingHistoryView: React.FC<ISchedulingHistoryPageProps> = (props) => {
    const getRentalHistoryRoute = "/getrentalhistory/";
    const [data, setData] = useState<ISchedulingHistoryData[]>([]);
    const [error , setError] = useState("");
    const [loaded, setLoaded] = useState(false);
    const [selectedSchedulingHistorys, setSelectedSchedulingHistorys] = useState<number[]>([]);
    // const [filters, setFilters] = useState<SchedulingHistoryType[]>([]);
    const [lastRefreshed, setLastRefreshed] = useState(Date.now() - REFRESH_COOLDOWN_MILLISECONDS);
    const prevDataRef = useRef<ISchedulingHistoryData[]>();

    const authData = Auth.getAccessData();

    if (!authData) {
        redirect("/login");
        return null;
    }

    const getData = async () => {
        await Ajax.get<ISchedulingHistoryData[]>(getRentalHistoryRoute).then((response) => {
            setData(response.data && response.data.length ? response.data : []);
            setError(response.error);
            setLoaded(response.loaded);
        });
    }

    useEffect(() => {
        getData();
    }, []);

    useEffect(() => {
        prevDataRef.current = data;
    }, [data]);

    const createSchedulingHistoryTableRow = (SchedulingHistoryData: ISchedulingHistoryData) => {
        const id = SchedulingHistoryData.SchedulingHistoryId;
        return (
            <tr key={`SchedulingHistory-${SchedulingHistoryData.SchedulingHistoryId}`}>
                <td className="table-button-hide">
                    <Button theme={selectedSchedulingHistorys.includes(id) ? ButtonTheme.DARK : ButtonTheme.HOLLOW_DARK} onClick={() => {
                        if (selectedSchedulingHistorys.includes(id)) {
                            selectedSchedulingHistorys.splice(selectedSchedulingHistorys.indexOf(id), 1);
                            setSelectedSchedulingHistorys([...selectedSchedulingHistorys]);
                            return;
                        } 

                        setSelectedSchedulingHistorys([...selectedSchedulingHistorys, id]);
                    }} title={""}/>
                </td>
                <td>{SchedulingHistoryData.Message}</td>
                <td>{SchedulingHistoryType[SchedulingHistoryData.Tag]}</td>
            </tr>
        );
    }

    // const createFilterButton = (filter: SchedulingHistoryType) => {
    //     return (
    //         <div key={`${filter}-filter`}>
    //             <Button theme={filters.includes(filter) ? ButtonTheme.DARK : ButtonTheme.HOLLOW_DARK} title={Filters[filter]} onClick={() => {
    //                 if (filters.includes(filter)) {
    //                     filters.splice(filters.indexOf(filter), 1);
    //                     setFilters([...filters]);
    //                     return;
    //                 } 

    //                 setFilters([...filters, filter]);
    //             }}/>
    //         </div>
    //     );
    // }

    return (
        <div className="SchedulingHistory-container">
            <div className="SchedulingHistory-content">
                <div className="SchedulingHistory-wrapper">
                    <h1>SchedulingHistorys</h1>
                    <div className="SchedulingHistory-card">
                        {/* <div className="filters-wrapper">
                            <div className="filters">
                                <p>Filters:</p>
                                {Object.keys(Filters).map(key => {
                                    return createFilterButton(+key as SchedulingHistoryType);
                                })}
                            </div>
                        </div> */}

                        <table>
                            <thead>
                                <tr>
                                    <th></th>
                                    <th>Message</th>
                                    <th>Tag</th>
                                </tr>
                            </thead>
                            <tbody>
                                {data.length == 0 &&
                                    <tr>
                                        <td></td>
                                        <td>You have no Scheduling History now.</td>
                                        <td></td>
                                    </tr>
                                }
                                {loaded && data && data.map(value => {
                                    // if (filters.length > 0 && !filters.includes(value.Tag)) return <></>;
                                    return createSchedulingHistoryTableRow(value);
                                })}
                            </tbody>
                        </table>

                        <div className="actions-wrapper">
                            <div className="actions">
                                <Button theme={ButtonTheme.DARK} loading={!loaded} title="Refresh" onClick={async () => {
                                    if (Date.now() - lastRefreshed < REFRESH_COOLDOWN_MILLISECONDS) {
                                        setError(`Must wait ${(REFRESH_COOLDOWN_MILLISECONDS/1000).toFixed(0)} seconds before refreshing again.`);
                                        return;
                                    }
                                    
                                    setLastRefreshed(Date.now());
                                    setLoaded(false);
                                    await getData();
                                    setLoaded(true);
                                }} />
                                
                                <Button theme={selectedSchedulingHistorys.length > 0 ? ButtonTheme.DARK : ButtonTheme.HOLLOW_DARK} title="Cancel Booking"
                                    onClick={() => {
                                        if (selectedSchedulingHistorys.length == 0) {
                                            setError("Select SchedulingHistorys to delete.");
                                            return;
                                        }

                                        Ajax.post("scheduling/cancel", { hideSchedulingHistorys: selectedSchedulingHistorys }).then(response => {
                                            setData(data.filter(el => !selectedSchedulingHistorys.includes(el.SchedulingHistoryId)));
                                            setSelectedSchedulingHistorys([...selectedSchedulingHistorys.filter(el => selectedSchedulingHistorys.includes(el))]);
                                            setError(response.error);
                                        });
                                    }} />
                            </div>
                        </div>
                        {error &&
                            <p className="error">{error}</p>
                        }
                    </div>
                </div>
            </div>
        </div>
    );
}

export default SchedulingHistoryView;