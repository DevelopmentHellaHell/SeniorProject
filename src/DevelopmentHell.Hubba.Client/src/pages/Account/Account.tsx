import React, { useState } from "react";
import { redirect } from "react-router-dom";
import { Auth } from "../../Auth";
import Footer from "../../components/Footer/Footer";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import SidebarAccount from "../../components/Sidebar/Sidebar";
import "./Account.css";
import DeleteAccount from "./LoginSecurity/DeleteAccount/DeleteAccount";
import LoginSecurity from "./LoginSecurity/LoginSecurity";

interface IAccountProps {

}

enum AccountViews {
    EDIT_PROFILE,
    LOGIN_SECURITY,
    LOGIN_SECURITY_UPDATE_PASSWORD,
    LOGIN_SECURITY_ACCOUNT_DELETION,
    NOTIFICATION_SETTINGS,
    SCHEDULING_HISTORY,
    MANAGE_LISTINGS,
}

const Account: React.FC<IAccountProps> = (props) => {
    const [view, setView] = useState(AccountViews.EDIT_PROFILE);
    const authData = Auth.isAuthenticated();

    if (!authData) {
        redirect("/login");
        return null;
    }

    const renderView = (view: AccountViews) => {
        switch(view) {
            case AccountViews.EDIT_PROFILE:
                return <></>; //TODO
            case AccountViews.LOGIN_SECURITY:
                return <LoginSecurity
                    onUpdateClick={() => { setView(AccountViews.LOGIN_SECURITY_UPDATE_PASSWORD) }}
                    onDeleteClick={() => { setView(AccountViews.LOGIN_SECURITY_ACCOUNT_DELETION) }}
                />;
            case AccountViews.LOGIN_SECURITY_UPDATE_PASSWORD:
                return <></>; //TODO
            case AccountViews.LOGIN_SECURITY_ACCOUNT_DELETION:
                return <DeleteAccount onCancelClick={() => { setView(AccountViews.LOGIN_SECURITY) }}/>;
            case AccountViews.NOTIFICATION_SETTINGS:
                return <></>; //TODO
            case AccountViews.SCHEDULING_HISTORY:
                return <></>; //TODO
            case AccountViews.MANAGE_LISTINGS:
                return <></>; //TODO
        }
    }

    return (
        <div className="account-container">
            <NavbarUser />
            <SidebarAccount>
                <li><p onClick={() => {alert("1")}}>Edit Profile</p></li>
                <li><p onClick={() => { setView(AccountViews.LOGIN_SECURITY) }}>Login & Security</p></li>
                <li><p onClick={() => {alert("3")}}>Notification Settings</p></li>
                <li><p onClick={() => {alert("4")}}>Scheduling History</p></li>
                <li><p onClick={() => {alert("5")}}>Manage Listings</p></li>
                <li><p onClick={() => {alert("6")}}>Project Showcase</p></li>
            </SidebarAccount>

            <div className="account-wrapper">
                {renderView(view)}
            </div>

            <Footer />
        </div> 
    );
}

export default Account;