import React, {  useEffect, useState } from "react";
import { redirect, useLocation, useNavigate } from "react-router-dom";
import { Auth } from "../../Auth";
import { Ajax } from "../../Ajax";
import Footer from "../../components/Footer/Footer";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import "./EditProjectShowcasePage.css";
import LikeButton from "../../components/Heart/Heart";
import Button, { ButtonTheme } from "../../components/Button/Button";
import NavbarGuest from "../../components/NavbarGuest/NavbarGuest";
import { send } from "process";


interface IEditProjectShowcasePageProps {

}

interface IShowcaseDTO {
    listingId?: number;
    title?: string;
    description?: string;
    files?: File[];
    showcaseId?: number;
}

interface OutDTO {
    title?: string;
    description?: string;
    files?: { Item1: string, Item2: string }[];
}

const EditProjectShowcasePage: React.FC<IEditProjectShowcasePageProps> = (props) => {
    const { search } = useLocation();
    const searchParams = new URLSearchParams(search);
    const [error, setError] = useState("");
    const [title, setTitle] = useState<string | null>(null);
    const [description, setDescription] = useState<String | null>(null);
    const [originalTitle, setOriginalTitle] = useState<string | null>(null);
    const [originalDescription, setOriginalDescription] = useState<String | null>(null);
    const [files, setFiles] = useState<File[]>([]);
    const [loaded, setLoaded] = useState(false);
    const [fileData, setFileData] = useState<{ Item1: string, Item2: string} []>([]);
    const [procEdit, setProcEdit] = useState(false);
    const [listingId, setListingId] = useState<number>(0);
    const showcaseId = searchParams.get("s");
    const [showingDescription, setShowingDescription] = useState(false);
    const [sendingDescription, setSendingDescription] = useState(false);
    const [sendingTitle, setSendingTitle] = useState(false);
    const [sendingFiles, setSendingFiles] = useState(false);
    const [sendingFileOrder, setSendingFileOrder] = useState(false);
    const [fileOrder, setFileOrder] = useState<string>("");

    const authData = Auth.getAccessData();
    const navigate = useNavigate();

    
    useEffect(() => {
        setListingId(searchParams.get("l") ? parseInt(searchParams.get("l")!) : 0)
        if (!loaded) {
            Ajax.get<IShowcaseDTO>(`/showcases/view?s=${showcaseId}`)
                .then((result) => {
                    if (result.status === 200) {
                        if (result.data)
                        {
                            if (result.data.title)
                            {
                                setTitle(result.data.title);
                                setOriginalTitle(result.data.title);
                            }
                            if (result.data.description)
                            {
                                setDescription(result.data.description);
                                setOriginalDescription(result.data.description);
                            }
                        }
                        setLoaded(true);
                    }
                    else {
                        setError(result.error);
                    }
                })
                .catch((error) => {
                    console.log(error);
                });
            Ajax.get<string[]> (`/showcases/files?s=${showcaseId}`)
                .then((response) => {
                    if (response.status === 200) {
                        if (response.data) {
                            const fileDataList: { Item1: string, Item2: string }[] = [];
                            response.data.forEach((file) => {
                                const fileData = { Item1: file, Item2: "" };
                                fileDataList.push(fileData);
                            });
                            setFileData(fileDataList);
                        }
                    }
                    else {
                        setError(response.error);
                    }
                });
        }
    }, []);


    const handleFileSelect = (event: React.ChangeEvent<HTMLInputElement>) => {
        if (event.target.files) {
            const selectedFiles = Array.from(event.target.files);
            const modifiedFiles = selectedFiles.map((file) => {
                const modifiedFile = new File([file], file.name, { type: file.type });
                return modifiedFile;
            });
            setFiles(modifiedFiles);
        }
    };

    const handleFileNameChange = (index: number, name: string) => {
        const newFiles = [...files];
        newFiles[index] = new File([newFiles[index]], name, { type: newFiles[index].type });
        setFiles(newFiles);
    };
    
    const handleSubmisison = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
      
        const fileDataList: { Item1: string, Item2: string }[] = [];
      
        try {
          await Promise.all(files.map(file => new Promise<void>((resolve, reject) => {
            const reader = new FileReader();
            reader.readAsDataURL(file);
            reader.onload = () => {
              // convert data URL to base64 encoded string
              const dataUrl = reader.result as string;
              const base64String = dataUrl.split(',')[1];
      
              // add the new object with file name and base64-encoded data to the fileDataList
              const fileData = { Item1: file.name, Item2: base64String };
              fileDataList.push(fileData);
      
              // set state with updated file data list
              if (fileDataList.length === files.length) {
                // set state with the file data list
                setFileData(fileDataList);
              }
      
              resolve();
            };
      
            reader.onerror = reject;
          })));
      
          setProcEdit(true);
          if (sendingFileOrder ) {
            await Ajax.post(`/showcases/order?s=${showcaseId}`, {fileOrder: fileOrder}).then(
                (response) => {
                    if (response.error) {
                        setError(response.error);
                        console.log(response.error)
                        console.log(response)
                    }
                }
            );
          }
          console.log(sendingFiles);
          const response = await Ajax.post<string>(`/showcases/edit?s=${showcaseId}`, { files: sendingFiles ? fileDataList : null,  title: sendingTitle ? title : null, description: sendingDescription ? description : null}).then(
            (response) => 
            {
                if (response.error) {
                    setError("Project showcase was not edited."+response.error);
                    setProcEdit(false);
                  } else {
                    navigate(`/showcases/p/view?s=${showcaseId}`);
                  }
            }
          );
        } catch (error) {
          //setError(error);
        }
      };



    if (!authData) {
        redirect("/login");
        return null;
    }

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
                    <p>{currentImageIndex + 1}</p>
                </div>
            </div>
        );
    };

    const getFileStack = () => {
        if (!sendingFileOrder && !sendingFiles) 
        {
            return <div className='file-stack'>
                <div className='file-stack'>
                <Button theme={ButtonTheme.DARK} onClick={() => {
                    setSendingFiles(true);
                }} title="Upload New Files" />
            </div>
            <div className='file-order-stack'>
                <Button theme={ButtonTheme.DARK} onClick={() => {
                    setSendingFileOrder(true);
                }} title="Edit File Order" />
            </div>
            </div>
        }
        if (sendingFiles) {
            return <div className='file-stack'>
                <Button theme={ButtonTheme.DARK} onClick={() => {
                    setSendingFiles(false);
                }} title="Cancel Upload New Files" />
            </div>
        }
        else {
            return <div className='file-order-stack'>
                <ImageSlider images={fileData.map((file) => file.Item1)} />
                <Button theme={ButtonTheme.DARK} onClick={() => {
                    setSendingFileOrder(false);
                }} title="Cancel File Order" />
                <p>File Order:</p>
                <p>ex: 3 files, want to reverse order = 321. </p>
                <p>Will update on submission</p>
                <input type="text" className="file-order-input" onChange={() => {
                    validatAndSetFileOrder((document.getElementsByClassName("file-order-input")[0] as HTMLInputElement).value);
                }} />
            </div>
        }
    }

    const validatAndSetFileOrder = (fileOrder: string) => {
        if (fileOrder.length !== fileData.length) {
            console.log (fileOrder.length+"!="+fileData.length);
            setError("File order must contain all file numbers");
            return;
        }
        const fileOrderSet = new Set(fileOrder);
        if (fileOrderSet.size !== fileData.length) {
            setError("File order must contain all file numbers");
            return;
        }
        const numFileOrderSet = Array.from(fileOrder).map((fileNum: string) => parseInt(fileNum)-1);
        const neededNums = new Set(Array.from(Array(fileData.length).keys()));
        numFileOrderSet.forEach((fileNum) => {
            if (!neededNums.has(fileNum)) {
                console.log(neededNums+" does not contain "+fileNum);
                setError("File order must contain all file numbers");
                return;
            }
            neededNums.delete(fileNum);
        });
        setError("");
        setFileOrder(fileOrder);
    };


    return (
        <div className="edit-project-showcase-container">
            <NavbarUser />

            <div className="edit-project-showcase-content">
                <div className="edit-project-showcase-content-wrapper">
                    <div className="v-stack">
                        <h1>Edit Project Showcase</h1>
                        <div className='v-stack'>
                            <h3>Project Title: </h3>
                            <div className='v-stack'>
                                <h3>Original Title:</h3>
                                <p>{originalTitle}</p>
                                {sendingTitle ?
                                    <div className = 'title-stack'>
                                        <input className="input-title" type='text' value={title == null ? "" : title} onChange={() => {
                                            setTitle((document.getElementsByClassName("input-title")[0] as HTMLInputElement).value);
                                        }} />
                                        <p>{`${title?.length}/50`}</p>
                                        <Button theme={ButtonTheme.DARK} onClick={() => {
                                            setSendingTitle(false);
                                        }} title="Cancel" />
                                    </div>
                                :   <Button theme={ButtonTheme.DARK} onClick={() => {
                                        setSendingTitle(true);
                                    }} title="Edit Title" />

                                }
                            </div>
                        </div>
                        <div className='v-stack'>
                            <h3>Project Description: </h3>
                            <div className='v-stack'>
                                {showingDescription ?
                                    <div className = 'show-description-stack'>
                                        <h3>Original Description:</h3>
                                        <p>{originalDescription}</p>
                                        <Button theme={ButtonTheme.DARK} onClick={() => {
                                            setShowingDescription(false);
                                        }} title="Hide Original Description" />
                                    </div>
                                :   <Button theme={ButtonTheme.DARK} onClick={() => {
                                        setShowingDescription(true);
                                    }} title="Show Original Description"/>
                                }

                                {sendingDescription ?
                                    <div className = 'description-stack'>
                                        <textarea className="input-description" rows={10} cols={50}  onChange={() => {
                                            setDescription((document.getElementsByClassName("input-description")[0] as HTMLInputElement).value);
                                        }}>{description == null ? "" : description}</textarea>
                                        <p>{`${description?.length}/3000`}</p>
                                        <Button theme={ButtonTheme.DARK} onClick={() => {
                                            setSendingDescription(false);
                                        }} title="Cancel" />
                                    </div>
                                :   <Button theme={ButtonTheme.DARK} onClick={() => {
                                        setSendingDescription(true);
                                    }} title="Edit Description" />
                                }
                            </div>
                        </div>
                        <div className='v-stack'>
                            <h3>Upload photos/videos</h3>
                            {getFileStack()}
                            <form onSubmit={handleSubmisison}>
                                {sendingFiles && !sendingFileOrder ?
                                    <div className='file-stack'>
                                        <input type="file" accept=".jpg,.jpeg,.png" multiple onChange={handleFileSelect} />
                                        {files.length > 0 &&
                                            <ul>
                                                <div>File(s) to add:
                                                    {files.map((file, index) => (
                                                        <li key={file.name}>
                                                            <input
                                                                type="text"
                                                                value={file.name}
                                                                onChange={(e) => handleFileNameChange(index, e.target.value)}
                                                            />
                                                        </li>
                                                    ))}
                                                </div>

                                            </ul>
                                        }
                                    </div>
                                    : <></>
                                }
                                {procEdit
                                    ? <p>Processing...</p>
                                    : <div className="v-stack">
                                        <button type="submit" >Submit Edited Project Showcase</button>
                                        <button type="submit" >Preview Changes</button>
                                    </div>
                                }
                            </form>
                            <p className='error-output'>{error ? error + " Refresh page or try again later." : ""}</p>
                        </div>
                    </div>
                </div>
            </div>
            <Footer />
        </div>
    );
}

export default EditProjectShowcasePage;