import { useEffect, useState } from "react";
import { Ajax } from "../../../Ajax";
import Button, { ButtonTheme } from "../../../components/Button/Button";
import Dropdown from "../../../components/Dropdown/Dropdown";
import "./NotificationSettingsView.css";

interface INotificationSettingsProps {

}

export interface INotificationSettingsData {
    [setting: string]: boolean
}

interface INotificationSettingsDataConversion {
    [settingKey: string]: string
}

interface ICellPhoneDetails {
    cellPhoneNumber?: string,
    cellPhoneProvider?: number,
}

const DeliveryMethods: INotificationSettingsDataConversion = {
    "siteNotifications": "Onsite",
    "emailNotifications": "Email",
    "textNotifications": "SMS"
};

const NotificationTypes: INotificationSettingsDataConversion = {
    "typeScheduling": "Scheduling",
    "typeWorkspace": "Workspace",
    "typeProjectShowcase": "Project Showcase",
    "typeOther": "Other"
};

const CellPhoneProvider = new Map<number, string>([
    [0, "Tmobile"],
    [1, "Verizon"],
    [2, "AT&T"],
    [3, "Sprint Mobile"],
    [4, "Virgin Mobile"]
])

const SAVE_COOLDOWN_MILLISECONDS = 5000;

const NotificationSettingsView: React.FC<INotificationSettingsProps> = (props) => {
    const [notificationSettingData, setNotificationSettingData] = useState<INotificationSettingsData | null>(null);
    const [phoneDetailsData, setPhoneDetailsData] = useState<ICellPhoneDetails | null>(null);
    
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(false);
    
    const [lastSaved, setLastSaved] = useState(Date.now() - SAVE_COOLDOWN_MILLISECONDS);
    const [saveSuccess, setSaveSuccess] = useState(false);
    const [showSaveButton, setShowSaveButton] = useState(false);

    useEffect(() => {
        let loadedResponses = true;
        let errorResponses = "";
        Ajax.get<INotificationSettingsData>("/notification/getNotificationSettings").then(response => {
            setNotificationSettingData(response.data);
            
            if (response.error) {
                loadedResponses = false;
                errorResponses = response.error;
            }
        });
        Ajax.get<ICellPhoneDetails>("/notification/getPhoneDetails").then(response => {
            setPhoneDetailsData(response.data);
            
            if (response.error) {
                loadedResponses = false;
                errorResponses = response.error;
            }
        });

        setError(errorResponses);
        setLoaded(loadedResponses);
    }, []);

    useEffect(() => {
        setSaveSuccess(false);
    }, [notificationSettingData, phoneDetailsData]);

    const getSettings = (settings: INotificationSettingsDataConversion) => {
        return <>
            {loaded && notificationSettingData && phoneDetailsData && Object.entries(settings).map(([key, value]) => {
                const settingValue = notificationSettingData[key];
                
                const getPhoneDetails = (key: string) => {
                    if (key !== "textNotifications") {
                        return (<></>);
                    }

                    const getItem = (cellPhoneProvider: number) => {
                        return (
                            <p key={`${cellPhoneProvider}-provider`} onClick={() => {
                                setPhoneDetailsData((previous) => {
                                    if (!previous) return null;
                                    return {...previous, cellPhoneProvider: cellPhoneProvider}
                                });
                                setShowSaveButton(true);
                            }}>{CellPhoneProvider.get(cellPhoneProvider)}</p>
                        );
                    }
                    
                    return (
                        <div className="cellphone-details-fields">
                            <input className="input-field" id="phone-number-input" type="text" maxLength={11} placeholder="Phone Number" value={phoneDetailsData.cellPhoneNumber ? phoneDetailsData.cellPhoneNumber : ""} onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                                setPhoneDetailsData((previous) => {
                                    if (!previous) return null;
                                    return {...previous, cellPhoneNumber: event.target.value}
                                });
                                setShowSaveButton(true);
                            }}/>
                            <div className="dropdown" id="provider-dropdown">
                                <Dropdown title={phoneDetailsData.cellPhoneProvider !== null ? CellPhoneProvider.get(phoneDetailsData.cellPhoneProvider!)! : "Phone Provider"}>
                                    {Array.from(CellPhoneProvider.keys()).map(k => {
                                        return getItem(k);
                                    })}
                                </Dropdown>
                            </div>
                        </div>
                    );
                }

                return (
                    <div className="toggle-item" key={key}>
                        <p className="inline-text">{value}</p>
                        <div className="buttons" id={`${key}-toggle-buttons`}>
                            <Button
                                title="Off"
                                theme={settingValue ? ButtonTheme.LIGHT : ButtonTheme.DARK}
                                onClick={() => {
                                    setNotificationSettingData((previous) => ({...previous, [key]: false}));
                                    setShowSaveButton(true);
                                }}
                            />
                            <Button
                                title="On"
                                theme={settingValue ? ButtonTheme.DARK : ButtonTheme.LIGHT}
                                onClick={() => {
                                    setNotificationSettingData((previous) => ({...previous, [key]: true}));
                                    setShowSaveButton(true);
                                }}
                            />
                        </div>
                        {getPhoneDetails(key)}
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
                    let error = "";
                    if (phoneDetailsData && phoneDetailsData.cellPhoneNumber
                        && (phoneDetailsData.cellPhoneNumber.length > 11
                        || phoneDetailsData.cellPhoneNumber.length <= 9
                        || !/^[0-9]+$/.test(phoneDetailsData.cellPhoneNumber))) {
                        error = "Phone number is not valid.";
                    }

                    if (Date.now() - lastSaved < SAVE_COOLDOWN_MILLISECONDS) {
                        setSaveSuccess(false);
                        error = `Must wait ${(SAVE_COOLDOWN_MILLISECONDS/1000).toFixed(0)} seconds before saving again.`;
                    }

                    if (error) {
                        setError(error);
                        return;
                    }

                    setLastSaved(Date.now());

                    Ajax.post("/notification/updateNotificationSettings", notificationSettingData).then(response => {
                        if (response.error) {
                            error = response.error;
                        }
                    });
                    Ajax.post("/notification/updatePhoneDetails", phoneDetailsData).then(response => {
                        if (response.error) {
                            error = response.error;
                        }
                    });
                    
                    if (!error) {
                        setSaveSuccess(true);
                        setShowSaveButton(false);
                    }

                    setError(error);
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