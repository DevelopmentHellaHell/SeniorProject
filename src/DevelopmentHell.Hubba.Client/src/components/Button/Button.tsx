import Loading from '../Loading/Loading';
import './Button.css';

interface IButtonProps {
    title: string;
    onClick?: () => void;
    loading?: boolean;
}

const Button: React.FC<IButtonProps> = (props) => {
    return (
        <button className="btn" onClick={props.onClick}>{
            !props.loading ?
            props.title :
            <Loading />
        }</button>
    );
}

export default Button;