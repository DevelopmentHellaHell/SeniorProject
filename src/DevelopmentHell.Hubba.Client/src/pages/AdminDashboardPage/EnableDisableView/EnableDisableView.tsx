import React, { useEffect, useState } from 'react';
import { Ajax } from '../../../Ajax';
import Loading from '../../../components/Loading/Loading';
import Button, { ButtonTheme } from "../../../components/Button/Button";
import './EnableDisableView.css';

interface IEnableDisableViewProps {

}

const EnableDisableView: React.FC<IEnableDisableViewProps> = (props) => {
    const [email, setEmail] = useState("");
    const [error, setError] = useState("");

    useEffect(()=>{

    }, []);

    return (
        <div className="enable-disable-wrapper">
            <h1>Enable/Disable Account</h1>
            <div>
                <div className="input-field">
                    <label>Email</label>
                    <input id='email' type="text" placeholder="email@mail.com" onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                            setEmail(event.target.value);
                        }}/>
                </div>
                <div className="buttons">
                    <Button title="Enable"theme={ButtonTheme.DARK} onClick={async () => {
                        const response = await Ajax.post("/usermanagement/enable", ({ email: email }));
                        if (response.error) {
                            setError(response.error);
                        }
                        else{
                            setError("Successfully Enabled Account");
                        }
                    }}/>
                    <Button title="Disable" theme={ButtonTheme.DARK} onClick={async () => {
                        const response = await Ajax.post("/usermanagement/disable", ({ email: email }));
                        if (response.error) {
                            setError(response.error);
                        }
                        else{
                            setError("Successfully Disabled Account");
                        }
                    }}/>
                </div>
                <label className="error">{error ? error.toString() : null}</label>
            </div>
        </div>
    );
}

export default EnableDisableView