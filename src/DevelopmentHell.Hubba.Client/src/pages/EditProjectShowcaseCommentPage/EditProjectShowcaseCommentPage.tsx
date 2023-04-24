import React, {  useEffect, useState } from "react";
import { redirect, useLocation, useNavigate } from "react-router-dom";
import { Auth } from "../../Auth";
import { Ajax } from "../../Ajax";
import Footer from "../../components/Footer/Footer";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import "./EditProjectShowcaseCommentPage.css";
import LikeButton from "../../components/Heart/Heart";
import Button, { ButtonTheme } from "../../components/Button/Button";
import NavbarGuest from "../../components/NavbarGuest/NavbarGuest";


interface IEditProjectShowcaseCommentPageProps {

}

interface IShowcaseDTO {
    listingId?: number;
    title?: string;
    description?: string;
    files?: File[];
    showcaseId?: number;
}

interface IShowcaseComment {
    id: number;
    commenterId: number;
    commenterEmail: string;
    showcaseId: string;
    text: string;
    rating: number;
    reported: boolean;
    timestamp: Date;
    editTimestamp: Date;
}

const EditProjectShowcaseCommentPage: React.FC<IEditProjectShowcaseCommentPageProps> = (props) => {
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(false);
    const [showcaseId, setShowcaseId] = useState("");
    const { search } = useLocation();
    const searchParams = new URLSearchParams(search);
    const commentId = searchParams.get("cid");

    const authData = Auth.getAccessData();
    const navigate = useNavigate();

    
    const [commentText, setCommentText] = useState<string | undefined>("");



    if (!authData) {
        redirect("/login");
        return null;
    }

    useEffect (() => {
        Ajax.get<IShowcaseComment>(`/showcases/comment?cid=${commentId}`)
            .then((result) => {
                if (!result.error) {
                    setCommentText(result.data?.text);
                    setLoaded(true);
                    setShowcaseId(result.data?.showcaseId.toString() ?? "");
                } else {
                    setError(result.error);
                }
            })
            .catch((error) => {
                setError(error);
            });
    }, []);


    return (
        <div className="edit-project-showcase-comment-container">
            {!authData && <NavbarGuest />}
            <NavbarUser />

            <div className="edit-project-showcase-comment-content">
                <div className="edit-project-showcase-comment-wrapper">
                    <h1>Edit Comment</h1>
                    {
                        loaded ? 
                        <div className="v-stack">
                            <textarea className="edit-comment-text" value={commentText} onChange={() => {
                                setCommentText((document.getElementsByClassName("edit-comment-text")[0] as HTMLInputElement).value);
                            }}></textarea>
                            <Button theme={ButtonTheme.DARK} title="Edit Comment" onClick={() => {
                                Ajax.post<IShowcaseComment>(`/showcases/comments/edit?cid=${commentId}`, {
                                    commentText: commentText
                                })
                                    .then((result) => {
                                        if (!result.error) {
                                            navigate(`/showcases/view?s=${showcaseId}`);
                                        } else {
                                            setError(result.error);
                                        }
                                    })
                                    .catch((error) => {
                                        setError(error);
                                    });
                            }} />
                        </div>
                        : <p>Loading...</p>
                    }
                    
                </div>
            </div>
            <p className='error-output'>{error ? error + " please try again later" : ""}</p>
            <Footer />
        </div>
    );
}

export default EditProjectShowcaseCommentPage;