import React, { useState } from "react";
import { redirect, useNavigate } from "react-router-dom";
import { Auth } from "../../Auth";
import Footer from "../../components/Footer/Footer";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import Sidebar from "../../components/Sidebar/Sidebar";
import "./AccountPage.css";
import DeleteAccountView from "./LoginSecurityView/DeleteAccountView/DeleteAccountView";
import LoginSecurityView from "./LoginSecurityView/LoginSecurityView";
import NotificationSettingsView from "./NotificationSettingsView/NotificationSettingsView";
import CollaboratorProfileView from "./CollaboratorProfileView/CollaboratorProfileView";
import CollaboratorRemovalView from "./CollaboratorProfileView/CollaboratorRemovalView/CollaboratorRemovalView";
import CollaboratorEditView from "./CollaboratorProfileView/CollaboratorEditView/CollaboratorEditView";
import CollaboratorDeletionView from "./CollaboratorProfileView/CollaboratorDeletionView/CollaboratorDeletionView";
import { Ajax } from "../../Ajax";

interface IAccountPageProps {

}

enum AccountViews {
    EDIT_PROFILE = "Edit Profile",
    LOGIN_SECURITY = "Login & Security",
    LOGIN_SECURITY_UPDATE_PASSWORD = "Update Password",
    LOGIN_SECURITY_ACCOUNT_DELETION = "Account Deletion",
    NOTIFICATION_SETTINGS = "Notification Settings",
    SCHEDULING_HISTORY = "Scheduling History",
    MANAGE_LISTINGS = "Manage Listings",
    PROJECT_SHOWCASES = "Project Showcases",
    COLLABORATOR_PROFILE = "Collaborator Profile",
    COLLABORATOR_PROFILE_REMOVAL = "Collaborator Profile Removal",
    COLLABORATOR_PROFILE_EDIT = "Collaborator Profile Update",
    COLLABORATOR_PROFILE_DELETION = "Collaborator Profile Deletion",
}

const SubViews: {
    [view in AccountViews]?: AccountViews[];
} = {
    [AccountViews.LOGIN_SECURITY]: [AccountViews.LOGIN_SECURITY_ACCOUNT_DELETION, AccountViews.LOGIN_SECURITY_UPDATE_PASSWORD],
    [AccountViews.COLLABORATOR_PROFILE]: [ AccountViews.COLLABORATOR_PROFILE_EDIT, AccountViews.COLLABORATOR_PROFILE_REMOVAL, AccountViews.COLLABORATOR_PROFILE_DELETION],
}

const AccountPage: React.FC<IAccountPageProps> = (props) => {
    const [view, setView] = useState(AccountViews.EDIT_PROFILE);
    const authData = Auth.getAccessData();
    const accountId = authData?.sub;
    const [collaboratorId, setCollaboratorId] = useState<number>();


    const navigate = useNavigate();

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
                return <NotificationSettingsView />; //TODO
            case AccountViews.SCHEDULING_HISTORY:
                return <></>; //TODO
            case AccountViews.MANAGE_LISTINGS:
                return <></>; //TODO
            case AccountViews.PROJECT_SHOWCASES:
                return <></>; //TODO
            case AccountViews.COLLABORATOR_PROFILE:
                return <CollaboratorProfileView 
                    onViewClick={() => { createOrViewCollab() }}
                    onEditClick={() => { createOrEditCollab() }}
                    onRemoveClick={() => { setView(AccountViews.COLLABORATOR_PROFILE_REMOVAL)}}
                    onDeleteCollabClick={() => { setView(AccountViews.COLLABORATOR_PROFILE_DELETION)}}
                />; 
            case AccountViews.COLLABORATOR_PROFILE_EDIT:
                return <CollaboratorEditView collaboratorId={collaboratorId} />;
            case AccountViews.COLLABORATOR_PROFILE_REMOVAL:
                return <CollaboratorRemovalView onCancelClick={() => { setView(AccountViews.COLLABORATOR_PROFILE)}}/>; 
            case AccountViews.COLLABORATOR_PROFILE_DELETION:
                return <CollaboratorDeletionView onCancelClick={() => { setView(AccountViews.COLLABORATOR_PROFILE)}}/>; 
        }
    }

    const getListItem = (actualView: AccountViews, currentView: AccountViews) => {
        return (
            <li>
                {/* Selection bar */}
                {currentView == actualView || SubViews[actualView]?.includes(currentView) ? <div className="selection-bar"></div> : null}
                {/* View */}
                <p onClick={() => { setView(actualView) }}>{actualView}</p>
            </li>
        );
    }

    const createOrViewCollab = async () =>{
        const response = await Ajax.post<boolean>("/collaborator/hascollaborator", {AccountId: accountId});
        if(response.data){
            const responseCollabId = await Ajax.post<number>("/collaborator/getCollaboratorId", {AccountId: accountId});
            if(responseCollabId.data){
                navigate("/collaborator", { state: { CollaboratorId: responseCollabId.data }});
            }
        }
        setCollaboratorId(undefined);
        return setView(AccountViews.COLLABORATOR_PROFILE_EDIT);
    }

    const createOrEditCollab = async () =>{
        const response = await Ajax.post<boolean>("/collaborator/hascollaborator", {AccountId: accountId});
        if(response.data){
            const responseCollabId = await Ajax.post<number>("/collaborator/getCollaboratorId", {AccountId: accountId});
            if(responseCollabId.data){
                setCollaboratorId(responseCollabId.data);
            }
        }
        return setView(AccountViews.COLLABORATOR_PROFILE_EDIT);
    }

    return (
        <div className="account-container">
            <NavbarUser />

            <div className="account-content">
                <Sidebar>
                    {getListItem(AccountViews.EDIT_PROFILE, view)}
                    {getListItem(AccountViews.LOGIN_SECURITY, view)}
                    {getListItem(AccountViews.NOTIFICATION_SETTINGS, view)}
                    {getListItem(AccountViews.SCHEDULING_HISTORY, view)}
                    {getListItem(AccountViews.MANAGE_LISTINGS, view)}
                    {getListItem(AccountViews.PROJECT_SHOWCASES, view)}
                    {getListItem(AccountViews.COLLABORATOR_PROFILE, view)}
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