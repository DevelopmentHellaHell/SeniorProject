import React, { useEffect, useRef, useState } from "react";
import { redirect } from "react-router-dom";
import { Ajax } from "../../Ajax";
import { Auth } from "../../Auth";
import Button, { ButtonTheme } from "../../components/Button/Button";
import Footer from "../../components/Footer/Footer";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import "./NotificationPage.css";

interface INotificationPageProps {

}

export interface INotificationData {
    DateCreated: Date,
    Hide: boolean,
    Message: string,
    NotificationId: number,
    Tag: number,
}

enum NotificationType {
    OTHER = 0,
    PROJECT_SHOWCASE = 1,
    WORKSPACE = 2,
    SCHEDULING = 3,
}

const Filters: {
    [type in NotificationType]: string
} = {
    [NotificationType.OTHER]: "Other",
    [NotificationType.PROJECT_SHOWCASE]: "Project Showcase",
    [NotificationType.WORKSPACE]: "Workspace",
    [NotificationType.SCHEDULING]: "Scheduling",
}

const REFRESH_COOLDOWN_MILLISECONDS = 5000;

const NotificationPage: React.FC<INotificationPageProps> = (props) => {
    const [data, setData] = useState<INotificationData[]>([]);
    const [error , setError] = useState("");
    const [loaded, setLoaded] = useState(false);
    const [selectedNotifications, setSelectedNotifications] = useState<number[]>([]);
    const [filters, setFilters] = useState<NotificationType[]>([]);
    const [lastRefreshed, setLastRefreshed] = useState(Date.now() - REFRESH_COOLDOWN_MILLISECONDS);
    const prevDataRef = useRef<INotificationData[]>();

    const authData = Auth.getAccessData();

    if (!authData) {
        redirect("/login");
        return null;
    }

    const getData = async () => {
        await Ajax.get<INotificationData[]>("/notification/getNotifications").then((response) => {
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

    const createNotificationTableRow = (notificationData: INotificationData) => {
        const id = notificationData.NotificationId;
        return (
            <tr key={`notification-${notificationData.NotificationId}`}>
                <td className="table-button-hide">
                    <Button theme={selectedNotifications.includes(id) ? ButtonTheme.DARK : ButtonTheme.HOLLOW_DARK} onClick={() => {
                        if (selectedNotifications.includes(id)) {
                            selectedNotifications.splice(selectedNotifications.indexOf(id), 1);
                            setSelectedNotifications([...selectedNotifications]);
                            return;
                        } 

                        setSelectedNotifications([...selectedNotifications, id]);
                    }} title={""}/>
                </td>
                <td>{notificationData.Message}</td>
                <td>{NotificationType[notificationData.Tag]}</td>
            </tr>
        );
    }

    const createFilterButton = (filter: NotificationType) => {
        return (
            <div key={`${filter}-filter`}>
                <Button theme={filters.includes(filter) ? ButtonTheme.DARK : ButtonTheme.HOLLOW_DARK} title={Filters[filter]} onClick={() => {
                    if (filters.includes(filter)) {
                        filters.splice(filters.indexOf(filter), 1);
                        setFilters([...filters]);
                        return;
                    } 

                    setFilters([...filters, filter]);
                }}/>
            </div>
        );
    }

    return (
        <div className="notification-container">
            <NavbarUser />

            <div className="notification-content">
                <div className="notification-wrapper">
                    <h1>Notifications</h1>
                    <div className="notification-card">
                        <div className="filters-wrapper">
                            <div className="filters">
                                <p>Filters:</p>
                                {Object.keys(Filters).map(key => {
                                    return createFilterButton(+key as NotificationType);
                                })}
                            </div>
                        </div>

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
                                        <td>You have no new notifications.</td>
                                        <td></td>
                                    </tr>
                                }
                                {loaded && data && data.map(value => {
                                    if (filters.length > 0 && !filters.includes(value.Tag)) return <></>;
                                    return createNotificationTableRow(value);
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
                                <Button theme={ButtonTheme.DARK} title="Clear All" 
                                    onClick={() => {
                                        if (data.length == 0) {
                                            setError("No notifications to clear.");
                                            return;
                                        }

                                        Ajax.post("/notification/hideAllNotifications", null).then(response => {
                                            setData([]);
                                            setSelectedNotifications([]);
                                            setError(response.error);
                                        });
                                    }} />
                                <Button theme={selectedNotifications.length > 0 ? ButtonTheme.DARK : ButtonTheme.HOLLOW_DARK} title="Delete"
                                    onClick={() => {
                                        if (selectedNotifications.length == 0) {
                                            setError("Select notifications to delete.");
                                            return;
                                        }

                                        Ajax.post("notification/hideIndividualNotifications", { hideNotifications: selectedNotifications }).then(response => {
                                            setData(data.filter(el => !selectedNotifications.includes(el.NotificationId)));
                                            setSelectedNotifications([...selectedNotifications.filter(el => selectedNotifications.includes(el))]);
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
            
            <Footer />
        </div>
    );
}

export default NotificationPage;