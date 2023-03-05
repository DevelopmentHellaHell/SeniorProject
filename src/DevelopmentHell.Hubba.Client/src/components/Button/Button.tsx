import Loading from '../Loading/Loading';
import './Button.css';

interface IButtonProps {
    style?: ButtonStyles;
    title: string;
    onClick?: () => void;
    loading?: boolean;
}

export enum ButtonStyles {
    LIGHT,
    DARK,
}

const Button: React.FC<IButtonProps> = (props) => {

    const getColor = (theme: string) => {
        return getComputedStyle(document.documentElement).getPropertyValue(theme);
    }

    const styles = {
        [ButtonStyles.LIGHT]: {
            background: getColor("--primary-button-light"),
            color: getColor("--primary-text-dark")
        },
        [ButtonStyles.DARK]: {
            background: getColor("--primary-button-dark"),
            color: getColor("--primary-text-light")
        }
    }
    

    return (
        <button 
            className="btn" 
            onClick={props.onClick}
            style={
                styles[props.style ?? ButtonStyles.LIGHT]
            }>
            {!props.loading ?
                props.title :
                <Loading />
            }
        </button>
    );
}

export default Button;