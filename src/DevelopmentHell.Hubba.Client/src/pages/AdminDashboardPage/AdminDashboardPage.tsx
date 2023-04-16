import React, { useState } from "react";
import { redirect } from "react-router-dom";
import { Auth } from "../../Auth";
import Footer from "../../components/Footer/Footer";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import Sidebar from "../../components/Sidebar/Sidebar";
import AnalyticsView from "./AnalyticsView/AnalyticsView";
import CreateUpdateView from "./CreateUpdateView/CreateUpdateView"
import EnableDisableView from "./EnableDisableView/EnableDisableView";
import DeleteView from "./DeleteView/DeleteView";
import "./AdminDashboardPage.css";

interface IAdminDashboardPageProps {

}

enum AdminDashboardViews {
    CREATEUPDATE = "Create/Update account",
    ENABLEDISABLE = "Enable/Disable account",
    DELETE = "Delete account",
    ANALYTICS = "Analytics",
}

const AdminDashboardPage: React.FC<IAdminDashboardPageProps> = (props) => {
    const [view, setView] = useState(AdminDashboardViews.ANALYTICS);
    const authData = Auth.getAccessData();

    if (!authData) {
        redirect("/login");
        return null;
    }

    const renderView = (view: AdminDashboardViews) => {
        switch(view) {
            case AdminDashboardViews.ANALYTICS:
                return <AnalyticsView />; 
            case AdminDashboardViews.CREATEUPDATE:
                return <CreateUpdateView/>;
            case AdminDashboardViews.ENABLEDISABLE:
                return <EnableDisableView/>;
            case AdminDashboardViews.DELETE:
                return <DeleteView/>;
        }
    }

    const getListItem = (actualView: AdminDashboardViews, currentView: AdminDashboardViews) => {
        return (
            <li>
                {/* Selection bar */}
                {currentView == actualView ? <div className="selection-bar"></div> : null}
                {/* View */}
                <p onClick={() => { setView(actualView) }}>{actualView}</p>
            </li>
        );
    }

    return (
        <div className="admin-dashboard-container">
            <NavbarUser />
            
            <div className="admin-dashboard-content">
                <Sidebar>
                    {getListItem(AdminDashboardViews.ANALYTICS, view)}
                    {getListItem(AdminDashboardViews.CREATEUPDATE, view)}
                    {getListItem(AdminDashboardViews.ENABLEDISABLE, view)}
                    {getListItem(AdminDashboardViews.DELETE, view)}
                </Sidebar>

                <div className="admin-dashboard-wrapper">
                    {renderView(view)}
                </div>
            </div>

            <Footer />
        </div> 
    );
}

export default AdminDashboardPage;