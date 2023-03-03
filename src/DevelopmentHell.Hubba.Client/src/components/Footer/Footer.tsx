import React from "react";
import "./Footer.css";

interface IFooterProps {

}

const Footer: React.FC<IFooterProps> = (props) => {
    return (
        <div className="footer-wrapper">
            © Hubba. All rights reserved.
        </div>
    );
}

export default Footer;