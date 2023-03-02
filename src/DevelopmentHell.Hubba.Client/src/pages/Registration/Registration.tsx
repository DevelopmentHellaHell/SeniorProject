import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Ajax } from "../../Ajax";
import Button from "../../components/Button/Button";
import Footer from "../../components/Footer/Footer";
import NavbarGuest from "../../components/NavbarGuest/NavbarGuest";
import "./Registration.css";

interface Props {

}

const Registration: React.FC<Props> = (props) => {
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

            <div className="registration-wrapper">
                <div className="registration-card">
                    <h1>Registration</h1>
                    <p className="info">Already registered? Login <u onClick={() => { navigate("/login") }}>HERE →</u></p>
                    <div>
                        <div className="input-field">
                            <label>Email</label>
                            <input type="text" placeholder="Your Email" onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                                setEmail(event.target.value);
                            }}/>
                        </div>
                        <div className="input-field">
                            <label>Password</label>
                            <input type="password" placeholder="Your Password" onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                                setPassword(event.target.value);
                            }}/>
                        </div>
                        <div className="input-field">
                            <label>Confirm Password {}</label>
                            <input type="password" placeholder="Your Password" onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                                setConfirmedPassword(event.target.value);
                            }}/>
                        </div>
                        <div className="buttons">
                            <Button title="Submit" loading={!loaded} onClick={async () => {
                                setLoaded(false);

                                if (!email) {
                                    onError("Email cannot be empty, please try again.");
                                    return;
                                }
                                if(!isValidEmail(email)) {
                                    onError("Invalid email, please try again.");
                                    return;
                                }

                                if (!password) {
                                    onError("Password cannot be empty, please try again.");
                                    return;
                                }
                                if (password.length < 8) {
                                    onError("Invalid password, please try again.");
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

            <Footer />
        </div>
    );
}

export default Registration;