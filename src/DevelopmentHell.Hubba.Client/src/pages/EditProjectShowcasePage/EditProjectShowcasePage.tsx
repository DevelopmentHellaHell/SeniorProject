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


interface IEditProjectShowcasePageProps {

}
/*
public struct ShowcaseDTO
{
    public int? ListingId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public List<IFormFile>? Files { get; set; }
}
*/

interface IShowcaseDTO {
    listingId?: number;
    title?: string;
    description?: string;
    files?: File[];
    showcaseId?: number;
}



const EditProjectShowcasePage: React.FC<IEditProjectShowcasePageProps> = (props) => {
    const { search } = useLocation();
    const searchParams = new URLSearchParams(search);
    const [error, setError] = useState("");
    const [title, setTitle] = useState<string | null>(null);
    const [description, setDescription] = useState<String | null>(null);
    const [files, setFiles] = useState<File[]>([]);
    const [fetchResult, setFetchResult] = useState();
    const [uploadResponse, setUploadResponse] = useState<IShowcaseDTO>();
    const [data, setData] = useState<IShowcaseDTO | null>(null);
    const [fileData, setFileData] = useState<{ Item1: string, Item2: string} []>([]);
    const [listingId, setListingId] = useState<number>(searchParams.get("l") ? parseInt(searchParams.get("l")!) : 0);
    const showcaseId = searchParams.get("s");

    const authData = Auth.getAccessData();
    const navigate = useNavigate();

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
                console.log("file data: ", fileDataList);
              }
      
              resolve();
            };
      
            reader.onerror = reject;
          })));
      
          console.log("file data: ", fileDataList);
          const response = await Ajax.post<string>(`/showcases/edit?s=${showcaseId}`, { files: fileDataList,  title: title, description: description, listingId:  listingId });
      
          if (response.error) {
            setError(response.error);
            console.log(response.error)
            console.log(response)
          } else {
            navigate(`/showcases/view?s=${showcaseId}`);
          }
        } catch (error) {
          //setError(error);
          console.log(error);
        }
      };



    if (!authData) {
        redirect("/login");
        return null;
    }

    return (
        <div className="edit-project-showcase-container">
            {!authData && <NavbarGuest />}
            <NavbarUser />

            <div className="edit-project-showcase-content">
                <div className="edit-project-showcase-content-wrapper">
                    <h1>Edit Project Showcase</h1>
                    <div className='h-stack'>
                        <h4>Project Title: </h4>
                        <div className='v-stack'>
                            <input className="input-title" type='text' onChange={() => {
                                setTitle((document.getElementsByClassName("input-title")[0] as HTMLInputElement).value);
                            }}/>
                            <text>{`${title?.length}/50`}</text>
                        </div>
                    </div>
                    <div className='h-stack'>
                        <h4>Project Description: </h4>
                        <div className='v-stack'>
                            <textarea className="input-description" rows={10} cols={50} onChange={() => {
                                setDescription((document.getElementsByClassName("input-description")[0] as HTMLInputElement).value);
                            }}/>
                            <text>{`${description?.length}/3000`}</text>
                        </div>
                    </div>
                    <div className='h-stack'>
                        <h4>Upload photos/videos</h4>
                        <form onSubmit={handleSubmisison}>
                            <input type="file" accept=".jpg,.jpeg,.png" multiple onChange={handleFileSelect}/>
                            {files.length >0 &&
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
                        <button type="submit" >Edit Project Showcase</button>
                        <button type="submit" >Preview Changes</button>
                        </form>
                    </div>
                </div>
            </div>
            <p className='error-output'>{error ? error + " please try again later" : ""}</p>
            <Footer />
        </div> 
    );
}

export default EditProjectShowcasePage;