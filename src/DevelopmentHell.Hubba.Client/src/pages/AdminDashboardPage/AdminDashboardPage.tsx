import React, { useState } from "react";
import { redirect } from "react-router-dom";
import { Auth } from "../../Auth";
import Footer from "../../components/Footer/Footer";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import Sidebar from "../../components/Sidebar/Sidebar";
import AnalyticsView from "./AnalyticsView/AnalyticsView";
import "./AdminDashboardPage.css";

interface IAdminDashboardPageProps {

}

enum AdminDashboardViews {
    ANALYTICS = "Analytics",
}

const AdminDashboardPage: React.FC<IAdminDashboardPageProps> = (props) => {
    const [view, setView] = useState(AdminDashboardViews.ANALYTICS);
    const authData = Auth.isAuthenticated();

    if (!authData) {
        redirect("/login");
        return null;
    }

    const renderView = (view: AdminDashboardViews) => {
        switch(view) {
            case AdminDashboardViews.ANALYTICS:
                return <AnalyticsView />; 
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