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
import NotificationStateProvider from "./NotificationStateProvider";
import OpenTimeSlotsView from "./pages/SchedulingPage/OpenSlotsView";
import "./App.css";
import OpenSlotsView from "./pages/SchedulingPage/OpenSlotsView";
import CollaboratorPage from "./pages/CollaboratorPage/CollaboratorPage";
import ListingProfilePage from "./pages/ListingProfilePage/ListingProfilePage";
import ViewListingPage from "./pages/ListingPage/ViewListingPage.tsx/ViewListingPage";
import EditListingPage from "./pages/ListingPage/EditListingPage.tsx/EditListingPage";
import ViewListingRatingsPage from "./pages/ListingPage/ViewListingPage.tsx/ViewListingRatingsPage/ViewListingRatingsPage";

interface IAppProps {

}

const App: React.FC<IAppProps> = (props) => {
	return (
		<NotificationStateProvider>
			<div id="App" className="App">
				<BrowserRouter>
					<Routes>
						{/* Anyone can access */}
						<Route index element={<HomePage />} />
						<Route path="*" element={<Navigate to='/' replace />} />
						<Route path="/unauthorized" element={<Unauthorized />} />
						<Route path="/viewlisting" element={<ViewListingPage />} />
						<Route path="/viewlistingratings" element={<ViewListingRatingsPage /> } />
						
						
						
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
						<Route path="/scheduling" element={
							<PublicOutlet redirectPath="/">
								<OpenSlotsView listingId={0} />
							</PublicOutlet>
						} />
						
						
						{/* Protect/private routes - with auth */}
						<Route path="/" element={<PrivateRoute redirectPath={"/login"} allowedRoles={[Auth.Roles.VERIFIED_USER, Auth.Roles.ADMIN_USER]} />}>
							<Route path="/account" element={<AccountPage />} />
							<Route path="/logout" element={<LogoutPage />} />
							<Route path="/notification" element={<NotificationPage />} />
							<Route path="/collaborator" element={<CollaboratorPage/>}/>
							<Route path="/listingprofile" element={<ListingProfilePage />} />
							<Route path="/editlisting" element={<EditListingPage />} />
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
