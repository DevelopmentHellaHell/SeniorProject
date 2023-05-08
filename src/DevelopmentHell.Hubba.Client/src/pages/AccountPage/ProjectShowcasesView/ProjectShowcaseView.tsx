import React, { useEffect, useRef, useState } from "react";
import { redirect, Link, useNavigate } from "react-router-dom";
import { Ajax } from "../../../Ajax";
import { Auth } from "../../../Auth";
import Button, { ButtonTheme } from "../../../components/Button/Button";
import LikeButton from "../../../components/Heart/Heart";
import "./ProjectShowcaseView.css";
import { render } from "react-dom";

interface IProjectShowcaseViewProps {
}

export interface IShowcaseData {
    id: string,
    showcaseUserId: number,
    listingId: string,
    listingTitle: string,
    title: string,
    description: string,
    isPublished: boolean,
    rating: number,
    publishTimestamp: Date,
    editTimestamp: Date,
    processing: boolean,
    message: string,
    confirmShowing: boolean,
    confirmAction: (Id: string)=>void
}



const ProjectShowcaseView: React.FC<IProjectShowcaseViewProps> = (props) => {
    const [data, setData] = useState<IShowcaseData[]>([]);
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(false);
    const authData = Auth.getAccessData();
    
    if(!authData) {
        redirect("/login");
        return null;
    }

    function EditShowcase(showcaseId: string) {
        navigate(`/showcases/p/edit?s=${showcaseId}`);
    }
    
    const Unpublish = async (showcaseId: string) => {
        setData((prevData) =>
            prevData.map((showcaseData) =>
                showcaseData.id === showcaseId
                ? { ...showcaseData, processing: true }
                : showcaseData
            )
        );
        await Ajax.post(`/showcases/unpublish?s=${showcaseId}`, {}).then((response) => {
            if (response.error) {
                setData((prevData) =>
                    prevData.map((showcaseData) =>
                        showcaseData.id === showcaseId
                        ? { ...showcaseData, processing: false, message: "Project showcase was not unpublished. Refresh page or try again later." }
                        : showcaseData
                    )
                );
            } else {
                setData((prevData) =>
                    prevData.map((showcaseData) =>
                        showcaseData.id === showcaseId
                        ? { ...showcaseData, isPublished: false, processing: false, message: "Project showcase was successfully unpublished." }
                        : showcaseData
                    )
                );
            }
        });
    };
    

    const Publish = async (showcaseId: string) => {
        setData((prevData) =>
            prevData.map((showcaseData) =>
                showcaseData.id === showcaseId
                ? { ...showcaseData, processing: true }
                : showcaseData
            )
        );
        await Ajax.post(`/showcases/publish?s=${showcaseId}`, {}).then((response) => {
            if (response.error) {
                setData((prevData) =>
                    prevData.map((showcaseData) =>
                        showcaseData.id === showcaseId
                        ? { ...showcaseData, processing: false, message: "Project showcase was not published. Refresh page or try again later: "+response.error }
                        : showcaseData
                    )
                );
            } else {
                setData((prevData) =>
                    prevData.map((showcaseData) =>
                            showcaseData.id === showcaseId
                            ? { ...showcaseData, isPublished: true, processing: false, message: "Project showcase was successfully published."}
                            : showcaseData
                        )
                    );
            }
        });
    };

    const DeleteShowcase = async (showcaseId: string) => {
        setData((prevData) =>
            prevData.map((showcaseData) =>
                showcaseData.id === showcaseId
                ? { ...showcaseData, processing: true }
                : showcaseData
            )
        );
        Ajax.post(`/showcases/delete?s=${showcaseId}`, {}).then((response) => {
            if (response.error) {
                setData((prevData) =>
                    prevData.map((showcaseData) =>
                        showcaseData.id === showcaseId
                        ? { ...showcaseData, processing: false, message: "Project showcase was not deleted. Refresh page or try again later." }
                        : showcaseData
                    )
                );
                getData();
            } else {
                setData((prevData) =>
                    prevData.map((showcaseData) =>
                        showcaseData.id === showcaseId
                            ? { ...showcaseData, confirmShowing: false, processing:false, message: "Project showcase deleted successfully." }
                            : showcaseData
                    )
                );
                getData();
            }
        });
    }

    const UnlinkShowcase = async (showcaseId: string) => {
        setData((prevData) =>
            prevData.map((showcaseData) =>
                showcaseData.id === showcaseId
                ? { ...showcaseData, processing: true }
                : showcaseData
            )
        );
        Ajax.post(`/showcases/unlink?s=${showcaseId}`, {}).then((response) => {
            if (response.error) {
                setData((prevData) =>
                    prevData.map((showcaseData) =>
                        showcaseData.id === showcaseId
                        ? { ...showcaseData, processing: false, message: "Project was not unlinked. Refresh page or try again later." }
                        : showcaseData
                    )
                );
            } else {
                setData((prevData) =>
                    prevData.map((showcaseData) =>
                        showcaseData.id === showcaseId
                            ? { ...showcaseData, confirmShowing: false, processing:false, message: "Project unlinked successfully." }
                            : showcaseData
                    )
                );
                getData();
            }
        });
    }

    const getData = async () => {
        await Ajax.get<IShowcaseData[]>("/showcases/user").then((response) => {
            setData(response.data && response.data.length ? response.data : []);
            setError("Unable to load project showcase. Refresh page or try again later.");
            setLoaded(response.loaded);
        });
    }

    const ShowConfirmButtons = (showcaseDatam : IShowcaseData) =>{
        return (
            <div className="h-stack" id = "confirmation-buttons">
                <Button theme={ButtonTheme.DARK} onClick={() => {
                    showcaseDatam.confirmAction(showcaseDatam.id);
                    setData((prevData) =>
                    prevData.map((showcaseData) =>
                            showcaseData.id === showcaseDatam.id
                            ? { ...showcaseData, confirmShowing: false}
                            : showcaseData
                        )
                    );
                }} title={"Confirm"}/>
                <Button theme={ButtonTheme.DARK} onClick={() => {
                    setData((prevData) =>
                    prevData.map((showcaseData) =>
                            showcaseData.id === showcaseDatam.id
                            ? { ...showcaseData, confirmShowing: false}
                            : showcaseData
                        )
                    );
                }} title={"Cancel"}/>
            </div>
        );
    }

    const ShowActionButtons = (showcaseDatam : IShowcaseData) => {
        return (
            <div className="h-stack">
                <Button theme={ButtonTheme.DARK} onClick={() => {
                    setData((prevData) =>
                    prevData.map((showcaseData) =>
                            showcaseData.id === showcaseDatam.id
                            ? { ...showcaseData, confirmShowing: true, confirmAction: DeleteShowcase }
                            : showcaseData
                        )
                    );
                }} title={"Delete"}/>
                {showcaseDatam.listingId!=null&&
                    <Button theme={ButtonTheme.DARK} onClick={() => {
                        setData((prevData) =>
                        prevData.map((showcaseData) =>
                                showcaseData.id === showcaseDatam.id
                                ? { ...showcaseData, confirmShowing: true, confirmAction: UnlinkShowcase }
                                : showcaseData
                            )
                        );
                    }} title={"Unlink"}/>
                }
                <Button theme={ButtonTheme.HOLLOW_DARK} onClick={() => {
                    EditShowcase(showcaseDatam.id);
                }} title={"Edit"}/>
            </div>
        );
    }

    const navigate = useNavigate();

    useEffect(() => {
        if (!loaded) getData();
    }, []);


    const createShowcaseTableRow = (showcaseData: IShowcaseData) => {
        const IsPublishedClass = showcaseData.isPublished ? "published-yes" : "published-no";
        showcaseData.confirmShowing == null ? false : showcaseData.confirmShowing;
        return (
            <tr key={`showcase-${showcaseData.id}`}>
                <td className="table-rating"> {showcaseData.rating}</td>
                <td className="table-listing" onClick={() => {
                    navigate('/viewListing', { state: { listingId: showcaseData.listingId} })
                }}>{showcaseData.listingId!=null && <p>Click to go to listing: {showcaseData.listingId}</p>}</td>
                <td className="table-title">
                    {showcaseData.id ?
                        <Link to={`/showcases/p/view?s=${showcaseData.id}`}>{showcaseData.title}</Link>
                        : <p>Unlinked</p>
                    }
                   
                </td>
                <td className="table-pubstatus">{
                    <svg className =  "vector-circle" width = "25" height = "25">
                        <circle cx = "12.5" cy = "12.5" r = "10" className = {showcaseData.processing ? IsPublishedClass+"-proc" : IsPublishedClass} onClick={() => {
                            if(!showcaseData.processing)
                            {
                                if(showcaseData.isPublished) {
                                    Unpublish(showcaseData.id);
                                }
                                else{
                                    Publish(showcaseData.id);
                                }
                            }
                        }}/>
                    </svg>
                }
                </td>
                <td className="table-actions">
                    {showcaseData.processing
                        ? <p>Processing...</p>
                        :showcaseData.confirmShowing ? ShowConfirmButtons(showcaseData) : ShowActionButtons(showcaseData)
                    }
                </td>
                <td className="table-message">
                    {
                        <p>{showcaseData.message}</p>
                    }
                </td>
            </tr>
        );
    }

    return (
        <div className="my-showcases-wrapper">
            <h1>My Project Showcases</h1>

            <div className="my-showcases-container">
                <Button theme={ButtonTheme.DARK} onClick={() => {
                    navigate('/showcases/p/new');
                }} title={"Create New Showcase"}/>
                <table className="my-showcases-table">
                    <thead className="my-showcases-table-header">
                        <tr>
                            <th className="header-likes">
                                <LikeButton size={"20"} defaultOn = {true} enabled={false}
                                OnUnlike={function (...args: any[]) {
                                    throw new Error("Function not implemented.");
                                } }
                                OnLike={function (...args: any[]) {
                                    throw new Error("Function not implemented.");
                                } }/>
                            </th>
                            <th className="header-listing">Linked Listing</th>
                            <th className="header-title">Project Title</th>
                            <th className="header-published">Published</th>
                            <th className="header-actions">Actions</th>
                            <th className="header-message"></th>
                        </tr>
                    </thead>
                    <tbody>
                        {data.length == 0 &&
                            <tr>
                                <td colSpan={5}>No project showcases found.</td>
                            </tr>
                        }
                        {loaded && data && data.map(value => {
                            return createShowcaseTableRow(value);
                        })}
                    </tbody>
                </table>
                {error &&
                    <p className="error">{error}</p>
                }
            </div>
        </div> 
    );
}

export default ProjectShowcaseView;