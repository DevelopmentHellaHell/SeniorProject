import { IRating } from "../../ViewListingPage";

interface IListingRatingCardProps {
    rating: IRating;
}

const ListingRatingCard: React.FC<IListingRatingCardProps> = (props) => {
    return (
        <div>
            
            <tr>
                <td>{props.rating.rating}</td>
                <td>{props.rating.username ? props.rating.username : "Anonymous"}</td>
                <td>{props.rating.comment}</td>
                <td>{props.rating.lastEdited ? new Date(props.rating.lastEdited).toLocaleString() : '-'}</td>
                </tr>

        </div>
    );
};
export default ListingRatingCard;
