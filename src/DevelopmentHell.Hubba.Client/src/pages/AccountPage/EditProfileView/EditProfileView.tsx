import React, { useEffect, useRef, useState } from "react";
import { Ajax } from "../../../Ajax";
import Button, { ButtonTheme } from "../../../components/Button/Button";


interface IEditProfileViewProps {
    onUpdateClick: () => void;

}

export interface IEditProfileData{
    firstName?: string,
    lastName?: string,
}

const EditProfileView: React.FC<IEditProfileViewProps> = (props) => {
    const [data, setData] = useState<IEditProfileData| null>(null);
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(false);
    const[firstName, setFirstName] = useState<string|null>(null);
    const[lastName, setLastName] = useState<string|null>(null);
   
    
    const onError = (message: string) => {
        setError(message);
        setLoaded(true);
    }

    const getData = async () => {
        await Ajax.get<IEditProfileData>("/accountsystem/getaccountsettings").then((response) => {
            setData(response.data);
            setError(response.error);
            setLoaded(response.loaded);
        });
    }

    useEffect(() => {
        getData();
        setLoaded(false);
    }, [])


    return(
        <div className ="edit-profile-wrapper">
            <h1 id="edit-profile-header">Edit Profile</h1>
            <p>Complete changes to email will automatically log you out.</p>

            <div className="update-email-container">
                <h2>Change Email: </h2>
                <div className="emailChangeButton">
                    <Button title="Update" onClick={props.onUpdateClick}></Button>
                </div>
            </div>
            <div className="update-name-container">
                <h2>Change Name: </h2>
                <div className="input-field">
                    <label>First Name</label>
                    <input id='firstName' type="text" placeholder={data?.firstName ? data.firstName : ""} onChange={(event: 
                    React.ChangeEvent<HTMLInputElement>) =>{
                        setFirstName(event.target.value);
                    }}/>
                </div>
                <div className="input-field">
                    <label>Last Name</label>
                    <input id='lastName' type="text" placeholder={data?.lastName ? data.lastName : "last name"} onChange={(event: 
                    React.ChangeEvent<HTMLInputElement>) =>{
                        setLastName(event.target.value);
                    }}/>
                </div>
                <div className="save-button">
                    <Button title="Save" theme={ButtonTheme.DARK} loading={!loaded} onClick={async () => {
                        if (!firstName && !lastName){
                            onError("Please enter in a name before saving. ");
                            return;
                        }
                        const response = await Ajax.post("/accountsystem/updateusername", ({firstName: firstName, lastName: lastName}));
                        if (response.error){
                            onError(response.error);
                            return;
                        }
                        setLoaded(true);
                        
                    } }/>
                </div>
            </div>
            {error && 
                <p className="error">{error}</p>
            }
        </div>
    )
}

export default EditProfileView