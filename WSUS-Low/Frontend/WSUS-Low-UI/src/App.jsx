import { useEffect } from "react";
import { Routes, Route } from "react-router-dom";
import { library } from "@fortawesome/fontawesome-svg-core";
import {
  faHouse,
  faGear,
  faRotate,
  faClockRotateLeft,
  faNetworkWired,
  faFileArrowDown,
} from "@fortawesome/free-solid-svg-icons";
import Header from "./Components/Header/Header";
import HeaderNav from "./Components/Header/HeaderNav";
import HeaderTitle from "./Components/Header/HeaderTitle";
import Overview from "./Components/Pages/Overview";
import SideBar from "./Components/SideBar/SideBar";
import Second from "./Components/Pages/Second";
import PageNotFound from "./Components/Pages/PageNotFound";

library.add(
  faHouse,
  faGear,
  faRotate,
  faClockRotateLeft,
  faNetworkWired,
  faFileArrowDown
);

const App = () => {
  useEffect(() => {
    console.log("App mounted");
  }, []);

  return (
    <div className="app-container">
      <div className="header-container">
        <Header title={<HeaderTitle />} content={<HeaderNav />} />
      </div>

      <div className="content-container">
        <SideBar />

        <Routes>
          <Route index element={<Overview />} />
          <Route path="second" element={<Second />} />
          <Route path="*" element={<PageNotFound />} />
        </Routes>
      </div>
    </div>
  );
};

export default App;
