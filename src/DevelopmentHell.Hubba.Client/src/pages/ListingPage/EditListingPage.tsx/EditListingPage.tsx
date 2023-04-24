import { useState, useEffect } from "react"
import { useLocation, useNavigate } from "react-router-dom"
import { Ajax } from "../../../Ajax"
import Button, { ButtonTheme } from "../../../components/Button/Button"
import Footer from "../../../components/Footer/Footer"
import NavbarUser from "../../../components/NavbarUser/NavbarUser"
import { IListing } from "../../ListingProfilePage/MyListingsView/MyListingsView"
import { Auth } from "../../../Auth"
import "./EditListingPage.css";

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
    const [isListingEdited, setIsListingEdited] = useState(false);
    const [showPublish, setShowPublish] = useState(false);
    const [files, setFiles] = useState<File[]>([]);
    const [deletedImageName, setDeletedImageName] = useState("");
    const [fileData, setFileData] = useState<{ Item1: string, Item2: string} []>([]);
    const [deletedFileNames, setDeletedFileNames] = useState<string[]>([]);
    
    useEffect(() => {
        const getData = async () => {
            const response = await Ajax.post<IViewListingData>('/listingprofile/viewListing', { listingId: state.listingId });
            setData(response.data);
            if (response.error) {
                setError(response.error);
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
        console.log(imageName)
        setDeletedFileNames(prevNames => [...prevNames, imageName]);
        setCurrentImage(index+1); // show the first image after deletion
        
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
      };
      

      const handleListingSubmit = async (e: { preventDefault: () => void }) => {
        e.preventDefault();
        setData({
          ...data,
          Listing: {
            ...data!.Listing,
            listingId: data?.Listing?.listingId ?? 0, // set listingId to a default value of 0 if it's not defined
          },
        });
      
        const response = await Ajax.post<null>("/listingprofile/editListing", data?.Listing );
        if (response.error) {
            setError(response.error);
        } else {
            navigate("/viewlisting", { state: { listingId: data?.Listing.listingId } });
        }
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
      };


    
    const handleFileSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
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
          const response = await Ajax.post<null>("/listingprofile/editListingFiles", { ListingId: data?.Listing.listingId, DeleteNames: deletedFileNames,  AddFiles: fileDataList });
      
          if (response.error) {
            setError(response.error);
            console.log(response.error)
            console.log(response)
          } else {
            navigate("/viewlisting", { state: { listingId: data?.Listing.listingId } });
          }
        } catch (error) {
          //setError(error);
          console.log(error);
        }
      };

      const handleFileNameChange = (index: number, name: string) => {
        const newFiles = [...files];
        newFiles[index] = new File([newFiles[index]], name, { type: newFiles[index].type });
        setFiles(newFiles);
      };
    
    const handlePublishSubmit = async () =>  {
        const response = await Ajax.post<null>("/listingprofile/publishListing", { ListingId: data?.Listing.listingId});
        if (response.error) {
            setError(response.error);
            return;
        }
        return navigate("/viewlisting", { state: {listingId: state.listingId}});
    }


    const handleDeleteClick = async () => {
        const response = await Ajax.post<null>('/listingprofile/deleteListing', { ListingId: data?.Listing.listingId })
        console.log(response)
        if (response.error) {
            setError(response.error);
            return;
        }
        navigate("/listingprofile");
    };

    const [date, setDate] = useState('');
    const [startTime, setStartTime] = useState('');
    const [endTime, setEndTime] = useState('');
    const [timeList, setTimeList] = useState<{ date: string; startTime: string; endTime: string; }[]>([]);

    
    const handleSubmit = (e: { preventDefault: () => void }) => {
        e.preventDefault();
        setTimeList([...timeList, { date, startTime, endTime }]);
        setDate('');
        setStartTime('');
        setEndTime('');
    };
          
          
    return (
        <div className="listing-container">
            <NavbarUser />
            {data && !error && loaded &&
            
            <div className="listing-content">
                <div className="listing-wrapper">
                    
                        <div className="listing-page">
                        <div className="listing-page-status">{isPublished ? 'Published' : 'Draft'}</div>
                        <p>
                          { isPublished && authData?.sub == data.Listing.ownerId.toString() ? (
                            <Button
                              theme={ButtonTheme.DARK}
                              onClick={() => {
                                navigate('/viewlisting', {
                                  state: { listingId: data.Listing.listingId },
                                });
                              }}
                              title={'View Listing'}
                            />
                          ) : (
                            <Button
                              theme={ButtonTheme.DARK}
                              onClick={() => {
                                navigate('/viewlisting', {
                                  state: { listingId: data.Listing.listingId },
                                });
                              }}
                              title={'Preview Listing'}
                            />
                          )}
                          <Button theme={ButtonTheme.DARK} onClick={() => { handleDeleteClick() }} title={"Delete Listing"} />
                        </p>
                        <h2 className="listing-page__title">{data.Listing.title}</h2>
                        {data.Files && data.Files.length > 0 && (
                          <div className="listing-page__image-wrapper">
                            { data!.Files![currentImage].toString().substring(data!.Files![currentImage].toString().lastIndexOf('/') + 1) } {currentImage + 1} / {data!.Files!.length}
                          <img
                            className="listing-page__picture"
                            src={data.Files[currentImage]?.toString()}
                            alt={data.Listing.title}
                          />
                          {data.Files.length > 1 && (
                            <>
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
                          {deletedImageName && (
                            <div className="listing-page__deleted-image">
                              {`Deleted image: ${deletedImageName}`}
                            </div>
                          )}
                          <button
                            className="listing-page__delete-image"
                            onClick={() => handleDeleteImage(currentImage)}
                          >
                            Delete Image
                          </button>
                        </div>
                          
                        )}
                        { deletedFileNames.length > 0 &&
                            <div>File(s) to delete:
                            { deletedFileNames.map(fileName => {
                                return(
                                    
                                        <li>{fileName}</li>
                                    
                                )}) 
                            }
                            </div>
                        }
                        
                       <form onSubmit={handleFileSubmit}>
                            <input type="file" accept=".jpg,.jpeg,.png,.mp4" multiple onChange={handleFileSelect} />
                            {files.length > 0 && (
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
                            )}
                            <button type="submit">Submit file changes</button>
                            </form>
                        <form onSubmit={handleListingSubmit}>
                            <div>
                                <label>
                                Price:
                                <input type="number" name="price" value={data.Listing.price} onChange={handleInputChange} />
                                </label>
                            </div>
                            <div className="detailItem">
                                <label>
                                Description:
                                <textarea name="description" value={data.Listing.description} onChange={handleInputChange} />
                                </label>
                            </div>
                            <div className="detailItem">
                                <label>
                                Location:
                                <input type="text" name="location" value={data.Listing.location} onChange={handleInputChange} />
                                </label>
                            </div>
                            <div className="buttons">
                                <Button theme={ButtonTheme.DARK} title={'Save Changes'} />
                            </div>
                            {error && (
                                <p className="error">{error}</p>
                            )}
                        </form>

                        <div>
                        <h2>Time Tracker</h2>
                            <form onSubmit={handleSubmit}>
                                <label htmlFor="date">Date:</label>
                                <input type="date" id="date" value={date} onChange={(e) => setDate(e.target.value)} />

                                <label htmlFor="start-time">Start Time:</label>
                                <input type="time" id="start-time" value={startTime} onChange={(e) => setStartTime(e.target.value)} />

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
                            </div>
     
                        <Button theme={ButtonTheme.DARK} title={"Publish"} onClick={ handlePublishSubmit } />
                    </div>                    
                    {error && loaded &&
                        <p className="error">{error}</p>
                    }
                </div>

            </div>
            }
            <Footer />
        </div>
    );
}


export default EditListingPage;
