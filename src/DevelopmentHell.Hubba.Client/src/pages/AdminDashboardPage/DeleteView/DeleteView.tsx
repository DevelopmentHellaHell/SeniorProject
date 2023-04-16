import React, { useEffect, useState } from 'react';
import { Ajax } from '../../../Ajax';
import Loading from '../../../components/Loading/Loading';
import Button, { ButtonTheme } from "../../../components/Button/Button";
import './DeleteView.css';

interface IDeleteViewProps {

}

const DeleteView: React.FC<IDeleteViewProps> = (props) => {
    const [email, setEmail] = useState("");
    const [confirmation, setConfirmation] = useState(false);
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(true);
    const onError = (message: string) => {
        setError(message);
        setLoaded(true);
    }

    useEffect(()=>{

    }, []);

    const Entry = () => {
        return <div className="buttons">
                <Button title="Delete"theme={ButtonTheme.DARK} onClick={async () => {
                    setConfirmation(true);

                }}/>
            </div>
    }

    const Confirmation = () => {
        return <div>
            <label>Are you sure you want to delete the account associated with {email}?</label>
            <div className="buttons">
                <Button title="Confirm"theme={ButtonTheme.DARK} onClick={async () => {
                    const response = await Ajax.post("/usermanagement/delete", ({ email: email }));
                    if (response.error) {
                        onError(response.error);
                        return;
                    }
                    else{
                        setError("Successfully Deleted Account");
                    }
                    setConfirmation(false);
                }}/>
                <Button title="Cancel"theme={ButtonTheme.HOLLOW_DARK} onClick={async () => {
                    setConfirmation(false);
                }}/>
            </div>
        </div>
    }

    return (
        <div className="delete-wrapper">
            <h1>Delete Account</h1>
            <div className="input-field">
                <label>Email</label>
                <input disabled={confirmation} id='email' type="text" placeholder="email@mail.com" onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                        setEmail(event.target.value);
                    }}/>
            </div>
            <div>
                { confirmation ? <Confirmation /> : <Entry /> }
                
            </div>
            <label className="error">{error}</label>
        </div>
    );
}

export default DeleteView