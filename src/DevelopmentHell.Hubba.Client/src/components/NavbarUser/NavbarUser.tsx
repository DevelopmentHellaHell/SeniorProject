import React from "react";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Ajax } from "../../Ajax";
import Dropdown from "../Dropdown/Dropdown";
import './NavbarUser.css';

interface Props {

}

const NavbarUser: React.FC<Props> = (props) => {
    const [accountId, setAccountId] = useState("");
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
                <p onClick={async () => {
                    const response = await Ajax.post("/authentication/logout", ({ accountId: accountId }));
                    if (response.error) {
                        alert(response.error);
                        return;
                    }
                    navigate("/logout")}}>Logout</p>
            </Dropdown>
        </header>
    );
}

export default NavbarUser;