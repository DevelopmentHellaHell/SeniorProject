import { Auth } from "../../../../Auth";
import { useEffect, useState } from "react";
import Button from "../../../../components/Button/Button";
import { Ajax } from "../../../../Ajax";
import "./CollaboratorEditView.css";
import { ICollaboratorData } from "../../../CollaboratorPage/CollaboratorPage";
import { redirect, useNavigate } from "react-router-dom";
import { remove } from "cypress/types/lodash";

interface ICollaboratorEditPageProps {
    collaboratorId?: number,
}

export interface ICollaboratorSubmissionData {
    Name: string,
    ContactInfo: string,
    Tags?: string | undefined,
    Description: string,
    Availability?: string | undefined,
    Published: boolean,
    PfpFile?: FormData | undefined,
    UploadedFiles: FormData[],
    RemovedFiles?: string[] | undefined
}

const CollaboratorEditPage: React.FC<ICollaboratorEditPageProps> = (props) => {
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState("");
    const [loaded, setLoaded] = useState<boolean>(true);
    const [data, setData] = useState<ICollaboratorData | null>(null);
   
    const authData = Auth.getAccessData();

    const [published, setPublished] = useState<boolean>(false);
    const [name, setName] = useState<string>("");
    const [tags, setTags] = useState<string | null>("");
    const [newTag, setNewTag] = useState("");
    const [description, setDescription] = useState<string>("");
    const [contactInfo, setContactInfo] = useState<string>("");
    const [availability, setAvailability] = useState<string | null>("");
    const [profilePicture, setProfilePicture] = useState<File | null>(null);
    const [oldProfilePicture, setOldProfilePicture] = useState<string | null>("");
    const [uploadedPhotos, setUploadedPhotos] = useState<FileList>();
    const [oldPhotos, setOldPhotos] = useState<string[] | null>([]);
    const [removedFiles, setRemovedFiles] = useState<string[]>([]);


    if(!authData){
        redirect("/login")
        return null;
    }

    useEffect(() => {
        console.log("CollaboratorId: " + props.collaboratorId)
        if(props.collaboratorId){
            const getData = async () => {
                // console.log("Ran getData")
                const response = await Ajax.post<ICollaboratorData>("collaborator/getcollaborator", {CollaboratorId: props.collaboratorId});
                setData(response.data);
                setError(response.error);
                setLoaded(response.loaded);

                if (response.data) {
                    setPublished(response.data.published);
                    setName(response.data.name);
                    setTags(response.data.tags || "");
                    setDescription(response.data.description);
                    setContactInfo(response.data.contactInfo || "");
                    setAvailability(response.data.availability || "");
                    setOldProfilePicture(response.data.pfpUrl || "");
                    setOldPhotos(response.data.collabUrls || []);
                }
            };
            getData();
        }
        
    }, [props.collaboratorId]);

    // const handleImageClick = (imageUrl: string) => {
    //     removedFiles?.push(imageUrl);
    // }

    const handleImageClick = (url: string) => {
        setRemovedFiles([...removedFiles, url]);
        if(oldPhotos){
            const filteredPhotos = oldPhotos.filter((photoUrl) => photoUrl !== url);
            setOldPhotos(filteredPhotos);
        }
    };

    function handleAddTag(){
        if (tags){
            setTags(tags+","+newTag);
        }
        else{
            setTags(newTag)
        }
        setNewTag("")
    }

    // function handleImageClick(url: string) {
    //     // Remove the clicked image from the list of oldPhotos
    //     const updatedOldPhotos = oldPhotos.filter((photoUrl) => photoUrl !== url);
    //     setOldPhotos(updatedOldPhotos);
    
    //     // Set the selected image URL to null or an empty string to make it disappear
    //     setSelectedImageUrl(null); // or setSelectedImageUrl('');
    // }
    


    const handleSubmit = async (event: React.FormEvent) => {
        setError(null)
        event.preventDefault();
        const formData = new FormData();
        formData.append("Published", published.toString());
        formData.append("Name", name);
        if(tags){
            formData.append("Tags", tags);
        }
        formData.append("Description", description);
        formData.append("ContactInfo", contactInfo);
        if(availability){
            formData.append("Availability", availability);
        }
        if (profilePicture) {
          formData.append("PfpFile", profilePicture);
        }
        if(uploadedPhotos){
            var length = uploadedPhotos.length;
            for(var i=0;i<length;i++){
                console.log(uploadedPhotos[i])
                formData.append("UploadedFiles", uploadedPhotos[i]);
            }
        }
        if(removedFiles){
            removedFiles.forEach((file) =>
            formData.append('RemovedFiles', file));
        }
        if(props.collaboratorId){
            const response = await Ajax.postFormData("/collaborator/editcollaborator", formData);
            console.log(response);
            if(response.error){
                setError(response.error);
                return;
            }
            setSuccess("Successfully submitted.");
            setError(null);
        }
        else{
            const response = await Ajax.postFormData("/collaborator/createcollaborator", formData);
            console.log(response);
            if(response.error){
                setError(response.error)
                return;
            }
            setSuccess("Successfully submitted.");
            setError(null);
        }
        
    };

    return(
        <div className="edit-collaborator-wrapper">
            <h1 id="edit-collaborator-header">
                {props.collaboratorId? 'Edit Collaborator' : 'Create Collaborator'}
            </h1>
            
            <div id="collaborator-page-form" className="collaborator-page-form">
                <form onSubmit={handleSubmit}>
                    <label htmlFor="published">Published: </label>
                    <select
                        id="published"
                        value={published ? published.toString() : "false"}
                        onChange={(event) => setPublished(event.target.value === "true")}
                    >
                        <option value="true">Yes</option>
                        <option value="false">No</option>
                    </select>
                    <br />
                    <br />
                    <label htmlFor="name">Name: </label>
                    <input
                        id="name"
                        type="text"
                        value={name? name : ""}
                        maxLength={70}
                        onChange={(event) => setName(event.target.value)}
                        required
                    />
                    <div>Limit 70 char</div>
                    <br />
                    <label htmlFor="tags">Tags: </label>
                    <textarea
                        id="tags"
                        readOnly={true}
                        value={tags? tags : ""}
                        maxLength={2000}
                        onChange={(event) =>
                            setTags(event.target.value.replace(/[^A-Za-z\s]/g, ""))
                        }
                    />
                    <div>Limit 2000 char</div>
                    <br />
                    <label htmlFor="newTag">New tag: </label>
                    <input
                        id="newTag"
                        type="text"
                        value={newTag}
                        maxLength={100}
                        onChange={(event) =>
                            setNewTag(event.target.value.replace(/[^A-Za-z\s]/g, ""))
                        }
                    />
                    <div>Limit 100 char</div>
                    <br />
                    <button type="button" onClick={handleAddTag}>Add Tag</button>
                    <button type="button" onClick={() => setTags("")}>Clear All Tags</button>
                    <br />
                    <br />
                    <label htmlFor="description">Description: </label>
                    <textarea
                        id="description"
                        value={description? description : ""}
                        maxLength={10000}
                        onChange={(event) => setDescription(event.target.value)}
                        required
                    />
                    <div>Limit 10000 char</div>
                    <br />
                    <br />
                    <label htmlFor="contactInfo">Contact Information: </label>
                    <input
                        id="contactInfo"
                        type="text"
                        maxLength={1000}
                        value={contactInfo? contactInfo : ""}
                        onChange={(event) => setContactInfo(event.target.value)}
                        required
                    />
                    <div>Limit 1000 char</div>
                    <br />
                    <br />
                    <label htmlFor="availability">Availability: </label>
                    <input
                        id="availability"
                        type="text"
                        name="availability"
                        maxLength={1000}
                        value={availability? availability : ""}
                        onChange={(e) => setAvailability(e.target.value)}
                    />
                    <div>Limit 1000 char</div>
                    <br />
                    <div id="profile-picture-wrapper">
                        <label htmlFor="profilePic">Upload Profile Picture: </label>
                        <input
                            type="file"
                            name="profilePic"
                            accept="image/"
                            onChange={(e) => setProfilePicture(e.target.files![0])}
                        />
                    </div>
                    <div id="collab-pictures-wrapper">
                        <label htmlFor="photos">Upload Photos: </label>
                        <input
                            type="file"
                            name="photos"
                            accept="image/"
                            multiple
                            onChange={(e) => setUploadedPhotos(e.target.files!)}
                            required = {props.collaboratorId? false : true}
                        />
                    </div>
                    <div className="collaborator-page-image-wrapper">
                        <div id="collaborator-page-images" className="collaborator-page-images">
                            <p>Previously uploaded images displayed below. Click to remove.</p>
                            <div>{oldProfilePicture? "Profile Picture" : ""}</div>
                            {oldProfilePicture &&
                                <img id="collaborator-page-image" className="collaborator-page-image"
                                src={removedFiles.includes(oldProfilePicture) ? undefined : oldProfilePicture} 
                                onClick={() => {
                                  handleImageClick(oldProfilePicture);
                                }}
                              />
                            }
                            <br/>
                            <div>{oldPhotos? "Collaborator Picture" : ""}{(oldPhotos && oldPhotos?.length > 1)? "s" : ""}</div>
                            {oldPhotos && oldPhotos.map((url) => (
                                <img id="collaborator-page-image" className="collaborator-page-image"
                                key={url} src={url} 
                                onClick={() => {
                                    handleImageClick(url);
                                }}/>
                            ))}
                        </div>
                    </div>
                        
                    <button type="submit">Submit</button>
                </form>
            </div>
            {/* <div id="submit-button" className="buttons">           
                <Button title="Submit" loading={!loaded} onClick={async () => {
                    setLoaded(false);
                    setSuccessLoaded(false);
                    setErrorLoaded(false);
                    if(!accountId){
                        onError("Unable to find selected account of collaborator.");
                        return;
                    }

                    const response = await Ajax.get("/collaborator/health");
                    if (response.error) {
                        onError(response.error);
                        setLoaded(true);
                        return;
                    }
                    setSuccessLoaded(true);
                    setLoaded(true);
                    setSuccess("Your collaborator profile has been successfully submitted.");
                }}/>
            </div> */}
            {error &&
                <p id='error' className="error">{error}</p>
            }
            {success &&
                <p id='success' className="success">{success}</p>
            }
        </div> 
    );
}

export default CollaboratorEditPage