import React from "react";
import { useNavigate } from "react-router-dom";
import Button from "../Button/Button";
import './NavbarGuest.css';

interface Props {

}

const NavbarGuest: React.FC<Props> = (props) => {
    const navigate = useNavigate();

    return (
        <header className="nav-guest">
            <p className="logo" onClick={() => {navigate("/")}}>HUBBA</p>
            <nav className="nav-links">
                <li><p onClick={() => {alert("1")}}>Discover</p></li>
                <li><p onClick={() => {alert("2")}}>About Us</p></li>
            </nav>
            <div className="buttons">
                <Button
                    title="Sign Up" 
                    onClick={() => {
                        alert("Sign Up");
                    }
                }/>
                <div className="divider" />
                <Button
                    title="Login" 
                    onClick={() => {
                        alert("Login");
                    }
                }/>
            </div>
        </header>
    );
}

export default NavbarGuest;