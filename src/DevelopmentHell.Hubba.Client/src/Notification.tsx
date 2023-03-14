import { useEffect, useState } from "react";
import { createRoot, Root } from "react-dom/client";
import Button from "./components/Button/Button";
import "./Notification.css";

interface INotificationProps {
    timer: number,
    message?: string,
    title: string,
}

let root: Root | undefined;
export const triggerNotification = (title: string, message?: string) => {
    if (!root) {
        root = createRoot(document.getElementById("notification-fixed-container") as Element);
    }
    root.render(<Notification timer={4000} title={title} message={message} />);
}

const Notification: React.FC<INotificationProps> = (props) => {
    const [closeTimeout, setCloseTimeout] = useState<NodeJS.Timeout | undefined>(undefined);

    useEffect(() => {
        beginCloseTimeout();
    }, []);

    const closeNotification = () => {
        clearTimeout(closeTimeout);
        root?.unmount();
        root = undefined;
    }

    const beginCloseTimeout = () => {
        if (props.timer) {
            const timeout = setTimeout(() => closeNotification(), props.timer);
            setCloseTimeout(timeout);
        }
    }

    return (
        <div className={"notification-popup-container"}
            onMouseEnter={() => clearTimeout(closeTimeout)}
            onMouseLeave={() => beginCloseTimeout()}>
            <div className="notification-wrapper">
                <div className="notification-info-container">
                    <div>
                        <h5 className="text-title">{props.title}</h5>
                        {props.message && 
                            <h5 className="text-message">  {props.message}</h5>
                        }
                    </div>
                </div>
                <div>
                    <Button title={"X"} onClick={() => closeNotification()} />
                </div>
            </div>
        </div>
    );
}

export default Notification;