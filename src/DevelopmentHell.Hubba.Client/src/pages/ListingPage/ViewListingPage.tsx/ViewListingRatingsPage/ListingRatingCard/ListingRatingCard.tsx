import { IRating } from "../../ViewListingPage";

interface IListingRatingCardProps {
    rating: IRating;
}

const ListingRatingCard: React.FC<IListingRatingCardProps> = (props) => {
    return (
            <tr>
                <td style={{width: '5%'}}>{props.rating.rating}</td>
                <td style={{width: '70%'}}>{props.rating.comment}</td>
                <td style={{width: '15%'}}>{props.rating.anonymous ? "Anonymous" : props.rating.username}</td>
                <td style={{width: '10%'}}>{props.rating.lastEdited ? new Date(props.rating.lastEdited).toLocaleString() : '-'}</td>
                </tr>
    );
};
export default ListingRatingCard;
