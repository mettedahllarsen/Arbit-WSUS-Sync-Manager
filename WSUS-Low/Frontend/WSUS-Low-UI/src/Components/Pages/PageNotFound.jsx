import { Container } from "react-bootstrap";
import image from "../../assets/404.jpg";

const PageNotFound = () => {
  return (
    <Container fluid className="px-2 py-3 text-center">
      <h1>404</h1>
      <h2>Page Not Found</h2>
      <img src={image} alt="404" />
    </Container>
  );
};

export default PageNotFound;
