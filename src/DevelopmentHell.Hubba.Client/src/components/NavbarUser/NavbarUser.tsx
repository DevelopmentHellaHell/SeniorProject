import React from "react";
import { redirect, useNavigate } from "react-router-dom";
import Dropdown from "../Dropdown/Dropdown";
import { Auth } from "../../Auth";
import './NavbarUser.css';

interface Props {

}

const NavbarUser: React.FC<Props> = (props) => {
    const navigate = useNavigate();
    const authData = Auth.isAuthenticated();

    if (!authData) {
        redirect("/login");
        return null;
    }

    return (
        <header className="nav-user">
            <p className="logo" onClick={() => {navigate("/")}}>HUBBA</p>
            <nav className="nav-links">
                <li><p onClick={() => {alert("1")}}>Profile</p></li>
                <li><p onClick={() => {alert("2")}}>Discover</p></li>
                {authData.role === Auth.Roles.ADMIN_USER &&
                    <li><p onClick={() => {alert("3")}}>Admin Panel</p></li>
                }
            </nav>
            <Dropdown title={authData.email ?? "User"}>
                <p onClick={() => {navigate("/account")}}>Account</p>
                <p onClick={() => {navigate("/notification")}}>Notification</p>
                <p onClick={async () => {
                    navigate("/logout")}}>Logout</p>
            </Dropdown>
        </header>
    );
}

export default NavbarUser;