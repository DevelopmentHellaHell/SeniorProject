import { Navigate, Outlet, useLocation } from "react-router-dom";
import { Auth } from "../../Auth";

interface IPrivateOuteletProps {
    children?: React.ReactNode;
	allowedRoles: string[];
	redirectPath: string;
}

const PrivateOutlet: React.FC<IPrivateOuteletProps> = (props: React.PropsWithChildren<IPrivateOuteletProps>) => {
	const location = useLocation();
	const data = Auth.isAuthenticated();

	if (props.allowedRoles.find((role) => data?.role == role)) {
		return (
			<>
				{props.children}
				<Outlet />
			</>
		);
	}

	if (data && data.role !== Auth.Roles.DEFAULT_USER) {
		return (
			<Navigate to="/unauthorized" state={{ from: location }} replace />
		);
	}
	
    return (
        <Navigate to={props.redirectPath} state={{ from: location }} replace />
    );
};

export default PrivateOutlet;