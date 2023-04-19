import React, { useEffect, useRef, useState } from "react";
import { redirect } from "react-router-dom";
import { Ajax } from "../../../Ajax";
import { Auth } from "../../../Auth";
import Button, { ButtonTheme } from "../../../components/Button/Button";
import "./NotificationPage.css";

interface IProjectShowcaseViewProps {
    onUpdateClick: () => void;
    onDeleteClick: () => void;
}

export interface ShowcaseData {
    DateEdited: Date,
    Rating: number,
    LinkedListingTitle: string,
    ShowcaseTitle: string,
    IsPublished: boolean,
}

const ProjectShowcaseView: React.FC<IProjectShowcaseViewProps> = (props) => {


    const createShowcaseTableRow = (showcaseData: ShowcaseData) => {
        const id = notificationData.NotificationId;
        return (
            <tr key={`notification-${notificationData.NotificationId}`}>
                <td className="table-button-hide">
                    <Button theme={selectedNotifications.includes(id) ? ButtonTheme.DARK : ButtonTheme.HOLLOW_DARK} onClick={() => {
                        if (selectedNotifications.includes(id)) {
                            selectedNotifications.splice(selectedNotifications.indexOf(id), 1);
                            setSelectedNotifications([...selectedNotifications]);
                            return;
                        } 

                        setSelectedNotifications([...selectedNotifications, id]);
                    }} title={""}/>
                </td>
                <td>{notificationData.Message}</td>
                <td>{NotificationType[notificationData.Tag]}</td>
            </tr>
        );
    }

    return (
        <div className="login-security-wrapper">
            <h1>My Project Showcases</h1>

        </div> 
    );
}

export default ProjectShowcaseView;