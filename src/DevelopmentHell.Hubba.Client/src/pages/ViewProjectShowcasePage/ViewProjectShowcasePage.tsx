import React, {  useEffect, useState } from "react";
import { redirect, useLocation, useNavigate } from "react-router-dom";
import { Auth } from "../../Auth";
import { Ajax } from "../../Ajax";
import Footer from "../../components/Footer/Footer";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import "./ViewProjectShowcasePage.css";
import LikeButton from "../../components/Heart/Heart";
import Button, { ButtonTheme } from "../../components/Button/Button";
import NavbarGuest from "../../components/NavbarGuest/NavbarGuest";

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
    const [commentsLoaded, setCommentsLoaded] = useState(false);
    const [showcase, setShowcase] = useState<IProjectShowcase>();
    const [images, setImages] = useState<string[]>([]);
    const [error , setError] = useState("");
    const [loaded, setLoaded] = useState(false);
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
        await Ajax.get<IPackagedProjectShowcase>(`/showcases/view?s=${showcaseId}`).then((response) => {
            if(response.data) {
                setComments(response.data.comments);
                setShowcase(response.data.showcase);
                setImages(response.data.filePaths);
                setShowcaseLikes(response.data.showcase.rating);
            }
            setError(response.error);
            setLoaded(response.loaded);
            console.log(`${images}`)
        });
    }

    const getComments = async() => {
        await Ajax.get<IShowcaseComment[]>(`/showcases/comments?s=${showcaseId}&c=${commentCount}&p=${commentPage}`).then((response) => {
            setComments(response.data && response.data.length ? response.data : []);
            alert(response.error);
        });
    }

    useEffect(() => {
        if (!loaded)
            getData();
    }, []);

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
          <div>
            <img src={images[currentImageIndex]} width={500}/>
            <button onClick={handlePrevClick} disabled={currentImageIndex === 0}>
              Prev
            </button>
            <button onClick={handleNextClick} disabled={currentImageIndex === images.length - 1}>
              Next
            </button>
          </div>
        );
      };

    return (
        <div className="view-project-showcase-container">
            {!authData && <NavbarGuest />}
            <NavbarUser />

            <div className="view-project-showcase-content">
                {error
                            ? <p className='error-output'>{error}</p>
                            : ( !(loaded && images && images.length > 0) ?  <h1>Loading...</h1> :
                <div className="view-project-showcase-wrapper">
                    <div className="showcase-header">
                        <div className="h-stack"> 
                            <h1>{showcase?.title}</h1>
                            <div className="v-stack">
                                {reportShowing ? 
                                    <div className="report-container">
                                        <div className="v-stack">
                                            <textarea className="report-input" onChange={() =>{
                                                setReportText((document.getElementsByClassName("report-input")[0] as HTMLInputElement).value)
                                            }}/>
                                            <div className="h-stack">
                                                <button className="report-submission-button" onClick={() => {
                                                    Ajax.post(`/showcases/report?s=${showcaseId}`, { reasonText: reportText }).then((response) => {
                                                        if (response.error){
                                                            alert(response.error);
                                                        }
                                                        if(response.data) {
                                                            alert("Report submitted successfully");
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
                        <Button theme={ButtonTheme.DARK} title="Go To Listing" onClick={() => {
                            navigate('/viewListing', { state: { listingId: showcase?.listingId} })
                        }}/>
                        <div className="h-stack">
                            <LikeButton size="50" enabled={true} defaultOn={false} 
                            OnLike={ () => {
                                Ajax.post(`/showcases/like?s=${showcaseId}`, { }).then((response) => {
                                    if (!response.error){
                                        console.log("Liked");
                                        setShowcaseLikes(showcaseLikes + 1);
                                    }
                                    else {
                                        alert("Unable to like at this time.");
                                    }
                                });
                            }} 
                            OnUnlike={ () => {
                                console.log("Unliked");
                            }}/>
                            <h3 className="showcase-rating-text">{showcaseLikes} Likes</h3>
                            <Button theme={ButtonTheme.DARK} title="Share This Showcase" onClick={() => { 
                                navigator.clipboard.writeText(`${window.location.href}`);
                                setShared(true);
                                alert(`Link Copied! : ${window.location.href}`)
                            }}/>
                            {shared && <p>Link Copied!</p>}
                        </div>
                    </div>
                    <div className="showcase-images">
                        <ImageSlider images={images} />
                    </div>
                    <div className="showcase-description">
                        <h3>Description</h3>
                        <p>{showcase?.description}</p>
                    </div>
                    <div className="showcase-comments">
                        <div className="comment-input">
                            <h3>Leave a comment</h3>
                            <textarea className="comment-input-box" onChange={() => {
                                setCommentText((document.getElementsByClassName("comment-input-box")[0] as HTMLInputElement).value)
                            }}></textarea>
                            <button onClick={() => {
                                console.log(commentText);
                                Ajax.post(`/showcases/comments?s=${showcaseId}`, { commentText: commentText } ).then((response) => {
                                    if (response.error){
                                        alert(response.error);
                                    }
                                    if(response.data) {
                                        alert("Comment submitted successfully");
                                        getComments();
                                    }
                                });
                            }}>Submit Comment</button>
                        </div>
                        <h3>Comments</h3>
                        <div className="comments">
                            {comments.map((comment) => {
                                return (
                                    <div className="comment" >
                                        <hr></hr>
                                        <div className="h-stack">
                                            <h4>{comment.commenterEmail.split("@")[0]}</h4>
                                            {authData && comment.commenterId.toString() == authData.sub &&
                                                <div className="owned-comment">
                                                    <button onClick={() => {
                                                        navigate(`/showcases/comments/edit?cid=${comment.id}`);
                                                    }}>edit</button>
                                                    <button onClick={() => {
                                                        Ajax.post(`/showcases/comments/delete?cid=${comment.id}`,{}).then((response) => {
                                                            if (response.error){
                                                                alert(response.error);
                                                            }
                                                            if(response.data) {
                                                                alert("Comment deleted successfully");
                                                                getComments();
                                                            }
                                                        });
                                                    }}>delete</button>
                                                </div>
                                            }
                                        </div>
                                        <p>{comment.text}</p>
                                        <div className="vote-control">
                                            <div className="h-stack">
                                                <p className="down-vote" onClick={() => {
                                                    Ajax.post(`/showcases/comments/rate?s=${showcaseId}&cid=${comment.id}&r=false`, { commentText: commentText } ).then((response) => {
                                                        if (response.error){
                                                            alert(response.error);
                                                        }
                                                        if(response.data) {
                                                            alert("Comment submitted successfully");
                                                            getComments();
                                                        }
                                                    });
                                                }}>-</p> 
                                                <p className="comment-rating-text">{comment.rating}</p>
                                                <p className="up-vote" onClick={() => {
                                                    Ajax.post(`/showcases/comments/rate?s=${showcaseId}&cid=${comment.id}&r=true`, { commentText: commentText } ).then((response) => {
                                                        if (response.error){
                                                            alert(response.error);
                                                        }
                                                        if(response.data) {
                                                            alert("Comment submitted successfully");
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
                                            setCommentCount(10);
                                            getComments();
                                        }
                                    }}>10</p>
                                    <p className={commentCount == 20 ? "selected-comment-count" : "unselected-comment-count"}
                                    onClick={() => {
                                        if (commentCount != 20) {
                                            setCommentCount(20);
                                            getComments();
                                        }
                                    }}>20</p>
                                    <p className={commentCount == 50 ? "selected-comment-count" : "unselected-comment-count"}
                                    onClick={() => {
                                        if (commentCount != 50) {
                                            setCommentCount(50);
                                            getComments();
                                        }
                                    }}>50</p>
                                </div>
                            </div>
                            <div className="comment-page-control">
                                <p>page #:</p>
                                <div className="h-stack"> 
                                    {commentPage > 1 &&
                                        <p className="prev-page"  onClick={() => {
                                            setCommentPage(commentPage - 1);
                                            getComments();
                                        }}>&lt;</p>
                                    }
                                    <p>{commentPage}</p>
                                    {comments.length == commentCount &&
                                        <p className="next-page" onClick={() => {
                                            setCommentPage(commentPage - 1);
                                            getComments();
                                        }}>&gt;</p>
                                    }
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                )}
            </div>
            <p className='error-output'>{error ? error + " please try again later" : ""}</p>
            <Footer />
        </div> 
    );
}

export default ViewProjectShowcasePage;