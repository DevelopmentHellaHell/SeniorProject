import React from "react";
import Footer from "../../components/Footer/Footer";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import "./Account.css";

interface Props {

}

const Account: React.FC<Props> = (props) => {
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