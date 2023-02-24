import Footer from "../../components/Footer/Footer"
import NavbarGuest from "../../components/NavbarGuest/NavbarGuest";

interface Props {

}

const Login: React.FC<Props> = (props) => {
    return (
        <div className="registration-container">
            <NavbarGuest />

            <div className="registration-wrapper">
                LOGIN TODO
            </div>
            
            <Footer />
        </div>
    );
}

export default Login;