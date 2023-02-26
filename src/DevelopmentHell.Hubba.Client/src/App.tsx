import React from "react";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import Account from "./pages/Account/Account";
import Analytics from "./pages/Analytics/Analytics";
import Home from "./pages/Home/Home";
import Login from "./pages/Login/Login";
import Logout from "./pages/Logout/Logout";
import Registration from "./pages/Registration/Registration";
import Otp from "./pages/Otp/Otp";

import "./Theme.css";

interface Props {

}

const App: React.FC<Props> = (props) => {
	return (
		<div className="App">
			<BrowserRouter>
				<Routes>
					<Route path="/" element={<Home />} />
					<Route path="/registration" element={<Registration />} />
					<Route path="/login" element={<Login />} />
					<Route path="/otp" element={<Otp />} />
					<Route path="/logout" element={<Logout />} />
					<Route path="/analytics" element={<Analytics />} />
					<Route path="/account" element={<Account />} />
				</Routes>
			</BrowserRouter>
		</div>
  	);
}

export default App;
