import { Navigate, Outlet } from "react-router-dom";
import { Auth } from "../../Auth";

interface Props {
    children?: React.ReactNode;
    redirectPath: string;
}

const PublicOutlet: React.FC<Props> = (props: React.PropsWithChildren<Props>) => {
    return !Auth.isAuthenticated() ? (
        <>
           	{props.children}
          	<Outlet />
         </>
    ) : (
        <Navigate to={props.redirectPath} replace />
    );
};

export default PublicOutlet;