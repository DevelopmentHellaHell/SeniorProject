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

    const navigate = useNavigate();

    return (
        <div className="login-container">
            <NavbarGuest />

            <div className="login-wrapper">
                <div className="login-card">
                    <h1>Login</h1>
                    <p className="info">Already registered? Login <u onClick={() => { navigate("/registration") }}>HERE →</u></p>
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
                            <Button title="Submit" onClick={async () => {
                                if (!email) {
                                    setError("Email cannot be empty, please try again.");
                                    return;
                                }

                                if (!password) {
                                    setError("Password cannot be empty, please try again.");
                                    return;
                                }

                                setError("");
                                
                                const response = await Ajax.post("/authentication/login", ({ email: email, password: password }));
                                if (response.error) {
                                    setError(response.error);
                                    return;
                                }
                                navigate("/otp");
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

export default Login;