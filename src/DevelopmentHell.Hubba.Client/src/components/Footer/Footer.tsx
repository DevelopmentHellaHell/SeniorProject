import React from "react";
import "./Footer.css";

interface Props {

}

const Footer: React.FC<Props> = (props) => {
    return (
        <div className="footer-wrapper">
            © Hubba. All rights reserved.
        </div>
    );
}

export default Footer;