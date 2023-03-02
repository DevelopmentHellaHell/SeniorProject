import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Ajax } from "../../Ajax";
import Button from "../../components/Button/Button";
import Footer from "../../components/Footer/Footer";
import NavbarGuest from "../../components/NavbarGuest/NavbarGuest";
import "./Login.css";

interface Props {

}

const Login: React.FC<Props> = (props) => {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
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
        <div className="login-container">
            <NavbarGuest />

            <div className="login-wrapper">
                <div className="login-card">
                    <h1>Login</h1>
                    <p className="info">Already registered? Register <u onClick={() => { navigate("/registration") }}>HERE →</u></p>
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
                                
                                const response = await Ajax.post("/authentication/login", ({ email: email, password: password }));
                                if (response.error) {
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
                    </div>
                </div>
            </div>

            <Footer />
        </div>
    );
}

export default Login;