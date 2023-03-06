import React from "react";
import Footer from "../../components/Footer/Footer";
import NavbarGuest from "../../components/NavbarGuest/NavbarGuest";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import { Auth } from "../../Auth";
import Button, { ButtonTheme } from "../../components/Button/Button";
import WebsiteImage from "./WebsiteImage/WebsiteImage";
import './HomePage.css';
import { useNavigate } from "react-router-dom";

interface IHomePageProps {
    
}

const HomePage: React.FC<IHomePageProps> = (props) => {
    const authData = Auth.isAuthenticated();

    const navigate = useNavigate();

    return (
        <div className="home-container">
            {authData && authData.role !== Auth.Roles.DEFAULT_USER  ? 
                <NavbarUser /> : <NavbarGuest /> 
            }

            <div className="home-content">
                <div className="home-wrapper">
                    <div className="introduction">
                        <div className="introduction-text">
                            <h2>We'll help you to find the best workspace, Hubbist!</h2>
                            <p>Looking for a work station for your freelance gigs? How about renting your spare workshop for fellow enthusiasts who share your mutual interest? With Hubba, you can do this and more!</p>
                            <Button title="Get Started" theme={ButtonTheme.DARK} onClick={() => { navigate("/registration") }} /> {/* TODO: Navigate to profile page instead */}
                        </div>
                        <WebsiteImage />
                    </div>
                </div>
            </div>
            
            <Footer />
        </div>
    );
}

export default HomePage;