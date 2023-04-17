import React, { useState } from "react";
import { redirect } from "react-router-dom";
import { Auth } from "../../Auth";
import Footer from "../../components/Footer/Footer";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import "./EditProjectShowcase.css";

interface IEditProjectShowcasePageProps {

}

const EditProjectShowcasePage: React.FC<IEditProjectShowcasePageProps> = (props) => {
    const authData = Auth.getAccessData();

    if (!authData) {
        redirect("/login");
        return null;
    }

    return (
        <div className="hubba-container">
            <NavbarUser />

            <div className="hubba-content">

                <div className="content-wrapper">
                </div>
            </div>

            <Footer />
        </div> 
    );
}

export default EditProjectShowcasePage;