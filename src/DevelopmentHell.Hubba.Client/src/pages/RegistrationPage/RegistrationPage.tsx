import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Ajax } from "../../Ajax";
import Button, { ButtonTheme } from "../../components/Button/Button";
import Footer from "../../components/Footer/Footer";
import NavbarGuest from "../../components/NavbarGuest/NavbarGuest";
import "./RegistrationPage.css";

interface IRegistrationPageProps {

}

const RegistrationPage: React.FC<IRegistrationPageProps> = (props) => {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [confirmedPassword, setConfirmedPassword] = useState("");
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(true);
    
    const navigate = useNavigate();

    const onError = (message: string) => {
        setError(message);
        setLoaded(true);
    }
    const isValidEmail = (email : string) => (
        /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$/i
    ).test(email);

    return (
        <div className="registration-container">
            <NavbarGuest />

            <div className="registration-content">
                <div className="registration-wrapper">
                    <div className="registration-card">
                        <h1>Registration</h1>
                        <p className="info"> <u id="redirect-login" onClick={() => { navigate("/login") }}>Already registered? Login HERE ←</u></p>
                        <div>
                            <div className="input-field">
                                <label>Email</label>
                                <input id="email" type="text" placeholder="Your Email" onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                                    setEmail(event.target.value);
                                }}/>
                            </div>
                            <div className="input-field">
                                <label>Password</label>
                                <input id='password' type="password" placeholder="Your Password" onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                                    setPassword(event.target.value);
                                }}/>
                            </div>
                            <div className="input-field">
                                <label>Confirm Password {}</label>
                                <input id='confirm-password' type="password" placeholder="Your Password" onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                                    setConfirmedPassword(event.target.value);
                                }}/>
                            </div>
                            <div  className="buttons">
                                <Button title="Submit" theme={ButtonTheme.DARK} loading={!loaded} onClick={async () => {
                                    setLoaded(false);

                                    if (!email) {
                                        onError("Email cannot be empty.");
                                        return;
                                    }
                                    if(!isValidEmail(email)) {
                                        onError("Invalid email.");
                                        return;
                                    }

                                    if (!password) {
                                        onError("Password cannot be empty.");
                                        return;
                                    }
                                    if (password.length < 8) {
                                        onError("Invalid password.");
                                        return;
                                    }

                                    if (password != confirmedPassword) {
                                        onError("Your passwords do not match.");
                                        return;
                                    }
                                    
                                    const response = await Ajax.post("/registration/register", ({ email: email, password: password }));
                                    if (response.error) {
                                        onError(response.error);
                                        return;
                                    }

                                    setLoaded(true);
                                    navigate("/login");
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

export default RegistrationPage;