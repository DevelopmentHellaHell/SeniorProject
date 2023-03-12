import React from "react";
import Footer from "../../components/Footer/Footer";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import "./NotificationPage.css";

interface INotificationPageProps {

}

const NotificationPage: React.FC<INotificationPageProps> = (props) => {
    return (
        <div className="notification-container">
            <NavbarUser />

            <div className="notification-content">
                <div className="notification-wrapper">
                    <h1>Notification Menu</h1>
                </div>
            </div>
            
            <Footer />
        </div>
    );
}

export default NotificationPage;