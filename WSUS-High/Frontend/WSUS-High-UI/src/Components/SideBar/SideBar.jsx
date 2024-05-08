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
          <Nav.Link href="/updates" className="nav-button">
            <FontAwesomeIcon icon={faFileArrowDown} className="nav-icon me-4" />
            Updates
          </Nav.Link>
          <Nav.Link href="/updates" className="nav-button">
            <FontAwesomeIcon icon={faNetworkWired} className="nav-icon me-4" />
            Clients
          </Nav.Link>
          <Nav.Link href="/updates" className="nav-button">
            <FontAwesomeIcon icon="rotate" className="nav-icon me-4" />
            Sync {/* Find ud af navn */}
          </Nav.Link>
          <Nav.Link href="/updates" className="nav-button">
            <FontAwesomeIcon
              icon={faClockRotateLeft}
              className="nav-icon me-4"
            />
            History
          </Nav.Link>
        </Nav>
      </Navbar.Collapse>
    </Navbar>
  );
};

export default SideBar;
