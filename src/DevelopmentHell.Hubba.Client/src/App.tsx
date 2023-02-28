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

interface Props {

}

const App: React.FC<Props> = (props) => {
	return (
		<div className="App">
			<BrowserRouter>
				<Routes>
					//add new page in alphabetical order
					<Route path="/" element={<Home />} />

					<Route path="/account" element={<Account />} />
					<Route path="/analytics" element={<Analytics />} />
					<Route path="/login" element={<Login />} />
					<Route path="/logout" element={<Logout />} />
					<Route path="/notification" element={<Notification />} />
					<Route path="/otp" element={<Otp />} />
					<Route path="/registration" element={<Registration />} />
					
				</Routes>
			</BrowserRouter>
		</div>
  	);
}

export default App;
