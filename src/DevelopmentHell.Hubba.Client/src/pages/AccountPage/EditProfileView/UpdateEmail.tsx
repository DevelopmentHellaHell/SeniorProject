import { useState } from "react";
import Button, { ButtonTheme } from "../../../components/Button/Button";
import { Ajax } from "../../../Ajax";
import { useNavigate } from "react-router-dom";

interface IUpdateEmailViewProps{
    onCancelClick: () => void;

}

const UpdateEmailView: React.FC<IUpdateEmailViewProps> = (props) => {
    const [newEmail, setNewEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(false);

    const onError = (message: string) => {
        setError(message);
        setLoaded(true);
    }

    const isValidEmail = (email : string) => (
        /^[A-Z0-9._%+-]+@[A-Z0-9_.-]+\.[A-Z]{2,4}$/i
    ).test(email);


    const navigate = useNavigate();

    return(
        <div className="update-email-wrapper">
            <h1 id="update-email-header">Change Email</h1>

            <p>Warining: You will be logged out when the procedure is finished. </p>
            <div>
                <div className="input-field" id="new-email-input">
                    <label>New Email </label>
                    <input id='newEmail' type="text" placeholder="New Email" onChange={(event: React.ChangeEvent<HTMLInputElement>) =>{
                        setNewEmail(event.target.value);
                    }}/>
                </div>
                <div className="input-field" id="password-input">
                    <label>Password </label>
                    <input id='password' type="password" placeholder="Password" onChange={(event: React.ChangeEvent<HTMLInputElement>) =>{
                        setPassword(event.target.value);
                    }}/>
                </div>
                <div className="buttons">
                    <Button title="Cancel" onClick={ props.onCancelClick}/>
                    <Button title="Submit" theme={ButtonTheme.DARK} onClick={async () => {
                        setLoaded(false);
                        if(!newEmail){
                            onError("Please enter in a valid email. ");
                            return;
                        }
                        if(!password){
                            onError("Please enter in your password. ");
                            return;
                        }
                        if(!isValidEmail(newEmail)){
                            onError("Invalid email. ");
                            return;
                        }

                        const response = await Ajax.post("/accountsystem/updateemailinformation", ({newEmail: newEmail, password: password}));
                        if (response.error){
                            onError(response.error);
                            return;
                        }

                        await Ajax.post("/authentication/logout", {});
                        navigate("/");
                    }}/>
                </div>
            </div>
            {error && 
                <p className="error">{error}</p>
            }   
        </div>
    )
}

export default UpdateEmailView