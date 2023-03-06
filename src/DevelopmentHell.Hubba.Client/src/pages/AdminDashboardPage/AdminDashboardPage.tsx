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
    ANALYTICS,
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

    return (
        <div className="admin-dashboard-container">
            <NavbarUser />
            
            <div className="admin-dashboard-content">
                <Sidebar>
                    <li><p onClick={() => { setView(AdminDashboardViews.ANALYTICS) }}>Analytics</p></li>
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