import Button, { ButtonTheme } from "../../../../components/Button/Button";
import { IListing } from "../MyListingsView";
import { redirect, useNavigate } from "react-router-dom";

interface IListingCardProps {
    listing: IListing;
}

const ListingCard: React.FC<IListingCardProps> = (props) => {
    const navigate = useNavigate();
    
    return <>
    <tr>
        <td>
            <Button theme={ButtonTheme.DARK} onClick={() => { navigate("/viewlisting", { state: { listingId: props.listing.listingId }})} } title={"View"} />
        </td>
        <td> { props.listing.title} </td>
        <td> { props.listing.averageRating }</td>
        <td> { props.listing.published ? 'Published' : 'Draft' }</td>
    </tr>
    </>
}
export default ListingCard;