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

const AccountRecovery: React.FC<Props> = (props) => {
    const [email, setEmail] = useState("");
    const [error, setError] = useState("");
    const isValidEmail = (email : string) => (
        /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$/i
    ).test(email);

    const navigate = useNavigate();

    const authData = Auth.isAuthenticated();

    return (
        <div className="recovery-container">
            <NavbarGuest />

            <div className="recovery-wrapper">
                <div className="recovery-card">
                    <h1>Account Recovery</h1>
                    <p className="info">Enter your registered email to recover your account.</p>
                    <div>
                        <div className="input-field">
                            <label>Email</label>
                            <input type="email" placeholder="Your email" onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                                setEmail(event.target.value);
                            }}/>
                        </div>
                        
                        <div className="buttons">
                            <Button title="Submit" onClick={async () => {
                                if (!email) {
                                    setError("Email cannot be empty, please try again.");
                                    return;
                                }
                                if(!isValidEmail(email)) {
                                    setError("Invalid email, please try again.");
                                    return;
                                }

                                setError("");

                                // const response = await Ajax.post("/authentication/recovery", ({ email: email, accountId: authData?.accountId }));
                                // if (response.error) {
                                //     setError(response.error);
                                //     return;
                                // }
                                
                                navigate("/otp");
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

export default AccountRecovery;
