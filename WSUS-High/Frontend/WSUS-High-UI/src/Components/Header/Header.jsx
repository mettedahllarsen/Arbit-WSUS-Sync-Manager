import { useEffect } from "react";
import PropTypes from "prop-types";
import { Navbar, Stack, NavbarText } from "react-bootstrap";

const Header = (props) => {
  useEffect(() => {
    console.log("Component Header mounted");
  }, []);

  return (
    <Navbar className="px-2 HeaderBar">
      <Stack direction="horizontal" gap={3} className="w-100">
        <NavbarText>
          <h1>Arbit Logo</h1>
        </NavbarText>
        <NavbarText>{props.title}</NavbarText>
        <NavbarText className="ms-auto">{props.content}</NavbarText>
      </Stack>
    </Navbar>
  );
};

Header.propTypes = {
  title: PropTypes.object.isRequired,
  content: PropTypes.object.isRequired,
};

export default Header;
