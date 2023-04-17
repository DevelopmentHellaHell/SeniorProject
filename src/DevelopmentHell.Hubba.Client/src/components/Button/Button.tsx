import Loading from '../Loading/Loading';
import './Button.css';

interface IButtonProps {
    theme?: ButtonTheme;
    title: string;
    onClick?: () => void;
    loading?: boolean;
}

export enum ButtonTheme {
    LIGHT = "light",
    DARK = "dark",
    HOLLOW_LIGHT = "hollow_light",
    HOLLOW_DARK = "hollow_dark",
}

const Button: React.FC<IButtonProps> = (props) => {

    const getColor = (theme: string) => {
        return getComputedStyle(document.documentElement).getPropertyValue(theme);
    }

    const themes: {
        [style in ButtonTheme]: any
     } = {
        [ButtonTheme.LIGHT]: {
            background: `rgb(${getColor("--primary-button-light")})`,
            color: `rgb(${getColor("--primary-text-dark")})`,
            border: `4px solid rgb(${getColor("--primary-button-light")})`,
        },
        [ButtonTheme.DARK]: {
            background: `rgb(${getColor("--primary-button-dark")})`,
            color: `rgb(${getColor("--primary-text-light")})`,
            border: `4px solid rgb(${getColor("--primary-button-dark")})`,
        },
        [ButtonTheme.HOLLOW_LIGHT]: {
            background: "rgb(0, 0, 0, 0)",
            color: `rgb(${getColor("--primary-text-light")})`,
            border: `4px solid rgb(${getColor("--primary-text-light")})`
        },
        [ButtonTheme.HOLLOW_DARK]: {
            background: "rgb(0, 0, 0, 0)",
            color: `rgb(${getColor("--primary-text-dark")})`,
            border: `4px solid rgb(${getColor("--primary-text-dark")})`
        }
    }

    return (
        <button 
            className="btn" 
            onClick={ props.onClick }
            style={
                themes[props.theme ?? ButtonTheme.LIGHT]
            }>
            {!props.loading ?
                props.title :
                <Loading />
            }
        </button>
    );
}

export default Button;