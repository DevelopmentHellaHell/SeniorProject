import { useEffect, useState } from "react";
import { Ajax } from "../../../Ajax";
import { Category, IDiscoveryListing, SearchQuery } from "../DiscoverPage";
import ListingCard from "../ListingCard/ListingCard";
import "./SearchView.css";

interface ISearchViewProps {
    searchQuery: SearchQuery;
}

const CuratedView: React.FC<ISearchViewProps> = (props) => {
    const [data, setData] = useState<any[] | null>(null);
    const [error, setError] = useState("");
    const [loaded, setLoaded] = useState(false);

    useEffect(() => {
        const getData = async () => {
            console.log(props.searchQuery.query, props.searchQuery.category, props.searchQuery.filter);

            const response = await Ajax.post<any>("/discovery/getSearch", { "Query": props.searchQuery.query, "Category": props.searchQuery.category, "Filter": props.searchQuery.filter, "Offset": 0 });
            setData(response.data);
            setError(response.error);
            setLoaded(response.loaded);
            
            console.log(response.data);
        }
        getData();
    }, [props.searchQuery]);

    return (
        <div className="search-view-wrapper">
            <p>Results: {data ? data.length : 0}</p>
            <div className="catalogue">
                {data && props.searchQuery.category == Category.LISTINGS && 
                    (data as IDiscoveryListing[]).map(item => {
                        return (
                            <ListingCard data={item} />
                        );
                    })
                }
            </div>
        </div>
    );
}

export default CuratedView;