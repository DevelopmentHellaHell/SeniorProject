import React, { useEffect, useState } from "react";
import { redirect, useLocation, useNavigate } from "react-router-dom";
import { Auth } from "../../Auth";
import { Ajax } from "../../Ajax";
import Footer from "../../components/Footer/Footer";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import "./ViewProjectShowcasePage.css";
import LikeButton from "../../components/Heart/Heart";
import Button, { ButtonTheme } from "../../components/Button/Button";
import NavbarGuest from "../../components/NavbarGuest/NavbarGuest";
import { set } from "cypress/types/lodash";

interface IViewProjectShowcasePageProps {

}

interface IProjectShowcase {
    id: string;
    showcaseUserId: number;
    showcaseUserEmail: string;
    listingId: number;
    title: string;
    description: string;
    reported: boolean;
    isPublished: boolean;
    rating: number;
    editTimestamp: Date;
    publishTimestamp: Date;
    liked: boolean;
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

interface IPackagedProjectShowcase {
    showcase: IProjectShowcase;
    comments: IShowcaseComment[];
    filePaths: string[];
}

const ViewProjectShowcasePage: React.FC<IViewProjectShowcasePageProps> = (props) => {
    const [comments, setComments] = useState<IShowcaseComment[]>([]);
    const [showcase, setShowcase] = useState<IProjectShowcase>();
    const [images, setImages] = useState<string[]>([]);
    const [imagesError, setImagesError] = useState("");
    const [commentsError, setCommentsError] = useState("");
    const [showcaseError, setShowcaseError] = useState("");
    const [showcaseLoaded, setShowcaseLoaded] = useState(false);
    const [commentsLoaded, setCommentsLoaded] = useState(false);
    const [imagesLoaded, setImagesLoaded] = useState(false);
    const [commentPage, setCommentPage] = useState(1);
    const [commentCount, setCommentCount] = useState(10);
    const [shared, setShared] = useState(false);
    const [reportShowing, setReportShowing] = useState(false);
    const [reportText, setReportText] = useState("");
    const [commentText, setCommentText] = useState("");
    const [showcaseLikes, setShowcaseLikes] = useState(0);


    const authData = Auth.getAccessData();
    const { search } = useLocation();
    const searchParams = new URLSearchParams(search);
    const showcaseId = searchParams.get("s");



    const navigate = useNavigate();


    const getData = async () => {
        await Ajax.get<IPackagedProjectShowcase>(`/showcases/packaged?s=${showcaseId}`).then((response) => {
            if (response.data) {
                if (response.data.comments) {
                    setComments(response.data.comments);
                }
                else {
                    setCommentsError("Unable to load comments. Refresh page or try again later.")
                }
                if (response.data.showcase) {
                    setShowcase(response.data.showcase);
                }
                else {
                    setShowcaseError("Unable to load project showcase. Refresh page or try again later.")
                }
                if (response.data.filePaths) {
                    setImages(response.data.filePaths);
                    if (response.data.filePaths.length == 0) {
                        setImagesError("No images available");
                    }
                }
                else {
                    setImagesError("Unable to load images. Refresh page or try again later.")
                }
                setShowcaseLikes(response.data.showcase.rating);
            }
            if (!response.error) {
                setShowcaseLoaded(true);
                setCommentsLoaded(true);
                setImagesLoaded(true);
            }
            else {
                setShowcaseError("Unable to load project showcase. Refresh page or try again later.");
                setCommentsError("Unable to load comments. Refresh page or try again later.")
            }
        });
    }

    const getComments = async () => {
        await Ajax.get<IShowcaseComment[]>(`/showcases/comments?s=${showcaseId}&c=${commentCount}&p=${commentPage}`).then((response) => {
            setComments(response.data && response.data.length ? response.data : []);
            setCommentsLoaded(response.loaded);
            if (response.error) {
                setCommentsError("Unable to load comments. Refresh page or try again later.");
                alert(response.error);
            }
        });
    }

    const getShowcase = async () => {
        await Ajax.get<IProjectShowcase>(`/showcases/view?s=${showcaseId}`).then((response) => {
            if (response.data) {
                setShowcase(response.data);
            }
            if (response.error){
                setShowcaseError("Unable to load project showcase. Refresh page or try again later.");
            }
            setShowcaseLoaded(response.loaded);
        });
    }

    const getImages = async () => {
        await Ajax.get<string[]>(`/showcases/files?s=${showcaseId}`).then((response) => {
            if (response.data) {
                setImages(response.data);
            }
            if (response.error) {
                setImagesError("Unable to load images. Refresh page or try again later.");
            }
            setImagesLoaded(response.loaded);
        });
    }

    useEffect(() => {
        if (!showcaseLoaded && !imagesLoaded && !commentsLoaded)
            getData();
        else {
            if (!showcaseLoaded)
                getShowcase();
            if (!imagesLoaded)
                getImages();
            if (!commentsLoaded)
                getComments();
        }
    }, []);

    useEffect(() => {
        getComments();
    }, [commentCount, commentPage])

    interface ImageSliderProps {
        images: string[];
    }

    const ImageSlider: React.FC<ImageSliderProps> = ({ images }) => {
        const [currentImageIndex, setCurrentImageIndex] = useState(0);

        const handlePrevClick = () => {
            setCurrentImageIndex(currentImageIndex - 1);
        };

        const handleNextClick = () => {
            setCurrentImageIndex(currentImageIndex + 1);
        };

        return (
            <div className="v-stack">
                <img src={images[currentImageIndex]} width={500} />
                <div className="h-stack">
                    <button onClick={handlePrevClick} disabled={currentImageIndex === 0}>
                        Prev
                    </button>
                    <button onClick={handleNextClick} disabled={currentImageIndex === images.length - 1}>
                        Next
                    </button>
                </div>
            </div>
        );
    };

    return (
        <div className="view-project-showcase-container">
            {!authData && <NavbarGuest />}
            <NavbarUser />

            <div className="view-project-showcase-content">
                <div className="view-project-showcase-wrapper">
                    {showcaseError ?
                        <div>
                            <p className='error-output'>{showcaseError}</p>
                            <button onClick={() => {
                                setShowcaseLoaded(false);
                                getShowcase();
                            }}>Reload Showcase</button>
                        </div> //TODO: add button to reload content
                        : (!(showcaseLoaded) ? <h1>Loading...</h1>
                            :
                            <div className="showcase-header">
                                <div className="h-stack">
                                    <h1>{showcase?.title}</h1>
                                    <div className="v-stack">
                                        {reportShowing ?
                                            <div className="report-container">
                                                <div className="v-stack">
                                                    <textarea className="report-input" onChange={() => {
                                                        setReportText((document.getElementsByClassName("report-input")[0] as HTMLInputElement).value)
                                                    }} />
                                                    <div className="h-stack">
                                                        <button className="report-submission-button" onClick={() => {
                                                            Ajax.post(`/showcases/report?s=${showcaseId}`, { reasonText: reportText }).then((response) => {
                                                                if (response.error) {
                                                                    alert("Project showcase was not reported. Refresh page or try again later.");
                                                                }
                                                                if (response.data) {
                                                                    alert("Project showcase reported successfully");
                                                                }
                                                            });
                                                            setReportText("");
                                                            setReportShowing(false);
                                                        }}>submit</button>
                                                        <button className="report-cancel-button" onClick={() => {
                                                            setReportText("");
                                                            setReportShowing(false);
                                                        }}>cancel</button>
                                                        <p>{`${reportText.length}/250`}</p>
                                                    </div>
                                                </div>
                                            </div>
                                            : <button className="report-button" onClick={() => {
                                                setReportShowing(true);
                                            }}>report</button>
                                        }

                                    </div>
                                </div>
                                {showcase && showcase.listingId && showcase.listingId > 0 &&
                                    <Button theme={ButtonTheme.DARK} title="Go To Listing" onClick={() => {
                                        if (showcase && showcase.listingId) {
                                            navigate(`/viewlisting`, { state: { listingId: showcase.listingId } });
                                        }
                                        else {
                                            alert("Unable to navigate to listing. Refresh page or try again later.")
                                        }
                                    }} />
                                }

                                <div className="h-stack">
                                    <LikeButton size="50" enabled={true} defaultOn={false}
                                        OnLike={() => {
                                            Ajax.post(`/showcases/like?s=${showcaseId}`, {}).then((response) => {
                                                if (!response.error) {
                                                    setShowcaseLikes(showcaseLikes + 1);
                                                }
                                                else {
                                                    alert("Project showcase like error. Unable to like project showcase. Refresh page or try again later.");
                                                }
                                            });
                                        }}
                                        OnUnlike={() => {

                                        }} />
                                    <h3 className="showcase-rating-text">{showcaseLikes} Likes</h3>
                                    <Button theme={ButtonTheme.DARK} title="Share This Showcase" onClick={() => {
                                        navigator.clipboard.writeText(`${window.location.href}`);
                                        setShared(true);
                                        alert(`Link Copied! : ${window.location.href}`)
                                    }} />
                                    {shared && <p>Link Copied!</p>}
                                </div>
                            </div>
                        )}

                    {imagesError || images.length === 0 ?
                        <div>
                            <p className='error-output'>{imagesError}</p>
                            <button onClick={() => {
                                setImagesLoaded(false);
                                getImages();
                            }}>Reload Images</button>
                        </div>

                        : (!(imagesLoaded && images && images.length > 0) ? <h1>Loading...</h1>
                            :
                            <div className="showcase-images">
                                <ImageSlider images={images} />
                            </div>
                        )}
                    {showcaseError
                        ? <p className='error-output'>{showcaseError}</p> //TODO: add button to reload content
                        : (!(showcaseLoaded) ? <h1>Loading...</h1>
                            :
                            <div className="showcase-description">
                                <h3>Description</h3>
                                <p>{showcase?.description}</p>
                            </div>
                        )}
                    {commentsError ?
                        <div>
                            <p className='error-output'>{commentsError}</p>
                            <button onClick={() => {
                                setCommentsLoaded(false);
                                getComments();
                            }}>Reload Comments</button>
                        </div>
                        : (!(commentsLoaded && comments) ? <h1>Loading Comments...</h1>
                            :
                            <div className="showcase-comments">
                                <div className="comment-input">
                                    <h3>Leave a comment</h3>
                                    <textarea className="comment-input-box" onChange={() => {
                                        setCommentText((document.getElementsByClassName("comment-input-box")[0] as HTMLInputElement).value)
                                    }}></textarea>
                                    <button onClick={() => {
                                        setCommentsLoaded(false);
                                        Ajax.post(`/showcases/comments?s=${showcaseId}`, { commentText: commentText }).then((response) => {
                                            if (response.error) {
                                                setCommentsError("Unable to load comments. Refresh page or try again later.");
                                            }
                                            else {
                                                alert("Comment submitted successfully");
                                                getComments();
                                            }
                                        });
                                    }}>Submit Comment</button>
                                </div>
                                <h3>Comments</h3>
                                <div className="comments">
                                    {comments.length === 0 ?
                                        <p>No comments yet.</p>
                                        : <div></div>
                                    }
                                    {comments.map((comment) => {
                                        return (
                                            <div className="comment" key={`comment-${comment.id}`}>
                                                <hr></hr>
                                                <div className="h-stack">
                                                    <h4>{comment.commenterEmail.split("@")[0]}</h4>
                                                    {authData && comment.commenterId == authData.sub &&
                                                        <div className="owned-comment">
                                                            <button onClick={() => {
                                                                navigate(`/showcases/p/comments/edit?cid=${comment.id}`);
                                                            }}>edit</button>
                                                            <button onClick={() => {
                                                                Ajax.post(`/showcases/comments/delete?cid=${comment.id}`, {}).then((response) => {
                                                                    if (response.error) {
                                                                        alert("Project showcase comment was not removed. Refresh page or try again later.");
                                                                    }
                                                                    if (response.data) {
                                                                        alert("Comment deleted successfully");
                                                                        getComments();
                                                                    }
                                                                });
                                                            }}>delete</button>
                                                        </div>
                                                    }
                                                </div>
                                                <p>Date: {comment.editTimestamp ? new Date(comment.editTimestamp).toUTCString() : comment.timestamp ? new Date(comment.timestamp).toUTCString() : "Unknown"}</p>
                                                <p>{comment.text}</p>
                                                <div className="vote-control">
                                                    <div className="h-stack">
                                                        <p className="down-vote" onClick={() => {
                                                            Ajax.post(`/showcases/comments/rate?s=${showcaseId}&cid=${comment.id}&r=false`, { commentText: commentText }).then((response) => {
                                                                if (response.error) {
                                                                    alert("Project showcase vote was not successful. Refresh page or try again later.");
                                                                }
                                                                if (response.data) {
                                                                    alert("Project showcase vote successful.");
                                                                    getComments();
                                                                }
                                                            });
                                                        }}>-</p>
                                                        <p className="comment-rating-text">{comment.rating}</p>
                                                        <p className="up-vote" onClick={() => {
                                                            Ajax.post(`/showcases/comments/rate?s=${showcaseId}&cid=${comment.id}&r=true`, { commentText: commentText }).then((response) => {
                                                                if (response.error) {
                                                                    alert("Project showcase vote was not successful. Refresh page or try again later.");
                                                                }
                                                                if (response.data) {
                                                                    alert("Project showcase vote successful.");
                                                                    getComments();
                                                                }
                                                            });
                                                        }}>+</p>
                                                    </div>
                                                </div>
                                            </div>
                                        );
                                    })}
                                </div>
                                <div className="comment-control">
                                    <div className="comment-count-control">
                                        <p>comments per page:</p>
                                        <div className="h-stack">
                                            <p className={commentCount == 10 ? "selected-comment-count" : "unselected-comment-count"}
                                                onClick={() => {
                                                    if (commentCount != 10) {
                                                        setCommentsLoaded(false);
                                                        setCommentPage(1);
                                                        setCommentCount(10);
                                                    }
                                                }}>10</p>
                                            <p className={commentCount == 20 ? "selected-comment-count" : "unselected-comment-count"}
                                                onClick={() => {
                                                    if (commentCount != 20) {
                                                        setCommentsLoaded(false);
                                                        setCommentPage(1);
                                                        setCommentCount(20);
                                                    }
                                                }}>20</p>
                                            <p className={commentCount == 50 ? "selected-comment-count" : "unselected-comment-count"}
                                                onClick={() => {
                                                    if (commentCount != 50) {
                                                        setCommentsLoaded(false);
                                                        setCommentPage(1);
                                                        setCommentCount(50);
                                                    }
                                                }}>50</p>
                                        </div>
                                    </div>
                                    <div className="comment-page-control">
                                        <p>page #:</p>
                                        <div className="h-stack">
                                            {commentPage > 1 &&
                                                <p className="prev-page" onClick={() => {
                                                    setCommentPage(commentPage - 1);
                                                    getComments();
                                                }}>&lt;</p>
                                            }
                                            <p>{commentPage}</p>
                                            {comments.length == commentCount &&
                                                <p className="next-page" onClick={() => {
                                                    setCommentPage(commentPage + 1);
                                                    getComments();
                                                }}>&gt;</p>
                                            }
                                        </div>
                                    </div>
                                </div>
                            </div>
                        )}
                </div>
            </div>
            <Footer />
        </div>
    );
}

export default ViewProjectShowcasePage;