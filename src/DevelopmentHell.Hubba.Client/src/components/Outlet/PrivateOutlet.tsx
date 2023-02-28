import { Navigate, Outlet, useLocation } from "react-router-dom";
import { Auth } from "../../Auth";

interface Props {
    children?: React.ReactNode;
	allowedRoles: string[];
}

const PrivateOutlet: React.FC<Props> = (props: React.PropsWithChildren<Props>) => {
	const location = useLocation();
	const data = Auth.isAuthenticated();
	
    return props.allowedRoles?.find((role) => data?.role.includes(role)) ? (
        <>
            {props.children}
            <Outlet />
        </>
    ) : data?.accountId ? (
		<Navigate to="/unauthorized" state={{ from: location }} replace />
	) : (
        <Navigate to="/login" state={{ from: location }} replace />
    );
};

export default PrivateOutlet;