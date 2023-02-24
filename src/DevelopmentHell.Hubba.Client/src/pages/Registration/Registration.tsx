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

    const navigate = useNavigate();

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
                            <Button title="Submit" onClick={async () => {
                                if (!email) {
                                    setError("Email cannot be empty, please try again.");
                                    return;
                                }

                                if (!password) {
                                    setError("Password cannot be empty, please try again.");
                                    return;
                                }

                                if (password != confirmedPassword) {
                                    setError("Your passwords do not match.");
                                    return;
                                }
                                setError("");

                                const response = await Ajax.post("/registration/register", ({ email: email, password: password }));
                                if (response.error) {
                                    setError(response.error);
                                    return;
                                }
                                
                                navigate("/login");
                                // TODO
                            }}/>
                        </div>
                        {error &&
                            <p className="info">{error}</p>
                        }
                    </div>
                </div>
            </div>

            <Footer />
        </div>
    );
}

export default Registration;