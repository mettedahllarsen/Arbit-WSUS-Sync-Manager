import { useEffect, useState } from "react";
// import axios from "axios";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  Container,
  Row,
  Col,
  Card,
  CardHeader,
  CardBody,
} from "react-bootstrap";
import TitleCard from "../Cards/TitleCard";
import Utils from "../../Utils/Utils";
// import { API_URL } from "../../Utils/Settings";

const Overview = (props) => {
  const { checkConnection, apiConnection, dbConnection } = props;
  const [isLoading, setLoading] = useState(false);

  const handleRefresh = () => {
    setLoading(true);
    checkConnection();
    Utils.simulateLoading().then(() => {
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
          <TitleCard
            title={"Overview"}
            icon={"house"}
            handleRefresh={handleRefresh}
            isLoading={isLoading}
          />
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
                    <span
                      className="text-success"
                      data-testid="apiStatusResult"
                    >
                      <b>Online</b>
                    </span>
                  ) : (
                    <span className="text-danger" data-testid="apiStatusResult">
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
