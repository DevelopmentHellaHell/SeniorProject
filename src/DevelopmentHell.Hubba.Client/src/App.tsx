import React from "react";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import Account from "./pages/Account/Account";
import Analytics from "./pages/Analytics/Analytics";
import Home from "./pages/Home/Home";
import "./Theme.css";

interface Props {

}

const App: React.FC<Props> = (props) => {
	return (
		<div className="App">
			<BrowserRouter>
				<Routes>
				<Route path="/" element={<Home />} />
				<Route path="/analytics" element={<Analytics />} />
				<Route path="/account" element={<Account />} />
				</Routes>
			</BrowserRouter>
		</div>
  	);
}

export default App;
