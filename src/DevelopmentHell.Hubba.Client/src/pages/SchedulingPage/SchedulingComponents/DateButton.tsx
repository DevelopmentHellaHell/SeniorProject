
import Loading from '../../../components/Loading/Loading';
import './DateButton.css';

interface IDateButtonProps {
    id : string
    theme?: DateButtonTheme;
    title: string;
    onClick?: () => void;
    loading?: boolean;
}

export enum DateButtonTheme {
    LIGHT = "light",
    DARK = "dark",
    HOLLOW_LIGHT = "hollow_light",
    GREY = "hollow_dark",
}

const DateButton: React.FC<IDateButtonProps> = (props) => {

    const getColor = (theme: string) => {
        return getComputedStyle(document.documentElement).getPropertyValue(theme);
    }

    const themes: {
        [style in DateButtonTheme]: any
     } = {
        [DateButtonTheme.LIGHT]: {
            background: `rgb(${getColor("--primary-button-light")})`,
            color: `rgb(${getColor("--primary-text-dark")})`,
            border: `2px solid rgb(${getColor("--primary-button-light")})`,
        },
        [DateButtonTheme.DARK]: {
            background: `rgb(${getColor("--primary-button-dark")})`,
            color: `rgb(${getColor("--primary-text-light")})`,
            border: `2px solid rgb(${getColor("--primary-button-dark")})`,
        },
        [DateButtonTheme.HOLLOW_LIGHT]: {
            background: "rgb(0, 0, 0, 0)",
            color: `rgb(${getColor("--primary-text-light")})`,
            border: `2px solid rgb(${getColor("--primary-text-light")})`
        },
        [DateButtonTheme.GREY]: {
            background: "rgb(0, 0, 0, 0.09)",
            color: `rgb(0, 0, 0, 0.09)`,
            border: `2px solid rgb(${getColor("--primary-text-light")})`
        }
    }

    return (
        <button 
            className="date-btn" 
            id={props.id}
            onClick={ props.onClick }
            style={
                themes[props.theme ?? DateButtonTheme.GREY]
            }>
            {!props.loading ?
                props.title :
                <Loading />
            }
        </button>
    );
}

export default DateButton;