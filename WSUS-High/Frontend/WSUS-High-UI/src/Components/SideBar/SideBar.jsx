import { useState } from "react";
import { Container, Row, Col } from "react-bootstrap";
import SideBarButton from "./SideBarButton";

const SideBar = () => {
  const [activeButton, setActiveButton] = useState("Overview");

  const buttonData = [
    { title: "Overview", icon: "house", testId: "overviewBtn" },
    { title: "Updates", icon: "file-arrow-down", testId: "updatesBtn" },
    { title: "Clients", icon: "network-wired", testId: "clientsBtn" },
    { title: "Sync Settings", icon: "rotate", testId: "syncsettingsBtn" },
    { title: "History", icon: "clock-rotate-left", testId: "historyBtn" },
  ];

  const handleClick = (clickedTitle) => {
    setActiveButton(clickedTitle);
  };

  return (
    <Container bg="black" className="SideBar">
      <Row>
        {buttonData.map((buttonProps) => (
          <Col xs="12" key={buttonProps.title}>
            <SideBarButton
              {...buttonProps}
              isActive={activeButton === buttonProps.title}
              onClick={() => handleClick(buttonProps.title)}
            />
          </Col>
        ))}
      </Row>
    </Container>
  );
};

export default SideBar;
