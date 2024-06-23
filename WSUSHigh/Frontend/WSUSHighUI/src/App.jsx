import { useEffect, useState } from "react";
import { Routes, Route, Outlet, BrowserRouter } from "react-router-dom";
import axios from "axios";
import { Row } from "react-bootstrap";
import { library } from "@fortawesome/fontawesome-svg-core";
import {
  faHouse,
  faGear,
  faRotate,
  faClockRotateLeft,
  faNetworkWired,
  faFileArrowDown,
  faCirclePlay,
  faCircleCheck,
  faCircleXmark,
  faPlus,
  faTrashCan,
  faCircleInfo,
  faWindowMinimize,
} from "@fortawesome/free-solid-svg-icons";
import Header from "./Components/Header/Header";
import HeaderNav from "./Components/Header/HeaderNav";
import HeaderTitle from "./Components/Header/HeaderTitle";
import SideBar from "./Components/SideBar/SideBar";
import Overview from "./Components/Pages/Overview";
import Updates from "./Components/Pages/Updates";
import Clients from "./Components/Pages/Clients";
import SyncSettings from "./Components/Pages/SyncSettings";
import History from "./Components/Pages/History";
import PageNotFound from "./Components/Pages/PageNotFound";
import { API_URL } from "./Utils/Settings";
import Utils from "./Utils/Utils";

library.add(
  faHouse,
  faGear,
  faRotate,
  faClockRotateLeft,
  faNetworkWired,
  faFileArrowDown,
  faCirclePlay,
  faCircleCheck,
  faCircleXmark,
  faPlus,
  faTrashCan,
  faCircleInfo,
  faWindowMinimize
);

const Layout = () => {
  return (
    <>
      <Header title={<HeaderTitle />} content={<HeaderNav />} />
      <Row className="g-0">
        <div className="SideBar">
          <SideBar />
        </div>
        <div className="Page">
          <div className="w-100">
            <Outlet />
          </div>
        </div>
      </Row>
    </>
  );
};

const App = () => {
  const [apiConnection, setApiConnection] = useState(false);
  const [dbConnection, setDbConnection] = useState(false);
  const [updateTime, setUpdateTime] = useState(
    new Date().toDateString("en-GB", {
      formatMatcher: "best fit",
    })
  );

  useEffect(() => {
    console.log("App mounted");
    checkConnection();
  }, []);

  const checkConnection = async () => {
    setUpdateTime(
      new Date().toLocaleString("en-GB", {
        formatMatcher: "best fit",
      })
    );
    try {
      await axios({
        method: "get",
        url: API_URL + "/api/Computers",
      });
      setApiConnection(true);
      setDbConnection(true);
      return true;
    } catch (error) {
      Utils.handleAxiosError(error);
      setApiConnection(false);
      setDbConnection(false);
      return false;
    }
  };
  return (
    <BrowserRouter>
      <Routes>
        <Route element={<Layout />}>
          <Route
            path="/"
            element={
              <Overview
                checkConnection={checkConnection}
                apiConnection={apiConnection}
                dbConnection={dbConnection}
                updateTime={updateTime}
              />
            }
          />
          <Route path="/updates" element={<Updates />} />
          <Route
            path="/clients"
            element={
              <Clients
                checkConnection={checkConnection}
                apiConnection={apiConnection}
                dbConnection={dbConnection}
                updateTime={updateTime}
              />
            }
          />
          <Route path="/syncsettings" element={<SyncSettings />} />
          <Route path="/history" element={<History />} />
          <Route path="*" element={<PageNotFound />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
};

export default App;
