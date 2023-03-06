import React from "react";
import Footer from "../../components/Footer/Footer";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import "./Notification.css";

interface INotificationProps {

}

const Notification: React.FC<INotificationProps> = (props) => {
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

export default Notification;