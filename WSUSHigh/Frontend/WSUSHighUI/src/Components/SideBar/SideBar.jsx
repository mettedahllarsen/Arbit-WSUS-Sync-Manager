import { useState } from "react";
import { Row, Col } from "react-bootstrap";
import SideBarButton from "./SideBarButton";

const SideBar = () => {
  const activeOnReload = () => {
    var path = window.location.pathname;
    return path == "/"
      ? (path = "Overview")
      : (path = path.replaceAll("/", ""));
  };

  const [activeButton, setActiveButton] = useState(activeOnReload);

  const buttonData = [
    { title: "Overview", icon: "house", testId: "overviewBtn" },
    { title: "Updates", icon: "file-arrow-down", testId: "updatesBtn" },
    { title: "Clients", icon: "network-wired", testId: "clientsBtn" },
    { title: "Sync Settings", icon: "rotate", testId: "syncsettingsBtn" },
    { title: "History", icon: "clock-rotate-left", testId: "historyBtn" },
  ];

  const handleClick = (clickedTitle) => {
    setActiveButton(clickedTitle.replaceAll(" ", ""));
  };

  return (
    <Row>
      {buttonData.map((buttonProps) => (
        <Col xs="12" key={buttonProps.title}>
          <SideBarButton
            {...buttonProps}
            isActive={activeButton == buttonProps.title.replaceAll(" ", "")}
            onClick={() => handleClick(buttonProps.title)}
          />
        </Col>
      ))}
    </Row>
  );
};

export default SideBar;
