import { useState, useEffect } from "react"
import { useLocation, useNavigate } from "react-router-dom"
import { Ajax } from "../../../Ajax"
import Button, { ButtonTheme } from "../../../components/Button/Button"
import Footer from "../../../components/Footer/Footer"
import NavbarUser from "../../../components/NavbarUser/NavbarUser"
import { IListing } from "../../ListingProfilePage/MyListingsView/MyListingsView"
import { Auth } from "../../../Auth"
import "./EditListingPage.css";
import EditListingAvailabilityCard from "./EditListingAvailabilityCard/EditListingAvailabilityCard"

interface IListingPageProps {

}

interface IAvailability {
    availabilityId: number,
    startTime: Date,
    endTime: Date,
    listingId: number
}

interface IListingFile {
    link: string
}

interface IRating {
    listingId: number,
    userId: number,
    username: string,
    rating: number,
    comment?: string,
    anonymous: boolean,
    lastEdited: Date
}

interface IViewListingData {
    Listing: IListing,
    Availabilities?: IAvailability[],
    Files?: IListingFile[],
    Ratings?: IRating[]
}


const EditListingPage: React.FC<IListingPageProps> = (props) => {
    const { state } = useLocation();
    const [error, setError] = useState<string | undefined>(undefined);
    const [loaded, setLoaded] = useState<boolean>(false);
    const [data, setData] = useState<IViewListingData | null>(null);
    const navigate = useNavigate();
    const authData = Auth.getAccessData();
    const isPublished = data?.Listing.published;    
    const [currentImage, setCurrentImage] = useState<number>(0);
    const [files, setFiles] = useState<File[]>([]);
    const [fileData, setFileData] = useState<{ Item1: string, Item2: string}[] | null>([]);
    const [deletedFileNames, setDeletedFileNames] = useState<string[]>([]);
    const [listingUpdate, setListingUpdate] = useState<boolean>(false);
    const [filesUpdate, setFilesUpdate] = useState<boolean>(false);
    const [availabilitiesUpdate, setAvailabilitiesUpdate] = useState<boolean>(false);
    const [attemptPublish, setAttemptPublish] = useState<boolean>(false);
    
    useEffect(() => {
        const getData = async () => {
            const response = await Ajax.post<IViewListingData>('/listingprofile/viewListing', { listingId: state.listingId });
            setData(response.data);
            if (response.error) {
                setError("Listing failed to load. Refresh page or try again later\n" + response.error);
            }
            setLoaded(response.loaded);
        };
        getData();
    }, []);

    useEffect(() => {
        if (error !== undefined) {
          alert(error);
          setError(undefined);
        }
    }, [error]);
      
    const handlePrevImage = () => {
        setCurrentImage((prevImage) =>
          prevImage === 0 ? data!.Files!.length! - 1 : prevImage - 1
        );
    };
    
    const handleNextImage = () => {
        setCurrentImage((prevImage) =>
            prevImage === (data?.Files?.length ?? 0) - 1 ? 0 : prevImage + 1
        );
    };

    const handleDeleteImage = async (index: number) => {
        const imageName = data!.Files![index].toString().substring(data!.Files![index].toString().lastIndexOf('/') + 1);
            setDeletedFileNames(prevNames => [...prevNames, imageName]);
            handleNextImage();
        setFilesUpdate(true);
    };

    const handleInputChange = (e: { target: any }) => {
        const target = e.target;
        const name = target.name;
        const value = target.type === 'checkbox' ? target.checked : target.type === 'number' ? parseFloat(target.value) : target.value;
        
        setData({
          ...data,
          Listing: {
            ...data!.Listing,
            [name]: value
          }
        });
        setListingUpdate(true);
      };

    const handleFileSelect = (event: React.ChangeEvent<HTMLInputElement>) => {
        if (event.target.files) {
            const selectedFiles = Array.from(event.target.files);
            const modifiedFiles = selectedFiles.map((file) => {
                const modifiedFile = new File([file], file.name, { type: file.type });
                return modifiedFile;
            });
            setFiles(modifiedFiles);
        }
        setFilesUpdate(true);
    };
    
    const handlePublishSubmit = async () => {
        setAttemptPublish(true);
      
        const response = await Ajax.post<null>("/listingprofile/publishListing", { ListingId: data?.Listing.listingId });
      
        if (response.error) {
            setError("Publishing listing error. Refresh page or try again later.\n" + response.error);
            setAttemptPublish(false);
            return;
        }
      
        navigate("/viewlisting", { state: { listingId: state.listingId } });
    };
      
    useEffect(() => {
        if (attemptPublish) {
            handleSaveChanges();
        }
    }, [attemptPublish]);

    const handleDeleteClick = async () => {
        const response = await Ajax.post<null>('/listingprofile/deleteListing', { ListingId: data?.Listing.listingId })
        if (response.error) {
            setError("Listing deletion error. Refresh page or try again later.\n" + response.error);

            return;
        }
        navigate("/mylistings");
    };

    const [date, setDate] = useState('');
    const [startTime, setStartTime] = useState('');
    const [endTime, setEndTime] = useState('');
    const [timeList, setTimeList] = useState<{ date: string; startTime: string; endTime: string; }[]>([]);
    
    const handleSaveOneAvailability = (e: { preventDefault: () => void }) => {
        e.preventDefault();
        setTimeList([...timeList, { date, startTime, endTime }]);
        setDate('');
        setStartTime('');
        setEndTime('');
        setAvailabilitiesUpdate(true);
    };
          
          
    const handleSaveChanges= async() => {
        if (listingUpdate) {
            setData({
                ...data,
                Listing: {
                    ...data!.Listing,
                    listingId: data?.Listing?.listingId ?? 0,
                },
            });
            const response = await Ajax.post<null>("/listingprofile/editListing", data?.Listing );
            if (response.error) {
                setError("Listing edits error. Refresh page or try again later./n" + response.error);
                return;
            } 
        }

        if (availabilitiesUpdate) {
            const availabilities = timeList.map(time => {
                return {
                  ListingId: state.listingId,
                  Date: time.date,
                  OwnerId: data?.Listing.ownerId,
                  StartTime: time.startTime,
                  EndTime: time.endTime,
                  Action: 1
                };
              });
              const response = await Ajax.post<null>("/listingprofile/editListingAvailabilities", { reactAvailabilities: availabilities });
              if (response.error) {
                setError("Listing edits error. Refresh page or try again later.\n" + response.error);
                return;
              }
        }

        if (filesUpdate) {
            const fileDataList: { Item1: string, Item2: string }[] = [];
            try {
              await Promise.all(files.map(file => new Promise<void>((resolve, reject) => {
                const reader = new FileReader();
                reader.readAsDataURL(file);
                reader.onload = () => {
                  const dataUrl = reader.result as string;
                  const base64String = dataUrl.split(',')[1];
                  const fileData = { Item1: file.name, Item2: base64String };
                  fileDataList.push(fileData);
                  if (fileDataList.length === files.length) {
                    setFileData(fileDataList);
                  }
                  resolve();
                };
                reader.onerror = reject;
              })));
              const response = await Ajax.post<null>("/listingprofile/editListingFiles", { ListingId: data?.Listing.listingId, DeleteNames: deletedFileNames,  AddFiles: fileDataList });
              if (response.error) {
                setError(response.error+ "\nRefresh page and try again.");
                setFileData(null);
                return;
              }
            } catch (error) {
                setError(error+ "\nRefresh page and try again.");
                return;
            }
        }
        if (!attemptPublish) {
            navigate("/viewlisting", { state: { listingId: data?.Listing.listingId } });
        }
        
    }

    return (
        <div className="listing-container">
            <NavbarUser />
            {data && !error && loaded &&
            
            <div className="listing-editing-content">

                <div className="Buttons">
                    <h2>
                        {isPublished ? 'Published' : 'Draft'}
                    </h2>
                    <p><Button theme={ButtonTheme.LIGHT} title="Save Changes" onClick={ handleSaveChanges } /> </p>
                    <p>
                        { isPublished && authData?.role !== Auth.Roles.DEFAULT_USER &&  authData?.sub == data.Listing.ownerId ? (
                            <>
                            <p>
                            <Button theme={ButtonTheme.HOLLOW_DARK} onClick={() => {
                                navigate('/viewlisting', {
                            state: { listingId: data.Listing.listingId },
                            });
                            }}
                            title={'View Listing'}
                            />
                            </p>
                            <p>
                            <Button theme={ButtonTheme.HOLLOW_DARK} onClick={async () => {
                                    const response = await Ajax.post("/listingprofile/unpublishListing", { listingId: data.Listing.listingId })
                                    if (response.error) {
                                        setError("Publishing listing error. Refresh page or try again later.\n" + response.error);
                                        return;
                                    }
                                    window.location.reload(); 
                                    }} title={"Unpublish Listing"} />        
                                                           
                            </p>
                            </>
                            ) : (
                                <>
                                <p><Button theme={ButtonTheme.HOLLOW_DARK} onClick={() => {
                                    navigate('/viewlisting', {
                                        state: { listingId: data.Listing.listingId },
                                    });
                                    }}
                                    title={'Preview Listing'} /></p>
                                    
                                    <p><Button theme={ButtonTheme.HOLLOW_DARK} title={"Publish"} onClick={handlePublishSubmit } /></p>
                                    
                                </>
                        )}
                    </p>
                    
                    
                    <p><Button theme={ButtonTheme.DARK} onClick={() => { handleDeleteClick() }} title={"Delete Listing"} /></p> 
                </div>

                <div className="Title">
                    <h1 className="listing-page__title">{data.Listing.title}</h1>
                    <h2>Owner: {data.Listing.ownerUsername}</h2>
                    <h2>Last Edited: {data.Listing.lastEdited.toLocaleString()}</h2>
                </div>

                <div className="Files">
                    {data.Files && data.Files.length > 0 && (
                        <div className="listing-page__image-wrapper">
                            { data!.Files![currentImage].toString().substring(data!.Files![currentImage].toString().lastIndexOf('/') + 1) } {currentImage + 1} / {data!.Files!.length}
                                <img className="listing-page__picture" src={data.Files[currentImage]?.toString()} alt={data.Listing.title}
                                />
                                {data.Files.length > 1 && ( <>
                                    <button
                                        className="listing-page__image-nav listing-page__image-nav--prev"
                                        onClick={handlePrevImage}
                                        >
                                        &#10094;
                                    </button>
                                    <button
                                        className="listing-page__image-nav listing-page__image-nav--next"
                                        onClick={handleNextImage}
                                        >
                                        &#10095;
                                    </button>
                                    </>
                                )}
                                <p>
                                    <Button theme={ButtonTheme.DARK} title="Delete Image" onClick={() => handleDeleteImage(currentImage)} />
                                </p>
                        </div>
                    )}
                    <input type="file" accept=".jpg,.jpeg,.png,.mp4" multiple onChange={handleFileSelect} />
                    
                </div>
                <div className="Files-List">
                    { deletedFileNames.length > 0 &&
                        <div>File{deletedFileNames.length > 1 ? "s" : ""}  to delete:
                            {deletedFileNames.map(fileName => {
                                return(
                                    <li>{fileName}</li> 
                                )})
                            }
                        </div>
                    }
                    {files.length > 0 && (
                        <ul>
                            <div>File{files.length > 1 ? "s" : ""} to be added:
                                {files.map((file, index) => (
                                    <li key={file.name}>
                                        <p>{file.name}</p>
                                    </li>
                                    ))
                                }
                            </div>   
                        </ul>
                    )}
                </div>
                <div className="Information">
                    <div>
                    <label> Location:
                        <input id="location-input" type="text" name="location" value={data.Listing.location} onChange={handleInputChange} />
                    </label>
                    </div>
                    <div>
                        <label> Price: </label>
                            <input id="price-input" type="number" name="price" value={data.Listing.price} onChange={handleInputChange} />
                        
                    </div>
                    <div>
                    <label> Description:
                        <textarea id="description-input" name="description" value={data.Listing.description} onChange={handleInputChange} wrap="soft"/>
                    </label>
                    </div>
                </div>

                <div className="Availabilities">
                    <h2>Time Tracker</h2>
                        <form onSubmit={handleSaveOneAvailability}>
                            <label htmlFor="date">Date:</label>
                            <input type="date" id="date" value={date} onChange={(e) => setDate(e.target.value)} />
                            <br/>
                            <label htmlFor="start-time">Start Time:</label>
                            <input type="time" id="start-time" value={startTime} onChange={(e) => setStartTime(e.target.value)} />
                            <br/>
                            <label htmlFor="end-time">End Time:</label>
                            <input type="time" id="end-time" value={endTime} onChange={(e) => setEndTime(e.target.value)} />

                            <button type="submit">Save Time</button>
                        </form>

                    <h3>Time List:</h3>
                        <ul>
                            {timeList.map((time, index) => (
                            <li key={index}>
                                {time.date} - {time.startTime} - {time.endTime}
                            </li>
                            ))}
                        </ul>
                    <table>
                        <thead>
                            <tr>
                                <th></th>
                                <th>StartTime</th>
                                <th>EndTime</th>
                            </tr>
                        </thead>
                        <tbody>
                        { data && data.Availabilities && data.Availabilities.map((value: IAvailability) => {
                            return <EditListingAvailabilityCard ownerId={data.Listing.ownerId} availability={value} key={`${value.listingId}-listing-card`}/>
                        })}
                        </tbody>
                    </table>             
                </div>    
                {error && loaded &&
                    <p className="error">{error}</p>
                }
            </div> }
            <Footer />
        </div>
    );
}


export default EditListingPage;
