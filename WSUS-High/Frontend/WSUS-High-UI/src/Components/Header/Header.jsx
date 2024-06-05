import { useEffect } from "react";
import { Navbar, Stack, NavbarText } from "react-bootstrap";

const Header = (props) => {
  const { title, content } = props;

  useEffect(() => {
    console.log("Component Header mounted");
  }, []);

  return (
    <Navbar className="px-2 HeaderBar">
      <Stack direction="horizontal" gap={3} className="w-100">
        <NavbarText>
          <h1>Arbit Logo</h1>
        </NavbarText>
        <NavbarText>{title}</NavbarText>
        <NavbarText className="ms-auto">{content}</NavbarText>
      </Stack>
    </Navbar>
  );
};

export default Header;
