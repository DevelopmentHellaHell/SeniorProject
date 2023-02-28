import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Ajax } from "../../Ajax";
import Button from "../../components/Button/Button";
import Footer from "../../components/Footer/Footer";
import NavbarGuest from "../../components/NavbarGuest/NavbarGuest";
import "./Otp.css";

interface Props {

}

const Otp: React.FC<Props> = (props) => {
    const [otp, setOtp] = useState("");

    const [error, setError] = useState("");

    const navigate = useNavigate();

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
                            <input type="password" placeholder="Your OTP" onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                                setOtp(event.target.value);
                            }}/>
                        </div>
                        
                        <div className="buttons">
                            <Button title="Submit" onClick={async () => {
                                if (!otp) {
                                    setError("OTP cannot be empty, please try again.");
                                    return;
                                }

                                setError("");

                                const response = await Ajax.post("/otp/register", ({ otp: otp }));
                                if (response.error) {
                                    setError(response.error);
                                    return;
                                }
                                
                                navigate("/account");
                                // TODO
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
