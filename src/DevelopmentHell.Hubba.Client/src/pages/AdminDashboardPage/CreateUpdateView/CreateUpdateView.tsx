import React, { useEffect, useState } from 'react';
import { Ajax } from '../../../Ajax';
import Loading from '../../../components/Loading/Loading';
import Button, { ButtonTheme } from "../../../components/Button/Button";
import './CreateUpdateView.css';

interface ICreateUpdateViewProps {
}

const CreateUpdateView: React.FC<ICreateUpdateViewProps> = (props) => {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [lastname, setLastName] = useState("");
    const [firstname, setFirstName] = useState("");
    const [role, setRole] = useState("VerifiedUser");
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(true);

    const onError = (message: string) => {
        setError(message);
        setLoaded(true);
    }


    useEffect(()=>{

    }, []);

    return (
        <div className="create-update-wrapper">
            <h1>Create/Update Account</h1>
            <div>
                <div className="input-field">
                    <label>Email</label>
                    <input id='email' type="text" placeholder="email@mail.com" onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                            setEmail(event.target.value);
                        }}/>
                </div>
                <div className="input-field">
                    <label>Password</label>
                    <input id='password' type="text" placeholder="password123" onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                            setPassword(event.target.value);
                        }}/>
                </div>
                <div className="input-field">
                    <label>Last Name</label>
                    <input id='last-name' type="text" placeholder="Doe" onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                            setLastName(event.target.value);
                        }}/>
                </div>
                <div className="input-field">
                    <label>First Name</label>
                    <input id='first-name' type="text" placeholder="John" onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                            setFirstName(event.target.value);
                        }}/>
                </div>
                <div className="input-field">
                    <label>Account Type</label>
                    <select id='role' onChange={(event: React.ChangeEvent<HTMLSelectElement>) => {
                            setRole(event.target.value);
                        }}>
                            <option selected value="VerifiedUser">Verified</option>
                            <option value="AdminUser">Administrator</option>
                    </select>
                </div>
                <div className="buttons">
                    <Button title="Create"theme={ButtonTheme.DARK} onClick={async () => {
                        
                        const response = await Ajax.post("/usermanagement/create", ({ email: email, password: password, firstname: firstname, lastname: lastname, role: role }));
                        if (response.error) {
                            onError(response.error);
                            return;
                        }
                        else {
                            setError("Successfully created account");
                        }
                    }}/>
                    <Button title="Update" theme={ButtonTheme.DARK} onClick={async () => {
                        const response = await Ajax.post("/usermanagement/update", ({ email: email, password: password, firstname: firstname, lastname: lastname, role: role }));
                        if (response.error) {
                            onError(response.error);
                            return;
                        }
                        else {
                            setError("Successfully updated account");
                        }
                    }}/>
                </div>
                <label className="error">{error ? error.toString() : null}</label>
            </div>
        </div>
    );
}

export default CreateUpdateView