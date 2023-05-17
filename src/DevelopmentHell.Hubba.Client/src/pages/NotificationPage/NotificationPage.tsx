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

interface IExpandableNotification {
    DateCreated: Date,
    Hide: boolean,
    IsExpanded: boolean,
    NotificationId: number,
    Notifications: INotificationData[]
}

interface TableState {
    expandedRows: Set<number>;
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
    const [groupedData, setGroupedData] = useState<INotificationData[][]>([]);
    const [notificationData, setNotificationData] = useState<(INotificationData | IExpandableNotification)[]>([]);
    const [error , setError] = useState("");
    const [loaded, setLoaded] = useState(false);
    const [selectedNotifications, setSelectedNotifications] = useState<number[]>([]);
    const [filters, setFilters] = useState<NotificationType[]>([]);
    const [lastRefreshed, setLastRefreshed] = useState(Date.now() - REFRESH_COOLDOWN_MILLISECONDS);
    const prevDataRef = useRef<INotificationData[]>();

 
    const initialState: TableState = {
        expandedRows: new Set<number>(),
    };

    const [tableState, setTableState]   = useState<TableState>(initialState);

    const authData = Auth.getAccessData();

    if (!authData) {
        redirect("/login");
        return null;
    }

    const createGrouping = (notifications: INotificationData[]): INotificationData[][] => {
        const groupedNotifications: INotificationData[][] = [];
        // current group is first notification
        let currentGroupedNotifications: INotificationData[] = [notifications[0]];
        let prevNotification = notifications[0];

        // populate current group from second notification
        for (let i = 1; i < notifications.length; i++){
            const currentNotification = notifications[i];
            // check if the next notification was created within 1 minute
            if (new Date(currentNotification.DateCreated).getTime() - new Date(prevNotification.DateCreated).getTime() <= 60000) {
                currentGroupedNotifications.push(currentNotification);
            }
            // if the next notification was over a minute ago, push current group into array
            // and check for next group using the new interval starting at currentNotification
            else {
                groupedNotifications.push(currentGroupedNotifications.reverse());
                currentGroupedNotifications = [currentNotification];
                prevNotification = currentNotification;
            }
        }
        if (currentGroupedNotifications.length) {
            groupedNotifications.push(currentGroupedNotifications.reverse());
        }
        return groupedNotifications.reverse();
    }

    const getData = async () => {
        await Ajax.get<INotificationData[]>("/notification/getNotifications").then((response) => {
            setData(response.data && response.data.length ? response.data : []);
            setError(response.error);
            setLoaded(response.loaded);
            if (response.data) {
                setGroupedData(createGrouping(response.data));
                setNotificationData(groupedData.map((group, index) => {
                    let loadedNotifications: (INotificationData | IExpandableNotification)[] = [];
                    if(group.length > 1){
                        return { DateCreated: group[0].DateCreated,Hide: false,IsExpanded: false, NotificationId: -index, Notifications: group };
                    }
                    else{
                        return group[0];
                    }
                }));
            }
        });
    }

    useEffect(() => {
        getData();
    }, []);

    useEffect(() => {
        setGroupedData(createGrouping(data));
        setNotificationData(groupedData.map((group, index) => {
            let loadedNotifications: (INotificationData | IExpandableNotification)[] = [];
            if(group.length > 1){
                return { DateCreated: group[0].DateCreated,Hide: false,IsExpanded: false, NotificationId: -index, Notifications: group };
            }
            else{
                return group[0];
            }
        }));
    }, [data])

    useEffect(() => {
        setNotificationData(groupedData.map((group, index) => {
            let loadedNotifications: (INotificationData | IExpandableNotification)[] = [];
            if(group.length > 1){
                return { DateCreated: group[0].DateCreated,Hide: false,IsExpanded: false, NotificationId: -index, Notifications: group };
            }
            else{
                return group[0];
            }
        }));
    }, [groupedData])

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

