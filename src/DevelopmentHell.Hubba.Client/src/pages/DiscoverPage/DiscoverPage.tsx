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
    ListingId: any;
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

            // if (response.data) {
            //     fill(response.data.listings, {
            //         AvgRatings: 0,
            //         ListingId: -1,
            //         Location: "Empty",
            //         Price: 0,
            //         Score: 0,
            //         Title: "Empty",
            //         TotalRatings: 0,
            //     }, 5);
            //     console.log(response.data);
            //     fill(response.data.collaborators, {
            //         CollaboratorId: -1,
            //         Name: "Empty",
            //         TotalVotes: 0,
            //     }, 5);
            //     fill(response.data.showcases, {
            //         Id: -1,
            //         Title: "Empty",
            //         Rating: 0,
            //         Description: "Empty",
            //     }, 5);
            // }
            
            setData(response.data);
            setError(response.error);
            setLoaded(response.loaded);
        }

        getData();
    }, []);

    return (
        <div id="discover-container" className="discover-container">
            {authData && authData.role !== Auth.Roles.DEFAULT_USER  ? 
                <NavbarUser /> : <NavbarGuest /> 
            }

            <div id="discover-content" className="discover-content">
                <div className="sidebar">
                    <div className="sidebar-box">
                        <div className="search-group search">
                            <h3>Search</h3>
                            <input id="search-input" placeholder="Search" onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                                setSearchQuery((previous) => { return {...previous, query: event.target.value} });
                            }}/>
                            <div id="search-button" className="search-button">
                                <Button
                                    title="🔎"
                                    onClick={ () => {
                                        if (!searchQuery.query || !searchQuery.query.length || searchQuery.query.length == 0) {
                                            setError("Empty search query. Please try again.");
                                            return;
                                        }

                                        if (searchQuery.query.length >= 250) {
                                            setError("Search query too long, please try again.");
                                            return;
                                        }

                                        setLastSearchQuery(searchQuery);
                                        setView(DiscoverViews.SEARCH);
                                    }}
                                />
                            </div>
                        </div>
                       
                        <div className="search-group">
                            <h3>Category</h3>
                            <div id="category">
                                <Dropdown title={searchQuery.category} id={"category-dropdown"}>
                                    <p id="category-listings" onClick={() => { setSearchQuery((previous) => { return {...previous, category: Category.LISTINGS} }) }}>Listings</p>
                                    <p id="category-project-showcases" onClick={() => { setSearchQuery((previous) => { return {...previous, category: Category.PROJECT_SHOWCASES} }) }}>Project Showcases</p>
                                    <p id="category-collaborators" onClick={() => { setSearchQuery((previous) => { return {...previous, category: Category.COLLABORATORS} }) }}>Collaborators</p>
                                </Dropdown>
                            </div>
                        </div>
                        <div className="search-group">
                            <h3>Filter</h3>
                            <div id="filter">
                                <Dropdown title={searchQuery.filter} id={"filter-dropdown"}>
                                    <p onClick={() => { setSearchQuery((previous) => { return {...previous, filter: "none"} }) }}>None</p>
                                    <p onClick={() => { setSearchQuery((previous) => { return {...previous, filter: "popular"} }) }}>Popularity</p>
                                </Dropdown>
                            </div>
                        </div>                        
                    </div>
                </div>

                <div className="discover-wrapper">
                    {error &&
                        <p className="error">{error}</p>
                    }
                    {!error && 
                        renderView(view)
                    }
                </div>
            </div>

            <Footer />
        </div> 
    );
}

export default DiscoverPage;