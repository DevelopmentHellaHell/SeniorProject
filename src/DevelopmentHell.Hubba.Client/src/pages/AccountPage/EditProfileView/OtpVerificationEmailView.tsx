import { useEffect, useState } from "react";
import { redirect, useNavigate } from "react-router-dom";
import "./OtpCard.css";
import { Ajax } from "../../../Ajax";
import Button, { ButtonTheme } from "../../../components/Button/Button";
import { Auth } from "../../../Auth";


interface IOtpCardProps {
    onSuccess: () => void;
}

const OtpVerificationEmailView: React.FC<IOtpCardProps> = (props) => {
    const [otp, setOtp] = useState("");
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(true);

    const sendOTP = async () => {
        await Ajax.post("/accountsystem/verifyaccount", {}).then((response) => {
            setError(response.error);
            setLoaded(response.loaded);
        })
    }

    useEffect(() => {
        sendOTP();
    }, [])

    const onError = (message: string) => {
        setError(message);
        setLoaded(true);
    }

    const authData = Auth.getAccessData();
    if (!authData) {
        redirect("/");
        return null;
    }
    
    return (
        <div className="otp-card">
            <h1>One-Time Passcode Required</h1>
            <p className="info">We sent an One-Time Passcode to your registered email. Please enter it below.</p>
            <div>
                <div className="input-field">
                    <label>OTP</label>
                    <input id='otp' type="text" maxLength={8} placeholder="Your OTP" onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                        setOtp(event.target.value);
                    }}/>
                </div>
                
                <div className="buttons">
                    <Button title="Submit" loading={!loaded} theme={ButtonTheme.DARK} onClick={async () => {
                        setLoaded(false);
                        if (!otp) {
                            onError("OTP cannot be empty.");
                            return;
                        }

                        const response = await Ajax.post("/accountsystem/otpverification", ({ otp: otp }));
                        if (response.error) {
                            onError(response.error);
                            return;
                        }
                        
                        setLoaded(true);
                        props.onSuccess();
                    }}/>
                </div>
                {error &&
                    <p className="error">{error}</p>
                }
            </div>
        </div>
    )
}

export default OtpVerificationEmailView;
