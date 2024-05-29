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
  CardHeader,
} from "react-bootstrap";
import { API_URL } from "../../Utils/Settings";
import Utils from "../../Utils/Utils";
import AddClientModal from "../Modals/AddClientModal";
import ConfirmDeleteModal from "../Modals/ConfirmDeleteModal";
import DetailedCard from "../Cards/DetailedCard";

const Clients = (props) => {
  const { checkConnection, apiConnection, dbConnection, updateTime } = props;
  const [isLoading, setLoading] = useState(false);
  const [computers, setComputers] = useState([]);
  const [selectedComputer, setSelectedComputer] = useState(null);

  const [showAddClientModal, setShowAddClientModal] = useState(false);
  const [showConfirmDeleteModal, setShowConfirmDeleteModal] = useState(false);
  const [showDetailedCard, setShowDetailedCard] = useState(false);

  const getComputers = async () => {
    try {
      const response = await axios.get(API_URL + "/api/computers");
      const computers = response.data;
      setComputers(computers);
    } catch (error) {
      Utils.handleAxiosError(error);
    }
  };

  const simulateLoading = () => {
    return new Promise((resolve) => setTimeout(resolve, 1000));
  };

  const handleRefresh = () => {
    setLoading(true);
    checkConnection();
    getComputers();
    simulateLoading().then(() => {
      setLoading(false);
    });
  };

  const handleDetailedCard = (computer) => {
    if (
      selectedComputer &&
      selectedComputer.computerID === computer.computerID
    ) {
      setShowDetailedCard(!showDetailedCard);
    } else {
      setSelectedComputer(computer);
      setShowDetailedCard(true);
    }
  };

  useEffect(() => {
    console.log("Overview mounted");

    getComputers();
  }, []);

  return (
    <Container fluid>
      <Row className="g-2">
        <Col xs="12">
          <Card className="px-3 py-2">
            <Row className="align-items-center">
              <Col as="h2" xs="auto" className="title m-0">
                <FontAwesomeIcon icon="network-wired" className="me-2" />
                Clients
              </Col>
              <Col xs="auto">
                <span>
                  <b>Last updated:</b> {updateTime}
                </span>
              </Col>
              <Col className="text-end">
                <Button
                  className="me-2"
                  onClick={() => setShowAddClientModal(true)}
                  disabled={!dbConnection && !apiConnection}
                >
                  <FontAwesomeIcon icon="plus" /> New Client
                </Button>
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
        <Col xs="6">
          <Card className="p-2">
            <Table striped bordered responsive hover className="m-0">
              <thead>
                <tr>
                  <th>#</th>
                  <th>Name</th>
                  <th>IP</th>
                  <th>OS Version</th>
                  <th>Last Connections</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {computers.map((computer, index) => (
                  <tr key={index}>
                    <td
                      onClick={() => handleDetailedCard(computer)}
                      title="See More"
                    >
                      {computer.computerID}
                    </td>
                    <td
                      onClick={() => handleDetailedCard(computer)}
                      title="See More"
                    >
                      {computer.computerName}
                    </td>
                    <td
                      onClick={() => handleDetailedCard(computer)}
                      title="See More"
                    >
                      {computer.ipAddress}
                    </td>
                    <td
                      onClick={() => handleDetailedCard(computer)}
                      title="See More"
                    >
                      {computer.osVersion}
                    </td>
                    <td
                      onClick={() => handleDetailedCard(computer)}
                      title="See More"
                    >
                      {computer.lastConnection
                        ? new Date(computer.lastConnection).toLocaleString(
                            "en-GB",
                            {
                              formatMatcher: "best fit",
                            }
                          )
                        : "N/A"}
                    </td>
                    <td className="p-0">
                      <Button
                        variant="danger"
                        onClick={() => {
                          setSelectedComputer(computer);
                          setShowConfirmDeleteModal(true);
                        }}
                        className="w-100"
                      >
                        <FontAwesomeIcon icon="trash-can" />
                      </Button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </Table>
          </Card>
        </Col>
        <Col xs="6">
          <Card>
            <CardHeader as={"h3"} className="text-center mb-3 title">
              Update Planner
            </CardHeader>
            <h3 className="text-center">TBA</h3>
          </Card>
        </Col>
        <Col>
          {showDetailedCard && (
            <DetailedCard
              key={selectedComputer ? selectedComputer.computerID : null}
              hide={() => {
                setShowDetailedCard(false);
              }}
              computer={selectedComputer}
              handleRefresh={handleRefresh}
              deleteClient={() => setShowConfirmDeleteModal(true)}
            />
          )}
        </Col>
      </Row>
      {showAddClientModal && (
        <AddClientModal
          show={showAddClientModal}
          hide={() => setShowAddClientModal(false)}
          handleRefresh={handleRefresh}
        />
      )}
      {showConfirmDeleteModal && (
        <ConfirmDeleteModal
          show={showConfirmDeleteModal}
          hide={() => {
            setShowConfirmDeleteModal(false);
          }}
          computer={selectedComputer}
          handleRefresh={handleRefresh}
        />
      )}
    </Container>
  );
};

export default Clients;
