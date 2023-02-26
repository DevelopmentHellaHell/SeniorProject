import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Ajax } from "../../Ajax";
import Button from "../../components/Button/Button";
import Footer from "../../components/Footer/Footer";
import NavbarGuest from "../../components/NavbarGuest/NavbarGuest";
import "./Logout.css";

interface Props {

}

const Logout: React.FC<Props> = (props) => {
    const [accountId, setAccountId] = useState(null);

    const [error, setError] = useState("");
    
    const navigate = useNavigate();

    return (
        <div className="logout-container">
            <NavbarGuest />

            <div className="logout-wrapper">
                <div className="logout-card">
                    <h1>Logging out...</h1>
                    <p className="info">If page is not refreshed automatically, please click the button below to return to Home Page</p>
                    <div>
                        <div className="buttons">
                            <Button title="Home Page" onClick={() => {navigate("/");}} />
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

export default Logout;