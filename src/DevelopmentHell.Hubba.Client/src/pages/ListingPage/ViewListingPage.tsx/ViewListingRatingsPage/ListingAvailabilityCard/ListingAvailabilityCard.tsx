import { IAvailability } from '../../ViewListingPage'


interface IListingAvailabilityCardProps {
    availability: IAvailability;
}

const ListingRatingCard: React.FC<IListingAvailabilityCardProps> = (props) => {
    return (
        <div>
            
            <tr>
                <td>{props.availability.startTime ? new Date(props.availability.startTime ).toLocaleString(): '-'}</td>
                <td>{props.availability.endTime ? new Date(props.availability.endTime ).toLocaleString() : '-'}</td>
                </tr>

        </div>
    );
};
export default ListingRatingCard;
