import { useState, useEffect } from "react"
import { useLocation, useNavigate } from "react-router-dom"
import { Ajax } from "../../../Ajax"
import Button, { ButtonTheme } from "../../../components/Button/Button"
import Footer from "../../../components/Footer/Footer"
import NavbarUser from "../../../components/NavbarUser/NavbarUser"
import { IListing } from "../../ListingProfilePage/MyListingsView/MyListingsView"
import { Auth } from "../../../Auth"

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
    const [fileData, setFileData] = useState<{ [key: string]: ArrayBuffer }>({});


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
      
        const fileDataList: { name: string, data: string }[] = [];
      
        // loop through each selected file and read as data URL
        files.forEach((file) => {
          const reader = new FileReader();
      
          reader.readAsDataURL(file);
          reader.onload = () => {
            // convert data URL to base64 encoded string
            const dataUrl = reader.result as string;
            const base64String = dataUrl.split(',')[1];
      
            // create a new object with file name and base64-encoded data
            const fileData = {
              name: file.name,
              data: base64String,
            };
      
            // add the new object to the fileDataList
            fileDataList.push(fileData);
      
            // set state with updated file data list
            if (fileDataList.length === files.length) {
                // create the dictionary from the fileDataList using reduce
                const fileDataDict = fileDataList.reduce((acc, curr) => {
                    const binaryString = atob(curr.data);
                    const bytes = new Uint8Array(binaryString.length);
                    for (let i = 0; i < binaryString.length; i++) {
                    bytes[i] = binaryString.charCodeAt(i);
                    }
                    acc[curr.name] = bytes.buffer;
                    return acc;
                }, {} as { [key: string]: ArrayBuffer });
      
                // set state with the file data dictionary
                setFileData((previous)=>{ return {...previous, ...fileDataDict} });
                
            }
          };
        });
        console.log({ ListingId: data?.Listing.listingId, DeleteNames: null,  AddFiles: fileData});
        const response = await Ajax.post<null>("/listingprofile/editListingFiles", { ListingId: data?.Listing.listingId, DeleteNames: null,  AddFiles: fileData});
        if (response.error) {
            setError(response.error);
        } else {
            navigate("/viewlisting", { state: { listingId: data?.Listing.listingId } });
        }

      };

      const handleFileNameChange = (index: number, name: string) => {
        const newFiles = [...files];
        newFiles[index] = new File([newFiles[index]], name, { type: newFiles[index].type });
        setFiles(newFiles);
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
                        </p>
                        <h2 className="listing-page__title">{data.Listing.title}</h2>
                        {data.Files && data.Files.length > 0 && (
                          <div className="listing-page__image-wrapper">
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
                            
                          </div>
                          
                        )}
                       <form onSubmit={handleFileSubmit}>
                            <input type="file" accept=".jpg,.jpeg,.png" multiple onChange={handleFileSelect} />
                            {files.length > 0 && (
                                <ul>
                                {files.map((file, index) => (
                                    <li key={file.name}>
                                    <input
                                        type="text"
                                        value={file.name}
                                        onChange={(e) => handleFileNameChange(index, e.target.value)}
                                    />
                                    </li>
                                ))}
                                </ul>
                            )}
                            <button type="submit">Convert to Base64</button>
                            {/* {fileData.length > 0 && (
                                <ul>
                                {fileData.map((data) => (
                                    <li key={data}>{data}</li>
                                ))}
                                </ul>
                            )} */}
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
                                <input type="text" name="description" value={data.Listing.description} onChange={handleInputChange} />
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
