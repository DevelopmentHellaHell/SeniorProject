import React, {  useEffect, useState } from "react";
import { Link, redirect, useLocation, useNavigate } from "react-router-dom";
import { Auth } from "../../Auth";
import { Ajax } from "../../Ajax";
import Footer from "../../components/Footer/Footer";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import "./ListingProjectShowcasesPage.css";
import LikeButton from "../../components/Heart/Heart";
import Button, { ButtonTheme } from "../../components/Button/Button";
import NavbarGuest from "../../components/NavbarGuest/NavbarGuest";
import { use } from "chai";


interface IListingProjectShowcasePageProps {

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
    linkedListingId: string,
    linkedListingTitle: string,
    title: string,
    description: string,
    isPublished: boolean,
    rating: number,
    publishTimestamp: Date,
    editTimestamp: Date,
    confirmShowing: boolean,
    confirmAction: (Id: string)=>void
}

const createShowcaseTableRow = (showcaseData: IShowcaseData) => {
    showcaseData.confirmShowing == null ? false : showcaseData.confirmShowing;
    return (
        <tr key={`showcase-${showcaseData.id}`}>
            <td className="table-rating"> {showcaseData.rating}</td>
            <td className="table-title">
                <Link to={`/showcases/p/view?s=${showcaseData.id}`}>{showcaseData.title}</Link>
            </td>
        </tr>
    );
}

const ListingProjectShowcasesPage: React.FC<IListingProjectShowcasePageProps> = (props) => {
    const [error, setError] = useState("");
    const [title, setTitle] = useState<string | null>(null);
    const [description, setDescription] = useState<String | null>(null);
    const [files, setFiles] = useState<File[]>([]);
    const [fetchResult, setFetchResult] = useState();
    const [uploadResponse, setUploadResponse] = useState<IShowcaseDTO>();
    const [data, setData] = useState<null | IShowcaseData[]>([]);
    const [fileData, setFileData] = useState<{ Item1: string, Item2: string }[]>([]);
    const [listingId, setListingId] = useState<number>(0);
    const { search } = useLocation();
    const searchParams = new URLSearchParams(search);

    const authData = Auth.getAccessData();
    const navigate = useNavigate();

    const getData = async () => {
        const response = await Ajax.get<IShowcaseData[]>(`/showcases/listing?l=${searchParams.get("l")}`);
        if (response.status === 200) {
            setData(response.data);
        }
        else {
            setError("Failed to load project showcases.");
        }
    }

    useEffect(() => {
        getData();
    }, []);

    return (
        <div className="listing-project-showcases-container">
            {!authData && <NavbarGuest />}
            <NavbarUser />
            <div className="listing-project-showcases-content">
                <div className="v-stack">
                    <div className="listing-project-showcases-header">
                        <h1>Project Showcases</h1>
                        <Button theme={ButtonTheme.DARK} onClick={() => {
                            navigate(`/showcases/p/new?l=${searchParams.get("l")}`);
                        }} title="Create new Showcase" />
                        <p>Here you can view all the project showcases for this listing.</p>
                        <Button theme={ButtonTheme.DARK} onClick={() => {
                            navigate(`/showcases/p/link?l=${searchParams.get("l")}`);
                        }} title="Link My Showcase(s)" />
                    </div>
                    <div className="listing-project-showcases-body">
                        <div className="listing-project-showcases-wrapper">
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
                                            <td colSpan={2}>No project showcases found.</td>
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

export default ListingProjectShowcasesPage;