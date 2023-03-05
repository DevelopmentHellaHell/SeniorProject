import { useState } from "react";
import { redirect, useNavigate } from "react-router-dom";
import { Ajax } from "../../Ajax";
import { Auth } from "../../Auth";
import Button, { ButtonStyles } from "../../components/Button/Button";
import Footer from "../../components/Footer/Footer";
import NavbarGuest from "../../components/NavbarGuest/NavbarGuest";
import "./Otp.css";

interface IOtpProps {

}

const Otp: React.FC<IOtpProps> = (props) => {
    const [otp, setOtp] = useState("");
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(true);

    const navigate = useNavigate();

    const onError = (message: string) => {
        setError(message);
        setLoaded(true);
    }

    const authData = Auth.isAuthenticated();
    if (!authData) {
        redirect("/login");
        return null;
    }
    

    return (
        <div className="otp-container">
            <NavbarGuest />

            <div className="otp-wrapper">
                <div className="otp-card">
                    <h1>Login</h1>
                    <p className="info">We sent an One-Time Passcode to your registered email. Please enter it below.</p>
                    <div>
                        <div className="input-field">
                            <label>OTP</label>
                            <input type="text" maxLength={8} placeholder="Your OTP" onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                                setOtp(event.target.value);
                            }}/>
                        </div>
                        
                        <div className="buttons">
                            <Button title="Submit" loading={!loaded} style={ButtonStyles.DARK} onClick={async () => {
                                setLoaded(false);
                                if (!otp) {
                                    onError("OTP cannot be empty.");
                                    return;
                                }

                                const response = await Ajax.post("/authentication/otp", ({ otp: otp }));
                                if (response.error) {
                                    onError(response.error);
                                    return;
                                }
                                
                                setLoaded(true);
                                navigate("/account");
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

export default Otp;
