import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Ajax } from "../../Ajax";
import { Auth } from "../../Auth";
import Button, { ButtonTheme } from "../../components/Button/Button";
import Footer from "../../components/Footer/Footer";
import NavbarGuest from "../../components/NavbarGuest/NavbarGuest";
import "./LogoutPage.css";

interface ILogoutPageProps {

}

const LogoutPage: React.FC<ILogoutPageProps> = (props) => {
    const [error, setError] = useState("");
    
    const navigate = useNavigate();

    useEffect(() => {
        (async () => {
            const response = await Ajax.post("/authentication/logout", {});
            if (response.error) {
                // Do nothing
            }
            Auth.removeCookie("access_token");
            navigate("/");
        })();    
    }, []);

    return (
        <div className="logout-container">
            <NavbarGuest />

            <div className="logout-content">
                <div className="logout-wrapper">
                    <div className="logout-card">
                        <h1>Logging out...</h1>
                        <p className="info">If page is not refreshed automatically, please click the button below to return to Home Page</p>
                        <div>
                            <div className="buttons">
                                <Button title="Home Page" theme={ButtonTheme.DARK} onClick={() => {navigate("/")}} />
                            </div>
                            {error &&
                                <p className="info">{error}</p>
                            }
                        </div>
                    </div>
                </div>
            </div>

            <Footer />
        </div>
    );
}

export default LogoutPage;