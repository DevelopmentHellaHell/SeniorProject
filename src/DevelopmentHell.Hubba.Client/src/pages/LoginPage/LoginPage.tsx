import { useState } from "react";
import { useNavigate } from "react-router-dom";
import Footer from "../../components/Footer/Footer";
import NavbarGuest from "../../components/NavbarGuest/NavbarGuest";
import LoginCard from "./LoginCard/LoginCard";
import OtpCard from "./OtpCard/OtpCard";
import "./LoginPage.css";

interface ILoginPageProps {

}

const LoginPage: React.FC<ILoginPageProps> = (props) => {
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

export default LoginPage;