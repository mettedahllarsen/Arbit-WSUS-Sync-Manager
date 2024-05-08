import {
  faClockRotateLeft,
  faFileArrowDown,
  faNetworkWired,
} from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Navbar, Nav } from "react-bootstrap";

const SideBar = () => {
  return (
    <Navbar bg="black" className="SideBar" variant="dark">
      <Navbar.Collapse>
        <Nav>
          <Nav.Link href="/" className="nav-button">
            <FontAwesomeIcon icon="house" className="nav-icon me-4" />
            Overview
          </Nav.Link>
          <Nav.Link href="/second" className="nav-button">
            <FontAwesomeIcon icon={faFileArrowDown} className="nav-icon me-4" />
            Second
          </Nav.Link>
        </Nav>
      </Navbar.Collapse>
    </Navbar>
  );
};

export default SideBar;
