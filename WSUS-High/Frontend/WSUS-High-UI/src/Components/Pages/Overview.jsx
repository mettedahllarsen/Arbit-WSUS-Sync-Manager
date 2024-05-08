//import PropTypes from "prop-types";
import { useEffect, useState } from "react";
import axios from "axios";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  Container,
  Row,
  Col,
  Card,
  Button,
  Spinner,
  Table,
} from "react-bootstrap";
import { API_URL } from "../../Utils/Settings";
import Utils from "../../Utils/Utils";

const Overview = () => {
  const [isLoading, setLoading] = useState(false);
  const [computers, setComputers] = useState([]);

  const getComputers = async () => {
    try {
      const response = await axios.get(API_URL + "/api/computers");
      const computers = response.data;
      setComputers(computers);
    } catch (error) {
      Utils.handleAxiosError(error);
    }
  };

  const handleRefresh = () => {
    if (isLoading) {
      setLoading(false);
    } else {
      setLoading(true);
    }
  };

  useEffect(() => {
    console.log("Overview mounted");

    getComputers();
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
        <Col>
          <Card className="p-2">
            <h5>Computers</h5>
            <Table striped bordered responsive hover>
              <thead>
                <tr>
                  <th>#</th>
                  <th>Name</th>
                  <th>IP</th>
                  <th>OS Version</th>
                  <th>Last Connections</th>
                </tr>
              </thead>
              <tbody>
                {computers.map((computer, index) => (
                  <tr key={index}>
                    <td>{computer.computerID}</td>
                    <td>{computer.computerName}</td>
                    <td>{computer.ipAddress}</td>
                    <td>{computer.osVersion}</td>
                    <td>
                      {computer.lastConnection
                        ? new Date(computer.lastConnection).toLocaleString(
                            "en-GB",
                            {
                              year: "numeric",
                              month: "numeric",
                              day: "numeric",
                              hour: "numeric",
                              minute: "numeric",
                              second: "numeric",
                            }
                          )
                        : "N/A"}
                    </td>
                  </tr>
                ))}
              </tbody>
            </Table>
            {/* Backend, Database osv. */}
          </Card>
        </Col>
      </Row>
    </Container>
  );
};

//Overview.propTypes = {};

export default Overview;
