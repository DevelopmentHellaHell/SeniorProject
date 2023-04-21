import React, { useEffect, useState } from "react";
import { Auth } from "../../Auth";
import Footer from "../../components/Footer/Footer";
import NavbarUser from "../../components/NavbarUser/NavbarUser";
import "./DiscoverPage.css";
import NavbarGuest from "../../components/NavbarGuest/NavbarGuest";
import { Ajax } from "../../Ajax";
import ListingCard from "./ListingCard/ListingCard";
import Dropdown from "../../components/Dropdown/Dropdown";
import Button from "../../components/Button/Button";

interface IDiscoverPageProps {

}

interface ICuratedData {
    listings: IDiscoveryListing[],
    collaborators: any;
    showcases: any;
}

export interface IDiscoveryListing {
    AvgRatings: number;
    ListingId: number;
    Location: string;
    Price: number;
    Score: number;
    Title: string;
    TotalRatings: number;
}

enum Category {
    LISTINGS = "listings",
    PROJECT_SHOWCASES = "showcases",
    COLLABORATORS = "collaborators",
}

const DiscoverPage: React.FC<IDiscoverPageProps> = (props) => {
    const [data, setData] = useState<ICuratedData | null>(null);
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(false);
    const [query, setQuery] = useState<string | null>(null);
    const [category, setCategory] = useState(Category.LISTINGS);
    const [filter, setFilter] = useState("none");

    const authData = Auth.getAccessData();

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
            }
            
            setData(response.data);
            setError(response.error);
            setLoaded(response.loaded);
        }

        getData();
    }, []);

    const search = async () => {
        console.log(query, category, filter);
        // todo switch views and transfer search data
        const response = await Ajax.post<any>("/discovery/getSearch", { "Query": query, "Category": category, "Filter": filter, "Offset": 0 });
        console.log(response.data);

    }

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
                                setQuery(event.target.value);
                            }}/>
                            <div className="search-button">
                                <Button
                                    title="🔎"
                                    onClick={ () => {
                                        search();
                                    }}
                                />
                            </div>
                        </div>
                       
                        <div className="search-group">
                            <h3>Category</h3>
                            <Dropdown title={category}>
                                <p onClick={() => { setCategory(Category.LISTINGS) }}>Listings</p>
                                <p onClick={() => { setCategory(Category.PROJECT_SHOWCASES) }}>Project Showcases</p>
                                <p onClick={() => { setCategory(Category.COLLABORATORS) }}>Collaborators</p>
                            </Dropdown>
                        </div>
                        <div className="search-group">
                            <h3>Filter</h3>
                            <Dropdown title={filter}>
                                <p onClick={() => { setFilter("none") }}>None</p>
                                <p onClick={() => { setFilter("popularity") }}>Popularity</p>
                            </Dropdown>
                        </div>                        
                    </div>
                </div>

                <div className="discover-wrapper">
                    <div>
                        <h3 className="category">Listings</h3>
                        <div className="catalogue">
                            {data && 
                                data.listings.map(item => {
                                    return (
                                        <ListingCard data={item} />
                                    );
                                })
                            }
                        </div>

                        <h3 className="category">Project Showcases</h3>
                        <div className="catalogue">
                            {data && 
                                data.listings.map(item => {
                                    return (
                                        <ListingCard data={item} />
                                    );
                                })
                            }
                        </div>

                        <h3 className="category">Collaborators</h3>
                        <div className="catalogue">
                            {data && 
                                data.listings.map(item => {
                                    return (
                                        <ListingCard data={item} />
                                    );
                                })
                            }
                        </div>
                    </div>
                </div>
            </div>

            <Footer />
        </div> 
    );
}

export default DiscoverPage;