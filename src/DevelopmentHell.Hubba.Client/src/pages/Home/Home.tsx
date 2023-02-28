import React from "react";
import Footer from "../../components/Footer/Footer";
import NavbarGuest from "../../components/NavbarGuest/NavbarGuest";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import { Auth } from "../../Auth";
import './Home.css';

interface Props {
    
}

const Home: React.FC<Props> = (props) => {
    return (
        <div className="home-container">
            {Auth.isAuthenticated() ? 
                <NavbarUser /> : <NavbarGuest /> 
            }

             <div className="home-wrapper">
                <h1>Home Todo</h1>
            </div>
            
            <Footer />
        </div>
    );
}

export default Home;