import { useEffect, useState } from "react";
import { Ajax } from "../../../Ajax";
import Button, { ButtonTheme } from "../../../components/Button/Button";
import "./NotificationSettingsView.css";

interface INotificationSettingsProps {
    
}

interface INotificationSettingsData {
    [setting: string]: boolean
}

interface INotificationSettingsDataConversion {
    [settingKey: string]: string
}

const DeliveryMethods: INotificationSettingsDataConversion = {
    "emailNotifications": "Email",
    "siteNotifications": "Onsite",
    "textNotifications": "SMS"
};

const NotificationTypes: INotificationSettingsDataConversion = {
    "typeScheduling": "Scheduling",
    "typeWorkspace": "Workspace",
    "typeProjectShowcase": "Project Showcase",
    "typeOther": "Other"
};

const SAVE_COOLDOWN_MILLISECONDS = 5000;

const NotificationSettingsView: React.FC<INotificationSettingsProps> = (props) => {
    const [data, setData] = useState<INotificationSettingsData | null>(null);
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(false);
    const [lastSaved, setLastSaved] = useState(Date.now() - SAVE_COOLDOWN_MILLISECONDS);
    const [saveSuccess, setSaveSuccess] = useState(false);
    const [showSaveButton, setShowSaveButton] = useState(false);

    useEffect(() => {
       Ajax.get<INotificationSettingsData>("/notification/getNotificationSettings").then(response => {
            setData(response.data);
            setError(response.error);
            setLoaded(response.loaded);
        });
    }, []);

    useEffect(() => {
        setSaveSuccess(false);
    }, [data]);

    const getSettings = (settings: INotificationSettingsDataConversion) => {
        return <>
            {loaded && data && Object.entries(settings).map(([key, value]) => {
                const settingValue = data[key];

                return (
                    <div className="toggle-item" key={key}>
                        <p className="inline-text">{value}</p>
                        <div className="buttons">
                            <Button
                                title="Off"
                                theme={settingValue ? ButtonTheme.LIGHT : ButtonTheme.DARK}
                                onClick={() => {
                                    setData((previous) => ({...previous, [key]: false}));
                                    setShowSaveButton(true);
                                }}
                            />
                            <Button
                                title="On"
                                theme={settingValue ? ButtonTheme.DARK : ButtonTheme.LIGHT}
                                onClick={() => {
                                    setData((previous) => ({...previous, [key]: true}));
                                    setShowSaveButton(true);
                                }}
                            />
                        </div>
                    </div>
                );
            })}
        </>
    }

    return (
        <div className="notification-settings-wrapper">
            <h1>Notification Settings</h1>
            
            <div className="notification-settings-container">
                <h2>Delivery Method: </h2>
                {getSettings(DeliveryMethods)}
                <h2>Notification Type:</h2>
                {getSettings(NotificationTypes)}
            </div>
            {showSaveButton && 
                <Button title="Save" theme={ButtonTheme.HOLLOW_DARK} onClick={() => {
                    if (Date.now() - lastSaved < SAVE_COOLDOWN_MILLISECONDS) {
                        setSaveSuccess(false);
                        setError("Must wait 5 before saving again.");
                        return;
                    }

                    setLastSaved(Date.now);

                    Ajax.post("/notification/updateNotificationSettings", data).then(response => {
                        setError(response.error);
                        setLoaded(response.loaded);
                        setSaveSuccess(true);
                        setShowSaveButton(false);
                    });
                }}/>
            }
            {!saveSuccess && error && 
                <p className="error">{error}</p>
            }
            {saveSuccess && !error &&
                <p className="success">Settings saved successfully!</p>
            }
        </div>
    );
}

export default NotificationSettingsView;