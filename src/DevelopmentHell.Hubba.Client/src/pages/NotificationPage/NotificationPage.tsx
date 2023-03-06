import React from "react";
import Footer from "../../components/Footer/Footer";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import "./NotificationPage.css";

interface INotificationPageProps {

}

const NotificationPage: React.FC<INotificationPageProps> = (props) => {
    return (
        <div className="notificaiton-container">
            <NavbarUser />

            <div className="notificaiton-content">
                <div className="notificaiton-wrapper">
                    <h1>Notificaiton Todo</h1>
                </div>
            </div>
            
            <Footer />
        </div>
    );
}

export default NotificationPage;