import { useEffect, useState } from "react";
import { Ajax } from "../../../Ajax";
import { Category, IDiscoveryCollaborator, IDiscoveryListing, IDiscoveryProjectShowcase, SearchQuery } from "../DiscoverPage";
import ListingCard from "../ListingCard/ListingCard";
import "./SearchView.css";
import ProjectShowcaseCard from "../ProjectShowcaseCard/ProjectShowcaseCard";
import CollaboratorCard from "../CollaboratorCard/CollaboratorCard";
import Button from "../../../components/Button/Button";

interface ISearchViewProps {
    searchQuery: SearchQuery;
}

const CuratedView: React.FC<ISearchViewProps> = (props) => {
    const [data, setData] = useState<any[] | null>(null);
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(false);
    const [offset, setOffset] = useState(0);

    useEffect(() => {
        const getData = async () => {
            console.log(props.searchQuery.query, props.searchQuery.category, props.searchQuery.filter);

            const response = await Ajax.post<any>("/discovery/getSearch", {
                Query: props.searchQuery.query,
                Category: props.searchQuery.category,
                Filter: props.searchQuery.filter,
                Offset: offset * 50
            });
            setData(response.data);
            setError(response.error);
            setLoaded(response.loaded);
        }
        getData();
    }, [props.searchQuery]);

    return (
        <div id="search-view-wrapper" className="search-view-wrapper">
            <p>Results: {data ? data.length : 0}</p>
            {data &&
                <div>
                    {offset > 0 &&
                        <Button title="Prev" onClick={() => {
                            setOffset(offset-1);
                        }}/>
                    }
                    {data.length >= 50 &&
                        <Button title="Next" onClick={() => {
                            setOffset(offset+1);
                        }}/>
                    }
                </div>
            }
            <div className="catalogue">
                {data && props.searchQuery.category == Category.LISTINGS && 
                    data.map((item: IDiscoveryListing) => {
                        return (
                            <ListingCard data={item} />
                        );
                    })
                }
                {data && props.searchQuery.category == Category.PROJECT_SHOWCASES && 
                    data.map((item: IDiscoveryProjectShowcase) => {
                        return (
                            <ProjectShowcaseCard data={item} />
                        );
                    })
                }
                {data && props.searchQuery.category == Category.COLLABORATORS && 
                    data.map((item: IDiscoveryCollaborator) => {
                        return (
                            <CollaboratorCard data={item} />
                        );
                    })
                }
            </div>
        </div>
    );
}

export default CuratedView;