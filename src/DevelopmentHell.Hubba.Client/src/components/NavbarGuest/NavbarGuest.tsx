import React from "react";
import { useNavigate } from "react-router-dom";
import Button, { ButtonTheme } from "../Button/Button";
import './NavbarGuest.css';

interface INavbarGuestProps {

}

const NavbarGuest: React.FC<INavbarGuestProps> = (props) => {
    const navigate = useNavigate();

    return (
        <header className="nav-guest">
            <p className="logo" onClick={() => {navigate("/")}}>HUBBA</p>
            <nav className="nav-links">
                <li><p onClick={() => {navigate("/discover")}}>Discover</p></li>
                <li><p onClick={() => {alert("2")}}>About Us</p></li>
            </nav>
            <div className="buttons">
                <Button title="Sign Up" theme={ButtonTheme.HOLLOW_LIGHT} onClick={() => { navigate("/registration") }}/>
                <div className="divider" />
                <Button title="Login" onClick={() => { navigate("/login") }}/>
            </div>
        </header>
    );
}

export default NavbarGuest;