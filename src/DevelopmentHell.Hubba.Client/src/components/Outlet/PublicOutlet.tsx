import { Navigate, Outlet } from "react-router-dom";
import { Auth } from "../../Auth";

interface Props {
    children?: React.ReactNode;
    redirectPath: string;
}

const PublicOutlet: React.FC<Props> = (props: React.PropsWithChildren<Props>) => {
	const authData = Auth.isAuthenticated();

    return !authData || authData.role == Auth.Roles.DEFAULT_USER ? (
        <>
           	{props.children}
          	<Outlet />
         </>
    ) : (
        <Navigate to={props.redirectPath} replace />
    );
};

export default PublicOutlet;