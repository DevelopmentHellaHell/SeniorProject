import React from "react";
import Footer from "../../components/Footer/Footer";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import "./Notification.css";

interface INotificationProps {

}

const Notification: React.FC<INotificationProps> = (props) => {
    return (
        <div className="notification-container">
            <NavbarUser />

            <div className="notification-wrapper">
                <h1>Notification Todo</h1>
            </div>

            <Footer />
        </div> 
    );
}

export default Notification;