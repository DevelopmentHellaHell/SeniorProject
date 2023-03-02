import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Ajax } from "../../Ajax";
import { Auth } from "../../Auth";
import Button from "../../components/Button/Button";
import Footer from "../../components/Footer/Footer";
import NavbarGuest from "../../components/NavbarGuest/NavbarGuest";
import "./AccountRecovery.css";

interface Props {

}

const RecoverAccount: React.FC<Props> = (props) => {
    const [email, setEmail] = useState("");
    const [showOtp, setShowOtp] = useState(false);
    const [otp, setOtp] = useState("");
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(true);

    const navigate = useNavigate();

    const isValidEmail = (email : string) => (
        /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$/i
    ).test(email);
    const onError = (message: string) => {
        setError(message);
        setLoaded(true);
    }
    
    return (
        <div className="accountrecovery-container">
            <NavbarGuest />

            <div className="accountrecovery-wrapper">
                <div className="accountrecovery-card">
                    <h1>Recover Account</h1>
                    {/* <p className="info">Already registered? Register <u onClick={() => { navigate("/registration") }}>HERE ←</u></p> */}
                    <div>
                        <div className="input-field">
                            <label>Email</label>
                            <input type="text" placeholder="Your Email" {...{readOnly: showOtp}} onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                                setEmail(event.target.value);
                            }}/>
                            
                        </div>
                        {showOtp && 
                            <div className="input-field">
                                <label>Recovery OTP</label>
                                <input type="text" maxLength={8} placeholder="Your OTP" onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                                    setOtp(event.target.value);
                                }}/>
                            </div>
                        }
                        <div className="buttons">
                            <Button title="Submit" loading={!loaded} onClick={async () => {
                                setLoaded(false);

                                if (!showOtp) {
                                    if (!email) {
                                        onError("Email cannot be empty.");
                                        return;
                                    }
                                    
                                    if(!isValidEmail(email)) {
                                        onError("Invalid email.");
                                        return;
                                    }
                                    
                                    const response = await Ajax.post("/accountrecovery/emailVerification", ({ email: email }));
                                    if (response.error) {
                                        onError(response.error);
                                        return;
                                    }

                                    setError("");
                                    setShowOtp(true);
                                    setLoaded(true);
                                } else {
                                    const response = await Ajax.post("/accountrecovery/recoveryOtp", ({ otp: otp }));
                                    if (response.error) {
                                        onError(response.error);
                                        return;
                                    }
                                    const authData = Auth.isAuthenticated();
                                    if (authData && authData.role == "DefaultUser") {
                                        setLoaded(true);
                                        Auth.removeCookie("access_token");
                                        alert("Request sent to admin");
                                        navigate("/");
                                        return;
                                    }

                                    setLoaded(true);
                                    navigate("/account");
                                }
                            }}/>
                        </div>
                        {error &&
                            <p className="error">{error}</p>
                        }
                    </div>
                </div>
            </div>

            <Footer />
        </div>
    );
}

export default RecoverAccount;