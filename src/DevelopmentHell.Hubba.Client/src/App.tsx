import React from "react";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import Home from "./pages/Home/Home";
import Account from "./pages/Account/Account";
import Analytics from "./pages/Analytics/Analytics";
import Login from "./pages/Login/Login";
import Logout from "./pages/Logout/Logout";
import Notification from "./pages/Notification/Notification";
import Otp from "./pages/Otp/Otp";
import Registration from "./pages/Registration/Registration";
import "./Theme.css";
import PrivateRoute from "./components/Outlet/PrivateOutlet";
import PublicOutlet from "./components/Outlet/PublicOutlet";
import Unauthorized from "./pages/Unauthorized/Unauthorized";
import { Auth } from "./Auth";

interface Props {

}

const App: React.FC<Props> = (props) => {
	return (
		<div className="App">
			<BrowserRouter>
				<Routes>
					<Route index element={<Home />} /> 
					
					<Route path="/registration" element={
						<PublicOutlet redirectPath="/">
							<Registration />
						</PublicOutlet>
					} />
					<Route path="/login" element={
						<PublicOutlet redirectPath="/">
							<Login />
						</PublicOutlet>
					} />
					<Route path="/" element={<PrivateRoute allowedRoles={[Auth.Roles.DEFAULT_USER]}/>}>
						<Route path="/otp" element={<Otp />} />
					</Route>

					<Route path="/" element={<PrivateRoute allowedRoles={[Auth.Roles.VERIFIED_USER, Auth.Roles.ADMIN_USER]} />}>
						<Route path="/account" element={<Account />} />
						<Route path="/logout" element={<Logout />} />
						<Route path="/notification" element={<Notification />} />
					</Route>
					<Route path="/" element={<PrivateRoute allowedRoles={[Auth.Roles.ADMIN_USER]}/>}>
						<Route path="/analytics" element={<Analytics />} />
					</Route>

					<Route path="/unauthorized" element={<Unauthorized />} />
				</Routes>
			</BrowserRouter>
		</div>
  	);
}

export default App;
function jwt_decode(arg0: string): any {
	throw new Error("Function not implemented.");
}

