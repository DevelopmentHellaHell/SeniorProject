import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Ajax } from "../../Ajax";
import Button, { ButtonTheme } from "../../components/Button/Button";
import Footer from "../../components/Footer/Footer";
import NavbarGuest from "../../components/NavbarGuest/NavbarGuest";
import "./Login.css";

interface ILoginProps {

}

const Login: React.FC<ILoginProps> = (props) => {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(true);
    const [showRecovery, setShowRecovery] = useState(false);

    const navigate = useNavigate();

    const isValidEmail = (email : string) => (
        /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$/i
    ).test(email);
    const onError = (message: string) => {
        setError(message);
        setLoaded(true);
    }
    
    return (
        <div className="login-container">
            <NavbarGuest />

            <div className="login-wrapper">
                <div className="login-card">
                    <h1>Login</h1>
                    <p className="info">Already registered? Register <u onClick={() => { navigate("/registration") }}>HERE ←</u></p>
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
                        <div className="buttons">
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
                                
                                const response = await Ajax.post("/authentication/login", ({ email: email, password: password }));
                                if (response.error) {
                                    if (response.error.toLowerCase().trim().indexOf("account disabled") != -1) {
                                        setShowRecovery(true);
                                    }
                                    
                                    onError(response.error);
                                    return;
                                }

                                setLoaded(true);
                                navigate("/otp");
                            }}/>
                        </div>
                        {error &&
                            <p className="error">{error}</p>
                        }
                        {showRecovery &&
                            <p className="error">Navigate to Account Recovery page <u onClick={() => { navigate("/account-recovery") }}>HERE ←</u></p> 
                        }
                    </div>
                </div>
            </div>

            <Footer />
        </div>
    );
}

export default Login;