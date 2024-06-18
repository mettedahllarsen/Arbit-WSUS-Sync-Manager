//import PropTypes from "prop-types";
import { useEffect } from "react";
// import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Container, Card } from "react-bootstrap";

const Second = () => {
  useEffect(() => {
    console.log("Updates mounted");
  }, []);

  return (
    <Container fluid className="px-2 py-3">
      <Card>Hej</Card>
    </Container>
  );
};

//Overview.propTypes = {};

export default Second;
