import React from "react";
import { useNavigate } from "react-router-dom";
import Dropdown from "../Dropdown/Dropdown";
import './NavbarUser.css';

interface Props {

}

const NavbarUser: React.FC<Props> = (props) => {
    const navigate = useNavigate();

    return (
        <header className="nav-user">
            <p className="logo" onClick={() => {navigate("/")}}>HUBBA</p>
            <nav className="nav-links">
                <li><p onClick={() => {alert("1")}}>Profile</p></li>
                <li><p onClick={() => {alert("2")}}>Discover</p></li>
            </nav>
            <Dropdown title="{Username}">
                <p onClick={() => {navigate("/account")}}>Account</p>
                <p onClick={() => {navigate("/notification")}}>Notification</p>
                <p onClick={() => {alert("2")}}>Logout</p>
            </Dropdown>
        </header>
    );
}

export default NavbarUser;