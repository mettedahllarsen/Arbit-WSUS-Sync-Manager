import { useEffect, useState } from "react";
// import axios from "axios";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  Container,
  Row,
  Col,
  Card,
  Button,
  Spinner,
  CardHeader,
  CardBody,
} from "react-bootstrap";
// import { API_URL } from "../../Utils/Settings";
// import Utils from "../../Utils/Utils";

const Overview = (props) => {
  const { checkConnection, apiConnection, dbConnection, updateTime } = props;
  const [isLoading, setLoading] = useState(false);

  const simulateLoading = () => {
    return new Promise((resolve) => setTimeout(resolve, 1000));
  };

  const handleRefresh = () => {
    setLoading(true);
    checkConnection();
    simulateLoading().then(() => {
      setLoading(false);
    });
  };

  useEffect(() => {
    console.log("Overview mounted");
  }, []);

  return (
    <Container fluid>
      <Row className="g-2">
        <Col xs="12">
          <Card className="px-3 py-2">
            <Row className="align-items-center">
              <Col as="h2" xs="auto" className="title m-0">
                <FontAwesomeIcon icon="house" className="me-2" />
                Overview
              </Col>
              <Col xs="auto">
                <span>
                  <b>Last updated:</b> {updateTime}
                </span>
              </Col>
              <Col className="text-end">
                <Button
                  variant="primary"
                  onClick={handleRefresh}
                  className="mb-0"
                >
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

        {/* Partially Working */}
        <Col xs="4">
          <Card>
            <CardHeader
              as="h3"
              className={
                dbConnection
                  ? "bg-success text-white px-2"
                  : "bg-danger text-white px-2"
              }
            >
              <Row>
                <Col>Status</Col>
                <Col xs="auto">
                  {dbConnection ? (
                    <FontAwesomeIcon icon="circle-check" />
                  ) : (
                    <FontAwesomeIcon icon="circle-xmark" />
                  )}
                </Col>
              </Row>
            </CardHeader>
            <CardBody className="p-2 text-center">
              <Row className="g-4">
                <Col md={12} xl={12}>
                  <span className="title">
                    <FontAwesomeIcon icon="circle-play" /> <b>WSUSHigh API:</b>{" "}
                  </span>
                  {apiConnection ? (
                    <span className="text-success">
                      <b>Online</b>
                    </span>
                  ) : (
                    <span className="text-danger">
                      <b>Offline</b>
                    </span>
                  )}
                </Col>

                <Col md={12} xl={12}>
                  <span className="title">
                    <FontAwesomeIcon icon="circle-play" /> <b>WSUSHigh DB:</b>{" "}
                  </span>
                  {dbConnection ? (
                    <span className="text-success">
                      <b>Online</b>
                    </span>
                  ) : (
                    <span className="text-danger">
                      <b>Offline</b>
                    </span>
                  )}
                </Col>
              </Row>
            </CardBody>
          </Card>
        </Col>

        {/* Not Working */}
        <Col xs="4">
          <Card>
            <CardHeader as="h3" className="title text-center">
              Latest Syncronization
            </CardHeader>
            <CardBody className="p-2 text-center">
              <span>27/05/2024, 12:42:23</span>
            </CardBody>
          </Card>
        </Col>

        {/* Not Working */}
        <Col xs="4" className="h-100">
          <Card className="">
            <CardHeader as="h3" className="title text-center">
              Available Updates
            </CardHeader>
            <CardBody className="p-2 text-center">
              <span>
                <b>7 updates are available</b>
              </span>
            </CardBody>
          </Card>
        </Col>
      </Row>
    </Container>
  );
};

export default Overview;
