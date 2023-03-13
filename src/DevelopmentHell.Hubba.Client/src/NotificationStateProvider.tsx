import { createContext, useEffect, useReducer, useRef, useState } from "react";
import { Ajax } from "./Ajax";
import { Auth } from "./Auth";
import { triggerNotification } from "./Notification";
import { INotificationData } from "./pages/NotificationPage/NotificationPage";

interface INotificationStateContext {
    data: INotificationData[],
}

export const NotificationStateContext = createContext<INotificationStateContext>({
    data: [],
});

interface INotificationStateProvider {
    children?: React.ReactNode;
}

const REFRESH_NOTIFICATIONS_INTERVAL_MILLISECONDS = 5000;

const NotificationStateProvider: React.FC<INotificationStateProvider> = (props: React.PropsWithChildren<INotificationStateProvider>) => {
    const [data, setData] = useState<INotificationData[]>([]);
    const prevDataRef = useRef<INotificationData[]>();
    
    useEffect(() => {
        const getData = async () => {
            const authData = Auth.getAuthData();
            if (!authData) return;
            await Ajax.get<INotificationData[]>("/notification/getNotifications").then((response) => {
                console.log(response.data);
                setData(response.data && response.data.length ? response.data : []);
            });
        }

        getData();

        const interval = setInterval(() => {
            getData();
         }, REFRESH_NOTIFICATIONS_INTERVAL_MILLISECONDS);

        return (() => { clearInterval(interval) });
    }, []);

    useEffect(() => {
        if (prevDataRef.current && data.length > prevDataRef.current.length) {
            const diff = data.length - prevDataRef.current.length;
            triggerNotification(`You have ${diff} new notification${diff > 1 ? "s" : ""}`);
        }
        prevDataRef.current = data;
    }, [data]);

    const contextValue = {
        data,
      };

    return (
        <NotificationStateContext.Provider value={contextValue}>
            {props.children}
        </NotificationStateContext.Provider>
    );
}

export default NotificationStateProvider;