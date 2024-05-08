//import PropTypes from "prop-types";
import { useEffect, useState } from "react";
// import axios from "axios";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Container, Row, Col, Card, Button, Spinner } from "react-bootstrap";
// import { API_URL } from "../../Utils/Settings";
// import Utils from "../../Utils/Utils";

const Overview = () => {
  const [isLoading, setLoading] = useState(false);

  const handleRefresh = () => {
    if (isLoading) {
      setLoading(false);
    } else {
      setLoading(true);
    }
  };

  useEffect(() => {
    console.log("Overview mounted");
  }, []);

  return (
    <Container fluid className="px-2 py-3">
      <Row className="g-2">
        <Col xs="12">
          <Card className="p-2">
            <Row className="align-items-center">
              <Col as="h3" xs="auto" className="title m-0">
                <FontAwesomeIcon icon="house" className="me-2" />
                Overview
              </Col>
              <Col xs="auto">
                <b>Last Updated:</b>{" "}
                {new Date().toLocaleString("en-GB", {
                  formatMatcher: "best fit",
                })}
              </Col>
              <Col className="text-end">
                <Button variant="primary" onClick={handleRefresh}>
                  {isLoading ? (
                    <Spinner animation="border" role="status" size="sm" />
                  ) : (
                    <FontAwesomeIcon icon="rotate" />
                  )}
                </Button>
              </Col>
            </Row>
          </Card>
        </Col>
      </Row>
    </Container>
  );
};

//Overview.propTypes = {};

export default Overview;
