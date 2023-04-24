import Button, { ButtonTheme } from "../../../../components/Button/Button";
import { IListing } from "../MyListingsView";
import { redirect, useNavigate } from "react-router-dom";
import './ListingCard.css';

interface IListingCardProps {
    listing: IListing;
}

const ListingCard: React.FC<IListingCardProps> = (props) => {
    const navigate = useNavigate();
    
    return <>
    <tr className="listing-card-row">
        <td className="view-btn-cell">
            <div className="view-btn-wrapper">

            <Button theme={ButtonTheme.DARK} onClick={() => { navigate("/viewlisting", { state: { listingId: props.listing.listingId }})} } title={"View"} />
            </div>
        </td>
        <td className="title-cell"> { props.listing.title} </td>
        <td className="rating-cell"> { props.listing.averageRating }</td>
        <td className="status-cell"> { props.listing.published ? 'Published' : 'Draft' }</td>
    </tr>
    </>
}
export default ListingCard;