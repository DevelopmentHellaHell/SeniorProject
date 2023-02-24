import './Button.css';

interface Props {
    title: string;
    onClick?: () => void;
}

const Button: React.FC<Props> = (props) => {
    return (
        <button className="btn" onClick={props.onClick}>{props.title}</button>
    );
}

export default Button;