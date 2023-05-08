import { useNavigate } from 'react-router-dom';
import Button, { ButtonTheme } from '../../../../components/Button/Button';
import { IAvailability } from '../../ViewListingPage.tsx/ViewListingPage';
import { Ajax } from '../../../../Ajax';
import { useState } from 'react';



interface IEditListingAvailabilityCardProps {
    ownerId: number,
    availability: IAvailability;
}



const EditListingAvailability: React.FC<IEditListingAvailabilityCardProps> = (props) => {
    const navigate = useNavigate();
    const [error, setError] = useState<string | undefined>(undefined);

        
      

    const handleDeleteClick = async () => {
        const availability = [{ ListingId: props.availability.listingId, AvailabilityId: props.availability.availabilityId, OwnerId: props.ownerId, StartTime: props.availability.startTime, EndTime: props.availability.endTime, Action: 3 }];

        const availabilityList = [availability]
        const response = await Ajax.post<null>("/listingprofile/editListingAvailabilities", { reactAvailabilities: availability });
        if (response.error) {
          setError(response.error);
          return;
        }
        return navigate("/viewlisting", { state: {listingId: props.availability.listingId}});
    };
    return <> 
        <tr>
            <td><Button theme={ButtonTheme.DARK} onClick={() => {handleDeleteClick() } } title={"Delete"} /></td>
            <td>{props.availability.startTime ? new Date(props.availability.startTime ).toLocaleString(): '-'}</td>
            <td>{props.availability.endTime ? new Date(props.availability.endTime ).toLocaleString() : '-'}</td>
        </tr>
    </>
};
export default EditListingAvailability;
