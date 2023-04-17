import React, { useState } from "react";
import { redirect } from "react-router-dom";
import { Auth } from "../../Auth";
import Footer from "../../components/Footer/Footer";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import "./ViewProjectShowcase.css";

interface IViewProjectShowcasePageProps {

}

const ViewProjectShowcasePage: React.FC<IViewProjectShowcasePageProps> = (props) => {
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

export default ViewProjectShowcasePage;