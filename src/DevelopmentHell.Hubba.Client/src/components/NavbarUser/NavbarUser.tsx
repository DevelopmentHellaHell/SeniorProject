import React from "react";
import { redirect, useNavigate } from "react-router-dom";
import Dropdown from "../Dropdown/Dropdown";
import { Auth } from "../../Auth";
import './NavbarUser.css';

interface INavbarUserProps {

}

const NavbarUser: React.FC<INavbarUserProps> = (props) => {
    const navigate = useNavigate();
    const authData = Auth.getAccessData();
    
    if (!authData) {
        redirect("/login");
        return null;
    }

    return (
        <header className="nav-user">
            <p className="logo" onClick={() => {navigate("/")}}>HUBBA</p>
            <nav className="nav-links">
                <li><p onClick={() => {navigate("/discover")}}>Discover</p></li>
                <li><p onClick={() => {navigate("/mylistings")}}>My Listings</p></li>
                
                {authData.role === Auth.Roles.ADMIN_USER &&
                    <li><p onClick={() => { navigate("/admin-dashboard") }}>Admin Dashboard</p></li>
                }
            </nav>
            <Dropdown title={authData.azp ?? "User"}>
                <p id="account" onClick={() => { navigate("/account") }}>Account</p>
                <p id="notification" onClick={() => { navigate("/notification") }}>Notification</p>
                <p id="logout" onClick={async () => {
                    navigate("/logout")}}>Logout</p>
            </Dropdown>
        </header>
    );
}

export default NavbarUser;