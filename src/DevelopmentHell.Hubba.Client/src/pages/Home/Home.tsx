import React from "react";
import Footer from "../../components/Footer/Footer";
import NavbarGuest from "../../components/NavbarGuest/NavbarGuest";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import { Auth } from "../../Auth";
import './Home.css';

interface IHomeProps {
    
}

const Home: React.FC<IHomeProps> = (props) => {
    const authData = Auth.isAuthenticated();

    return (
        <div className="home-container">
            {authData && authData.role !== Auth.Roles.DEFAULT_USER  ? 
                <NavbarUser /> : <NavbarGuest /> 
            }

            <div className="home-content">
                <div className="home-wrapper">
                    <h1>Home Todo</h1>
                </div>
            </div>
            
            <Footer />
        </div>
    );
}

export default Home;