
import Loading from '../../../components/Loading/Loading';
import './HourBarButton.css';

interface IHourBarButtonProps {
    theme?: HourBarButtonTheme;
    title: string;
    onClick?: () => void;
    loading?: boolean;
}

export enum HourBarButtonTheme {
    LIGHT = "light",
    DARK = "dark",
    HOLLOW_DARK = "hollow_dark",
    GREY = "grey",
}

const HourBarButton: React.FC<IHourBarButtonProps> = (props) => {

    const getColor = (theme: string) => {
        return getComputedStyle(document.documentElement).getPropertyValue(theme);
    }

    const themes: {
        [style in HourBarButtonTheme]: any
     } = {
        [HourBarButtonTheme.LIGHT]: {
            background: `rgb(${getColor("--primary-button-light")})`,
            color: `rgb(${getColor("--primary-text-dark")})`,
            border: `2px solid rgb(${getColor("--primary-button-light")})`,
        },
        [HourBarButtonTheme.DARK]: {
            background: `rgb(${getColor("--primary-button-dark")})`,
            color: `rgb(${getColor("--primary-text-light")})`,
            border: `4px solid rgb(${getColor("--primary-button-dark")})`,
        },
        [HourBarButtonTheme.HOLLOW_DARK]: {
            background: "rgb(0, 0, 0, 0)",
            color: `rgb(${getColor("--primary-text-dark")})`,
            border: `2px solid rgb(${getColor("--primary-text-dark")})`
        },
        [HourBarButtonTheme.GREY]: {
            background: "rgb(0, 0, 0, 0.09)",
            color: `rgb(0, 0, 0, 0.09)`,
            border: `2px solid rgb(${getColor("--primary-text-light")})`
        }
    }

    return (
        <button 
            className="hour-btn" 
            onClick={ props.onClick }
            style={
                themes[props.theme ?? HourBarButtonTheme.GREY]
            }>
            {!props.loading ?
                props.title :
                <Loading />
            }
        </button>
    );
}

export default HourBarButton;