import React from "react";
import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import "./Theme.css";
import PrivateRoute from "./components/Outlet/PrivateOutlet";
import PublicOutlet from "./components/Outlet/PublicOutlet";
import Unauthorized from "./pages/UnauthorizedPage/UnauthorizedPage";
import { Auth } from "./Auth";
import HomePage from "./pages/HomePage/HomePage";
import AccountPage from "./pages/AccountPage/AccountPage";
import LoginPage from "./pages/LoginPage/LoginPage";
import LogoutPage from "./pages/LogoutPage/LogoutPage";
import NotificationPage from "./pages/NotificationPage/NotificationPage";
import RegistrationPage from "./pages/RegistrationPage/RegistrationPage";
import AdminDashboardPage from "./pages/AdminDashboardPage/AdminDashboardPage";
import AccountRecoveryPage from "./pages/AccountRecoveryPage/AccountRecoveryPage";
import "./App.css";
import NotificationStateProvider from "./NotificationStateProvider";

interface IAppProps {

}

const App: React.FC<IAppProps> = (props) => {
	return (
		<NotificationStateProvider>
			<div className="App">
				<BrowserRouter>
					<Routes>
						{/* Anyone can access */}
						<Route index element={<HomePage />} />
						<Route path="*" element={<Navigate to='/' replace />} />
						<Route path="/unauthorized" element={<Unauthorized />} />
						
						{/* Public routes - no auth */}
						<Route path="/registration" element={
							<PublicOutlet redirectPath="/">
								<RegistrationPage />
							</PublicOutlet>
						} />
						<Route path="/login" element={
							<PublicOutlet redirectPath="/">
								<LoginPage />
							</PublicOutlet>
						} />
						<Route path="/account-recovery" element={
							<PublicOutlet redirectPath="/">
								<AccountRecoveryPage />
							</PublicOutlet>
						} />
						
						
						{/* Protect/private routes - with auth */}
						<Route path="/" element={<PrivateRoute redirectPath={"/login"} allowedRoles={[Auth.Roles.VERIFIED_USER, Auth.Roles.ADMIN_USER]} />}>
							<Route path="/account" element={<AccountPage />} />
							<Route path="/logout" element={<LogoutPage />} />
							<Route path="/notification" element={<NotificationPage />} />
						</Route>
						<Route path="/" element={<PrivateRoute redirectPath={"/login"} allowedRoles={[Auth.Roles.ADMIN_USER]}/>}>
							<Route path="/admin-dashboard" element={<AdminDashboardPage />} />
						</Route>
					</Routes>
				</BrowserRouter>
			</div>
		</NotificationStateProvider>
  	);
}

export default App;
