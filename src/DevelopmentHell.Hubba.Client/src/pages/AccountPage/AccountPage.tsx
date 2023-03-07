import React, { useState } from "react";
import { redirect } from "react-router-dom";
import { Auth } from "../../Auth";
import Footer from "../../components/Footer/Footer";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import Sidebar from "../../components/Sidebar/Sidebar";
import "./AccountPage.css";
import DeleteAccountView from "./LoginSecurityView/DeleteAccountView/DeleteAccountView";
import LoginSecurityView from "./LoginSecurityView/LoginSecurityView";

interface IAccountPageProps {

}

enum AccountViews {
    EDIT_PROFILE,
    LOGIN_SECURITY,
    LOGIN_SECURITY_UPDATE_PASSWORD,
    LOGIN_SECURITY_ACCOUNT_DELETION,
    NOTIFICATION_SETTINGS,
    SCHEDULING_HISTORY,
    MANAGE_LISTINGS,
    PROJECT_SHOWCASES,
}

const AccountPage: React.FC<IAccountPageProps> = (props) => {
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
                return <LoginSecurityView
                    onUpdateClick={() => { setView(AccountViews.LOGIN_SECURITY_UPDATE_PASSWORD) }}
                    onDeleteClick={() => { setView(AccountViews.LOGIN_SECURITY_ACCOUNT_DELETION) }}
                />;
            case AccountViews.LOGIN_SECURITY_UPDATE_PASSWORD:
                return <></>; //TODO
            case AccountViews.LOGIN_SECURITY_ACCOUNT_DELETION:
                return <DeleteAccountView onCancelClick={() => { setView(AccountViews.LOGIN_SECURITY) }}/>;
            case AccountViews.NOTIFICATION_SETTINGS:
                return <></>; //TODO
            case AccountViews.SCHEDULING_HISTORY:
                return <></>; //TODO
            case AccountViews.MANAGE_LISTINGS:
                return <></>; //TODO
            case AccountViews.PROJECT_SHOWCASES:
                return <></>; //TODO
        }
    }

    const getSelectionBar = (actualView: AccountViews, currentView: AccountViews, ) => {
        return (
            <>
                {currentView == actualView ? <div className="selection-bar"></div> : null}
            </>
        )
    }

    const getListItem = (title: string, actualView: AccountViews, currentView: AccountViews,) => {
        return (
            <li>{getSelectionBar(actualView, currentView)}<p onClick={() => { setView(actualView) }}>{title}</p></li>
        );
    }

    return (
        <div className="account-container">
            <NavbarUser />

            <div className="account-content">
                <Sidebar>
                    {getListItem("Edit Profile", AccountViews.EDIT_PROFILE, view)}
                    {getListItem("Login & Security", AccountViews.LOGIN_SECURITY, view)}
                    {getListItem("Notification Settings", AccountViews.NOTIFICATION_SETTINGS, view)}
                    {getListItem("Scheduling History", AccountViews.SCHEDULING_HISTORY, view)}
                    {getListItem("Manage Listings", AccountViews.MANAGE_LISTINGS, view)}
                    {getListItem("Project Showcases", AccountViews.PROJECT_SHOWCASES, view)}
                </Sidebar>

                <div className="account-wrapper">
                    {renderView(view)}
                </div>
            </div>

            <Footer />
        </div> 
    );
}

export default AccountPage;