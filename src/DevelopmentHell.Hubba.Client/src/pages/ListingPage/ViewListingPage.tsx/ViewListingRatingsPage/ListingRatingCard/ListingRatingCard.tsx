import { IRating } from "../../ViewListingPage";

interface IListingRatingCardProps {
    rating: IRating;
}

const ListingRatingCard: React.FC<IListingRatingCardProps> = (props) => {
    console.log(props.rating);
    return (
        <div>
            
            <tr>
                <td>{props.rating.rating}</td>
                <td>{props.rating.comment}</td>
                <td>{props.rating.anonymous ? "Anonymous" : props.rating.username}</td>
                <td>{props.rating.lastEdited ? new Date(props.rating.lastEdited).toLocaleString() : '-'}</td>
                </tr>

        </div>
    );
};
export default ListingRatingCard;
