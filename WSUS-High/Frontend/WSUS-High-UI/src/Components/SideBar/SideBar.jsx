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
          <Nav.Link href="/Updates" className="nav-button">
            <FontAwesomeIcon icon="file-arrow-down" className="nav-icon me-4" />
            Updates
          </Nav.Link>
          <Nav.Link href="/Clients" className="nav-button">
            <FontAwesomeIcon icon="network-wired" className="nav-icon me-4" />
            Clients
          </Nav.Link>
          <Nav.Link href="/Syncronization-Settings" className="nav-button">
            <FontAwesomeIcon icon="rotate" className="nav-icon me-4" />
            Sync Settings {/* TODO: Find ud af navn */}
          </Nav.Link>
          <Nav.Link href="/Activity" className="nav-button">
            <FontAwesomeIcon
              icon="clock-rotate-left"
              className="nav-icon me-4"
            />
            Activity
          </Nav.Link>
        </Nav>
      </Navbar.Collapse>
    </Navbar>
  );
};

export default SideBar;
