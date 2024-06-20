import { useEffect, useState } from "react";
import { Container, Row, Col } from "react-bootstrap";
import { Routes, Route } from "react-router-dom";
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
import Overview from "./Components/Pages/Overview";
import SideBar from "./Components/SideBar/SideBar";
import PageNotFound from "./Components/Pages/PageNotFound";
import Updates from "./Components/Pages/Updates";
import axios from "axios";
import { API_URL } from "./Utils/Settings";
import Utils from "./Utils/Utils";
import Clients from "./Components/Pages/Clients";
import SyncSettings from "./Components/Pages/SyncSettings";
import History from "./Components/Pages/History";

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

  // TODO: Make a way to check the two connections individually
  const checkConnection = async () => {
    setUpdateTime(
      new Date().toLocaleString("en-GB", {
        formatMatcher: "best fit",
      })
    );
    try {
      await axios({
        method: "get",
        url: API_URL + "Computers",
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
    <Container fluid className="gx-0 Content">
      <Header title={<HeaderTitle />} content={<HeaderNav />} />
      <Row className="g-0">
        <Col xs="auto">
          <SideBar />
        </Col>
        <Col className="Page py-3">
          <Routes>
            <Route
              path="/"
              element={
                <Overview
                  checkConnection={checkConnection}
                  apiConnection={apiConnection}
                  dbConnection={dbConnection}
                  /*updateTime={updateTime}*/
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
          </Routes>
        </Col>
      </Row>
    </Container>
  );
};

export default App;
