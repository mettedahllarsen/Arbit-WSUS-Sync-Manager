import { useEffect, useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Container, Card, Row, Col, Button, Spinner } from "react-bootstrap";

const Activity = () => {
  const [isLoading, setLoading] = useState(false);

  useEffect(() => {
    console.log("Activity mounted");
  }, []);

  const handleRefresh = () => {
    isLoading ? setLoading(false) : setLoading(true);
  };

  return (
    <Container fluid>
      <Row className="g-2">
        <Col>
          <Card className="px-3 py-2">
            <Row className="align-items-center">
              <Col as="h2" xs="auto" className="title m-0">
                <FontAwesomeIcon icon="clock-rotate-left" className="me-2" />
                Activity
              </Col>
              <Col xs="auto">
                <span>
                  <b>Last updated:</b>{" "}
                  {new Date().toLocaleString("en-GB", {
                    formatMatcher: "best fit",
                  })}
                </span>
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

export default Activity;
