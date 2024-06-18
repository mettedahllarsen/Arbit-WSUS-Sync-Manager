import { useEffect } from "react";
import { Navbar, Stack, NavbarText } from "react-bootstrap";

const Header = (props) => {
  const { title, content } = props;

  useEffect(() => {
    console.log("Component Header mounted");
  }, []);

  // TODO: Use logo component
  return (
    <Navbar className="px-2 HeaderBar">
      <Stack direction="horizontal" gap={2} className="w-100">
        <NavbarText className="ms-2 p-0">
          <h1 className="text-dark logoText m-0">
            a<span className="text-danger">r</span>bit
          </h1>
        </NavbarText>
        <NavbarText>{title}</NavbarText>
        <NavbarText className="ms-auto">{content}</NavbarText>
      </Stack>
    </Navbar>
  );
};

export default Header;
