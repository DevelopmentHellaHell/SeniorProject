import React, {  useEffect, useState } from "react";
import { Link, redirect, useLocation, useNavigate } from "react-router-dom";
import { Auth } from "../../Auth";
import { Ajax } from "../../Ajax";
import Footer from "../../components/Footer/Footer";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import "./LinkListingShowcasePage.css";
import LikeButton from "../../components/Heart/Heart";
import Button, { ButtonTheme } from "../../components/Button/Button";
import NavbarGuest from "../../components/NavbarGuest/NavbarGuest";
import { use } from "chai";


interface ILinkListingShowcasePagePageProps {

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

export interface IShowcaseData {
    id: string,
    showcaseUserId: number,
    listingId: number,
    listingTitle: string,
    title: string,
    description: string,
    isPublished: boolean,
    rating: number,
    publishTimestamp: Date,
    editTimestamp: Date,
    confirmShowing: boolean,
    processing: boolean,
    confirmAction: (Id: string)=>void
}



const LinkListingShowcasePagePage: React.FC<ILinkListingShowcasePagePageProps> = (props) => {
    const [error, setError] = useState("");
    const [title, setTitle] = useState<string | null>(null);
    const [description, setDescription] = useState<String | null>(null);
    const [files, setFiles] = useState<File[]>([]);
    const [fetchResult, setFetchResult] = useState();
    const [uploadResponse, setUploadResponse] = useState<IShowcaseDTO>();
    const [data, setData] = useState<IShowcaseData[]>([]);
    const [fileData, setFileData] = useState<{ Item1: string, Item2: string }[]>([]);
    const [listingId, setListingId] = useState<number>(0);
    const { search } = useLocation();
    const searchParams = new URLSearchParams(search);

    const authData = Auth.getAccessData();
    const navigate = useNavigate();
    
    useEffect(() => {
        setListingId(searchParams.get("l") ? parseInt(searchParams.get("l")!) : 0)
        getData();
    }, []);
   
    const createShowcaseTableRow = (showcaseData: IShowcaseData) => {
        showcaseData.confirmShowing == null ? false : showcaseData.confirmShowing;
        return (
            <tr key={`showcase-${showcaseData.id}`}>
                <td className="table-rating"> {showcaseData.rating}</td>
                <td className="table-title">
                    <Link to={`/showcases/p/view?s=${showcaseData.id}`}>{showcaseData.title}</Link>
                </td>
                <td className="table-linked">
                    {showcaseData.processing
                        ? <p>Processing...</p>
                    : showcaseData.listingId == null
                        ? <Button theme={ButtonTheme.DARK} onClick={() => {
                            linkShowcase(showcaseData.id);
                        }} title= "Link Showcase"/>
                        : <p>Already Linked</p>
                    }
                </td>
            </tr>
        );
    }

    const linkShowcase = (showcaseId: string) => {
        setData((prevData) =>
            prevData.map((showcaseData) =>
                showcaseData.id === showcaseId
                ? { ...showcaseData, confirmShowing: true, processing: true }
                : showcaseData
            )
        );
        Ajax.post(`/showcases/link?s=${showcaseId}&l=${listingId}`, null).then(
            (response) => {
                if (response.status === 200 || response.status === 204) {
                    setData((prevData) => 
                        prevData.map((showcaseData) =>
                            showcaseData.id === showcaseId
                            ? { ...showcaseData, listingId: listingId, processing: false }
                            : showcaseData
                        )
                    );
                }
                else {
                    setError("Failed to link showcase.");
                    setData((prevData) =>
                        prevData.map((showcaseData) =>
                            showcaseData.id === showcaseId
                            ? { ...showcaseData, processing: false }
                            : showcaseData
                        )
                    );
                }
            }
        )
    }
    
    
    if (!authData) {
        redirect("/login");
        return null;
    }


    const getData = async () => {
        const response = await Ajax.get<IShowcaseData[]>(`/showcases/user?u=${authData.sub}`);
        if (response.status === 200) {
            if (response.data) {
                setData(response.data);
            }
        }
        else {
            setError("Failed to load project showcases.");
        }
    }

    return (
        <div className="link-listing-showcase-container">
            <NavbarUser />
            <div className="link-listing-showcase-content">
                <div className="v-stack">
                <div className="link-listing-showcase-header">
                        <h1>Link Project Showcase(s)</h1>
                    <p>Here you can link your own project showcases to this listing.</p>
                </div>
                <div className="link-listing-showcase-body">
                    <div className="link-listing-showcase-wrapper">
                        <table className="my-showcases-table">
                            <thead className="my-showcases-table-header">
                                <tr>
                                    <th className="header-likes">
                                        <LikeButton size={"20"} defaultOn={true} enabled={false}
                                            OnUnlike={function (...args: any[]) {
                                                throw new Error("Function not implemented.");
                                            }}
                                            OnLike={function (...args: any[]) {
                                                throw new Error("Function not implemented.");
                                            }} />
                                    </th>
                                    <th className="header-listing">Linked Listing</th>
                                </tr>
                            </thead>
                            <tbody>
                                {data && data.length == 0 &&
                                    <tr>
                                        <td colSpan={3}>No project showcases found.</td>
                                    </tr>
                                }
                                {data && data.map(value => {
                                    return createShowcaseTableRow(value);
                                })}
                            </tbody>
                        </table>
                    </div>
                </div>
                </div>
            </div>


            <Footer />
        </div>
    );
}

export default LinkListingShowcasePagePage;