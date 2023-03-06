import { useState } from "react";
import { useNavigate } from "react-router-dom";
import Footer from "../../components/Footer/Footer";
import NavbarGuest from "../../components/NavbarGuest/NavbarGuest";
import "./Login.css";
import LoginCard from "./LoginCard/LoginCard";
import OtpCard from "./OtpCard/OtpCard";

interface ILoginProps {

}

const Login: React.FC<ILoginProps> = (props) => {
    const [showOtp, setShowOtp] = useState(false);
    
    const navigate = useNavigate();

    return (
        <div className="login-container">
            <NavbarGuest />

            <div className="login-content">
                <div className="login-wrapper">
                    {!showOtp ?
                        <LoginCard onSuccess={() => { setShowOtp(true) }}/> :
                        <OtpCard onSuccess={() => { navigate("/") } }/>
                    }
                </div>
            </div>

            <Footer />
        </div>
    );
}

export default Login;