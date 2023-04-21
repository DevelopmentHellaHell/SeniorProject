import React, { useEffect, useRef, useState } from "react";
import { redirect } from "react-router-dom";
import { Ajax } from "../../../Ajax";
import { Auth } from "../../../Auth";
import Button, { ButtonTheme } from "../../../components/Button/Button";
import LikeButton from "../../../components/Heart/Heart";
import "./ProjectShowcaseView.css";
import Heart from "../../../components/Heart/Heart";

interface IProjectShowcaseViewProps {
}

export interface ShowcaseData {
    Id: string,
    ShowcaseUser: string,
    LinkedListingTitle: string,
    ShowcaseTitle: string,
    Description: string,
    IsPublished: boolean,
    Rating: number,
    EditTimestamp: Date
}

function Unpublish(showcaseId: string) {
    Ajax.post("/showcase/unpublish", {showcaseId: showcaseId}).then((response) => {
        if (response.error) {
            console.log("Error unpublishing showcase");
        }
    });
}

function Publish(showcaseId: string) {
    Ajax.post("/showcase/publish", {showcaseId: showcaseId}).then((response) => {
        if (response.error) {
            console.log("Error publishing showcase");
        }
    });
}

function DeleteShowcase(showcaseId: string) {
    Ajax.post("/showcase/delete", {showcaseId: showcaseId}).then((response) => {
        if (response.error) {
            console.log("Error deleting showcase");
        }
    });
}

function UnlinkShowcase(showcaseId: string) {
    Ajax.post("/showcase/unlink", {showcaseId: showcaseId}).then((response) => {
        if (response.error) {
            console.log("Error unlinking showcase");
        }
    });
}

const ShowConfirmButtons = () =>{
    <div className="h-stack" id = "confirmation-buttons">
        <Button theme={ButtonTheme.DARK} onClick={() => {
            console.log("Delete");
        }} title={"Delete"}/>
        <Button theme={ButtonTheme.DARK} onClick={() => {
            console.log("Unlink");
        }} title={"Unlink"}/>
    </div>
}

function EditShowcase(showcaseId: string) {

}

const ProjectShowcaseView: React.FC<IProjectShowcaseViewProps> = (props) => {
/*
<Button theme={selectedNotifications.includes(id) ? ButtonTheme.DARK : ButtonTheme.HOLLOW_DARK} onClick={() => {
    if (selectedNotifications.includes(id)) {
        selectedNotifications.splice(selectedNotifications.indexOf(id), 1);
        setSelectedNotifications([...selectedNotifications]);
        return;
    } 

    setSelectedNotifications([...selectedNotifications, id]);
}} title={""}/>
*/

    const createShowcaseTableRow = (showcaseData: ShowcaseData) => {
        const IsPublishedClass = "published-" + showcaseData.IsPublished ? "yes" : "no";
        return (
            <tr key={`showcase-${showcaseData.Id}`}>
                <td className="table-rating"> {showcaseData.Rating}</td>
                <td className="table-listing">{showcaseData.LinkedListingTitle}</td>
                <td className="table-title">{showcaseData.ShowcaseTitle}</td>
                <td className="table-pubstatus">{
                    <svg className =  "vector-circle" width = "20" height = "20">
                        <circle cx = "10" cy = "10" r = "10" className = {IsPublishedClass} onClick={() => {
                            if(showcaseData.IsPublished) {
                                Unpublish(showcaseData.Id);
                            }
                            else{
                                Publish(showcaseData.Id);
                            }
                        }}/>
                    </svg>
                }
                </td>
                <td className="table-actions">
                    <div className="h-stack">
                        <Button theme={ButtonTheme.LIGHT} onClick={() => {
                            DeleteShowcase(showcaseData.Id);
                        }} title={"Delete"}/>
                        <Button theme={ButtonTheme.LIGHT} onClick={() => {
                            UnlinkShowcase(showcaseData.Id);
                        }} title={"Unlink"}/>
                        <Button theme={ButtonTheme.HOLLOW_LIGHT} onClick={() => {
                            EditShowcase(showcaseData.Id);
                        }} title={"Edit"}/>
                    </div>
                </td>
            </tr>
        );
    }

    return (
        <div className="my-showcases-wrapper">
            <h1>My Project Showcases</h1>

            <div className="my-showcases-container">
                <table>
                    <thead>
                        <th>
                            <LikeButton size={"20"} defaultOn = {true} enabled={false}
                            OnUnlike={function (...args: any[]) {
                                throw new Error("Function not implemented.");
                            } }
                            OnLike={function (...args: any[]) {
                                throw new Error("Function not implemented.");
                            } }/>
                        </th>
                        <th>Linked Listing</th>
                        <th>Project Title</th>
                        <th>Published</th>
                        <th>Actions</th>
                    </thead>
                </table>
            </div>
        </div> 
    );
}

export default ProjectShowcaseView;