    //keeps track of what rows are being traversed
    const toggleRow = (rowIndex: number) => {
        const expandedRows = new Set(tableState.expandedRows);
        if (expandedRows.has(rowIndex)) {
          expandedRows.delete(rowIndex);
        } else {
          expandedRows.add(rowIndex);
        }
        setTableState({ expandedRows });
    };
    

    //creates the expandable rows or the 2d
    const createExpandableRow = (notificationDataGroup: INotificationData[]) =>{
        return(
            <>
            {notificationDataGroup.map((notificationData, index) => {
                if (filters.length > 0 && !filters.includes(notificationData.Tag)) return null;
                const id = notificationData.NotificationId;
                return(
                    <React.Fragment key={index}>
                        <tr onClick={() => toggleRow(index)}>
                            <td colSpan={3}>Grouping</td>
                        </tr>
                        {tableState.expandedRows.has(index) && (
                            <tr>
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
                    )} 
                    </React.Fragment>
                );
            })}
            </>
        )
    };


    const createNotificationRow = (notificationData: IExpandableNotification | INotificationData) => {
        if ((notificationData as IExpandableNotification).Notifications !== undefined) {
            const expandable = (notificationData as IExpandableNotification)

            const id = notificationData.NotificationId;
            return (
                <React.Fragment key={notificationData.NotificationId}>
                    <tr onClick={() => {
                        if (expandable.IsExpanded) {
                            setNotificationData((oldData) =>
                                oldData.map((notificationDatum) =>
                                    notificationDatum.NotificationId === notificationData.NotificationId
                                        ? { ...notificationDatum, IsExpanded: false }
                                        : notificationDatum
                                )
                            );
                        }
                        else {
                            setNotificationData((oldData) =>
                                oldData.map((notificationDatum) =>
                                    notificationDatum.NotificationId === notificationData.NotificationId
                                        ? { ...notificationDatum, IsExpanded: true }
                                        : notificationDatum
                                )
                            );
                        }
                    }}>
                        <td colSpan={3}>Grouped Notification {expandable.IsExpanded ? 'V' : '>'}</td>
                    </tr>
                    {expandable.IsExpanded
                        &&
                        expandable.Notifications.map((notification: INotificationData) => {
                            if (filters.length > 0 && !filters.includes(notification.Tag)) return null;
                            return <tr>
                                <td className="table-button-hide">
                                    <Button theme={selectedNotifications.includes(notification.NotificationId) ? ButtonTheme.DARK : ButtonTheme.HOLLOW_DARK} onClick={() => {
                                        if (selectedNotifications.includes(notification.NotificationId)) {
                                            selectedNotifications.splice(selectedNotifications.indexOf(notification.NotificationId), 1);
                                            setSelectedNotifications([...selectedNotifications]);
                                            return;
                                        }
                                        setSelectedNotifications([...selectedNotifications, notification.NotificationId]);
                                    }} title={""} />
                                </td>
                                <td>{notification.Message}</td>
                                <td>{NotificationType[notification.Tag]}</td>
                            </tr>
                        })
                    }
                </React.Fragment>
            );
                
        }
        else {
            if (filters.length > 0 && !filters.includes((notificationData as INotificationData).Tag)) return null;
            const id = notificationData.NotificationId;
            return <tr>
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
                <td>{(notificationData as INotificationData).Message}</td>
                <td>{NotificationType[(notificationData as INotificationData).Tag]}</td>
            </tr>
        }
    };
      

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
                                {filters.length > 0 && 
                                    <Button title="Clear" theme={ButtonTheme.DARK} onClick={() => 
                                        setFilters([])}/>
                                }
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
                                {loaded && data && groupedData && groupedData.length > 0 && notificationData.map((notification) => {
                                    return createNotificationRow(notification);
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
                                <Button theme={selectedNotifications.length > 0 ? ButtonTheme.DARK : ButtonTheme.HOLLOW_DARK} title="Hide"
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