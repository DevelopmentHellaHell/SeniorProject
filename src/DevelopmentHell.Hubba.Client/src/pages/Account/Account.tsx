import React from "react";
import Footer from "../../components/Footer/Footer";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import "./Account.css";

interface IAccountProps {

}

const Account: React.FC<IAccountProps> = (props) => {
    return (
        <div className="account-container">
            <NavbarUser />

            <div className="account-wrapper">
                <h1>Account Todo</h1>
            </div>

            <Footer />
        </div> 
    );
}

export default Account;