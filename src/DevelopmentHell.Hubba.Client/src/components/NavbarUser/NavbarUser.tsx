import React from "react";
import Dropdown from "../Dropdown/Dropdown";
import './NavbarUser.css';

interface Props {

}

const NavbarUser: React.FC<Props> = (props) => {
    return (
        <div className="wrapper">
            <div className="nav">
                <div className="nav-menu">
                    <div className="link">
                        <p onClick={() => {alert("1")}}>1</p>
                    </div>
                    <div className="link">
                        <p onClick={() => {alert("2")}}>2</p>
                    </div>
                    <div className="link">
                        <p onClick={() => {alert("3")}}>3</p>
                    </div>
                </div>
                <div className="nav-btn">
                    <Dropdown title="{Username}">
                        <p onClick={() => {alert("1")}}>Account</p>
                        <p onClick={() => {alert("2")}}>Logout</p>
                    </Dropdown>
                </div>
            </div>
        </div>
    );
}

export default NavbarUser;