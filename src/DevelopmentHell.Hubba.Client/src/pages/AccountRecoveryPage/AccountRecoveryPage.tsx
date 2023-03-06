import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Ajax } from "../../Ajax";
import { Auth } from "../../Auth";
import Button, { ButtonTheme } from "../../components/Button/Button";
import Footer from "../../components/Footer/Footer";
import NavbarGuest from "../../components/NavbarGuest/NavbarGuest";
import "./AccountRecoveryPage.css";

interface IAccountRecoveryPageProps {

}

const AccountRecoveryPage: React.FC<IAccountRecoveryPageProps> = (props) => {
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
        <div className="account-recovery-container">
            <NavbarGuest />

            <div className="account-recovery-content">
                <div className="account-recovery-wrapper">
                    <div className="account-recovery-card">
                        <h1>Recover Account</h1>
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
                                <Button title="Submit" theme={ButtonTheme.DARK} loading={!loaded} onClick={async () => {
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
                                        
                                        const response = await Ajax.post("/account-recovery/emailVerification", ({ email: email }));
                                        if (response.error) {
                                            onError(response.error);
                                            return;
                                        }

                                        setError("");
                                        setShowOtp(true);
                                        setLoaded(true);
                                    } else {
                                        const response = await Ajax.post("/account-recovery/recoveryOtp", ({ otp: otp }));
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
            </div>

            <Footer />
        </div>
    );
}

export default AccountRecoveryPage;