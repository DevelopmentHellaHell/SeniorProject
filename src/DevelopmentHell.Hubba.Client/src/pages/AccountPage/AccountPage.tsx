import React, { useState } from "react";
import { redirect, useLocation, useNavigate } from "react-router-dom";
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
import ProjectShowcaseView from "./ProjectShowcasesView/ProjectShowcaseView";
import UpdatePasswordView from "./LoginSecurityView/UpdatePasswordView/UpdatePasswordView";
import OtpVerificationPasswordView from "./LoginSecurityView/UpdatePasswordView/OtpVerificationPasswordView";
import OtpVerificationEmailView from "./EditProfileView/OtpVerificationEmailView";
import EditProfileView from "./EditProfileView/EditProfileView";
import UpdateEmailView from "./EditProfileView/UpdateEmail";
import BookingHistoryView from "./BookingHistoryView/BookingHistoryView";

interface IAccountPageProps {

}

export enum AccountViews {
    EDIT_PROFILE = "Edit Profile",
    EDIT_PROFILE_UPDATE_EMAIL = "Update Email",
    LOGIN_SECURITY = "Login & Security",
    LOGIN_SECURITY_UPDATE_PASSWORD = "Update Password",
    LOGIN_SECURITY_ACCOUNT_DELETION = "Account Deletion",
    NOTIFICATION_SETTINGS = "Notification Settings",
    BOOKING_HISTORY = "Booking History",
    MANAGE_LISTINGS = "Manage Listings",
    PROJECT_SHOWCASES = "Project Showcases",
    COLLABORATOR_PROFILE = "Collaborator Profile",
    COLLABORATOR_PROFILE_REMOVAL = "Collaborator Profile Removal",
    COLLABORATOR_PROFILE_EDIT = "Collaborator Profile Update",
    COLLABORATOR_PROFILE_DELETION = "Collaborator Profile Deletion",
    OTP_VIEW_PW = "Otp Pw",
    OTP_VIEW_EMAIL = "Otp Email",
}

const SubViews: {
    [view in AccountViews]?: AccountViews[];
} = {
    [AccountViews.LOGIN_SECURITY]: [AccountViews.LOGIN_SECURITY_ACCOUNT_DELETION, AccountViews.LOGIN_SECURITY_UPDATE_PASSWORD],
    [AccountViews.COLLABORATOR_PROFILE]: [ AccountViews.COLLABORATOR_PROFILE_EDIT, AccountViews.COLLABORATOR_PROFILE_REMOVAL, AccountViews.COLLABORATOR_PROFILE_DELETION],
    [AccountViews.OTP_VIEW_PW]: [AccountViews.LOGIN_SECURITY_UPDATE_PASSWORD],
    [AccountViews.EDIT_PROFILE]: [AccountViews.OTP_VIEW_EMAIL],
    [AccountViews.OTP_VIEW_EMAIL]: [AccountViews.EDIT_PROFILE_UPDATE_EMAIL],
}

const AccountPage: React.FC<IAccountPageProps> = (props) => {
    const { state } = useLocation();
    const [view, setView] = useState(state ? state.view : AccountViews.EDIT_PROFILE);
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
                return <EditProfileView
                    onUpdateClick={() => { setView(AccountViews.OTP_VIEW_EMAIL)}}
                />; //TODO
            case AccountViews.LOGIN_SECURITY:
                return <LoginSecurityView
                    onUpdateClick={() => { setView(AccountViews.OTP_VIEW_PW) }}
                    onDeleteClick={() => { setView(AccountViews.LOGIN_SECURITY_ACCOUNT_DELETION) }}
                />;
            case AccountViews.LOGIN_SECURITY_UPDATE_PASSWORD:
                return <UpdatePasswordView 
                    onCancelClick={() => { setView(AccountViews.LOGIN_SECURITY) }}
                    onSuccess={() => {setView(AccountViews.LOGIN_SECURITY)}}
                />; //TODO
                case AccountViews.EDIT_PROFILE_UPDATE_EMAIL:
                    return <UpdateEmailView 
                        onCancelClick={() => { setView(AccountViews.LOGIN_SECURITY) }}
                    />; //TODO
            case AccountViews.LOGIN_SECURITY_ACCOUNT_DELETION:
                return <DeleteAccountView onCancelClick={() => { setView(AccountViews.LOGIN_SECURITY) }}/>;
            case AccountViews.NOTIFICATION_SETTINGS:
                return <NotificationSettingsView />; //TODO
            case AccountViews.BOOKING_HISTORY:
                return <BookingHistoryView/>; //TODO
            case AccountViews.MANAGE_LISTINGS:
                return <></>; //TODO
            case AccountViews.PROJECT_SHOWCASES:
                return <ProjectShowcaseView />;
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
            case AccountViews.OTP_VIEW_PW:
                return <OtpVerificationPasswordView
                        onSuccess={() => {setView(AccountViews.LOGIN_SECURITY_UPDATE_PASSWORD)}}/>;
            case AccountViews.OTP_VIEW_EMAIL:
                return <OtpVerificationEmailView
                    onSuccess={() => {setView(AccountViews.EDIT_PROFILE_UPDATE_EMAIL)}}/>;
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
                    {getListItem(AccountViews.BOOKING_HISTORY, view)}
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