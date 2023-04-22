import React, { useEffect, useState } from "react";
import { Auth } from "../../Auth";
import Footer from "../../components/Footer/Footer";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import "./DiscoverPage.css";
import NavbarGuest from "../../components/NavbarGuest/NavbarGuest";
import { Ajax } from "../../Ajax";
import Dropdown from "../../components/Dropdown/Dropdown";
import Button from "../../components/Button/Button";
import CuratedView from "./CuratedView/CuratedView";
import SearchView from "./SearchView/SearchView";

interface IDiscoverPageProps {

}

export interface ICuratedData {
    listings: IDiscoveryListing[],
    collaborators: IDiscoveryCollaborator[];
    showcases: IDiscoveryProjectShowcase[];
}

export interface IDiscoveryListing {
    AvgRatings: number;
    ListingId: number;
    Location: string;
    Price: number;
    Title: string;
    TotalRatings: number;
}

export interface IDiscoveryCollaborator {
    CollaboratorId: number;
    Name: string;
    TotalVotes: number;
}

export interface IDiscoveryProjectShowcase {
    Id: number;
    Title: string;
    Rating: number;
    Description: string;
}

export enum Category {
    LISTINGS = "listings",
    PROJECT_SHOWCASES = "showcases",
    COLLABORATORS = "collaborators",
}

enum DiscoverViews {
    CURATED,
    SEARCH,
}

export interface SearchQuery {
    query: string;
    category: string;
    filter: string;
}

const DiscoverPage: React.FC<IDiscoverPageProps> = (props) => {
    const [data, setData] = useState<ICuratedData | null>(null);
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(false);
    const [searchQuery, setSearchQuery] = useState<SearchQuery>({
        query: "",
        category: Category.LISTINGS,
        filter: "none"
    });
    const [lastSearchQuery, setLastSearchQuery] = useState<SearchQuery>(searchQuery);
    const [view, setView] = useState(DiscoverViews.CURATED);

    const authData = Auth.getAccessData();

    const renderView = (view: DiscoverViews) => {
        switch(view) {
            case DiscoverViews.CURATED:
                return <CuratedView data={data ? data : undefined} />;
            case DiscoverViews.SEARCH:
                return <SearchView searchQuery={lastSearchQuery}/>;
        }
    }

    useEffect(() => {
        const getData = async () => {
            const response = await Ajax.post<ICuratedData>("/discovery/getCurated", { "Offset": 0 });
            
            const fill = (arr: any[], obj: any, num: number) => {
                while (arr.length < num) {
                    arr.push(obj);
                }
            } 

            if (response.data) {
                fill(response.data!.listings, {
                    AvgRatings: 0,
                    ListingId: -1,
                    Location: "Empty",
                    Price: 0,
                    Score: 0,
                    Title: "Empty",
                    TotalRatings: 0,
                }, 5);
                fill(response.data!.collaborators, {
                    CollaboratorId: -1,
                    Name: "Empty",
                    TotalVotes: 0,
                }, 5);
                fill(response.data!.showcases, {
                    Id: -1,
                    Title: "Empty",
                    Rating: 0,
                    Description: "Empty",
                }, 5);
            }
            
            setData(response.data);
            setError(response.error);
            setLoaded(response.loaded);
        }

        getData();
    }, []);

    return (
        <div className="discover-container">
            {authData && authData.role !== Auth.Roles.DEFAULT_USER  ? 
                <NavbarUser /> : <NavbarGuest /> 
            }

            <div className="discover-content">
                <div className="sidebar">
                    <div className="sidebar-box">
                        <div className="search-group search">
                            <h3>Search</h3>
                            <input id="search" placeholder="Search" onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                                setSearchQuery((previous) => { return {...previous, query: event.target.value} });
                            }}/>
                            <div className="search-button">
                                <Button
                                    title="🔎"
                                    onClick={ () => {
                                        setLastSearchQuery(searchQuery);
                                        setView(DiscoverViews.SEARCH);
                                    }}
                                />
                            </div>
                        </div>
                       
                        <div className="search-group">
                            <h3>Category</h3>
                            <Dropdown title={searchQuery.category}>
                                <p onClick={() => { setSearchQuery((previous) => { return {...previous, category: Category.LISTINGS} }) }}>Listings</p>
                                <p onClick={() => { setSearchQuery((previous) => { return {...previous, category: Category.PROJECT_SHOWCASES} }) }}>Project Showcases</p>
                                <p onClick={() => { setSearchQuery((previous) => { return {...previous, category: Category.COLLABORATORS} }) }}>Collaborators</p>
                            </Dropdown>
                        </div>
                        <div className="search-group">
                            <h3>Filter</h3>
                            <Dropdown title={searchQuery.filter}>
                                <p onClick={() => { setSearchQuery((previous) => { return {...previous, filter: "none"} }) }}>None</p>
                                <p onClick={() => { setSearchQuery((previous) => { return {...previous, filter: "popularity"} }) }}>Popularity</p>
                            </Dropdown>
                        </div>                        
                    </div>
                </div>

                <div className="discover-wrapper">
                    {renderView(view)}
                </div>
            </div>

            <Footer />
        </div> 
    );
}

export default DiscoverPage;