import { useEffect, useState } from "react";
import axios from "axios";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  Container,
  Row,
  Col,
  Card,
  Button,
  Table,
  CardHeader,
  Toast,
  ToastContainer,
} from "react-bootstrap";
import { API_URL } from "../../Utils/Settings";
import Utils from "../../Utils/Utils";
import AddClientModal from "./Modals/AddClientModal";
import ConfirmDeleteModal from "./Modals/ConfirmDeleteModal";
import DetailedCard from "../Cards/DetailedCard";
import TitleCard from "../Cards/TitleCard";

const Clients = (props) => {
  const { checkConnection, apiConnection, dbConnection, updateTime } = props;
  const [isLoading, setLoading] = useState(false);
  const [computers, setComputers] = useState([]);
  const [selectedComputer, setSelectedComputer] = useState({});

  const [showAddClientModal, setShowAddClientModal] = useState(false);
  const [showConfirmDeleteModal, setShowConfirmDeleteModal] = useState(false);
  const [showDetailedCard, setShowDetailedCard] = useState(false);

  const [showToast, setShowToast] = useState(false);
  const [toastData, setToastData] = useState({
    success: false,
    message: "",
  });

  const handleToast = (success, message) => {
    setToastData({ success, message });
    setShowToast(true);
  };

  const getComputers = async () => {
    try {
      const response = await axios.get(API_URL + "/api/Computers");
      const computers = response.data;
      setComputers(computers);
    } catch (error) {
      Utils.handleAxiosError(error);
    }
  };

  const handleRefresh = () => {
    setLoading(true);
    checkConnection();
    getComputers();
    Utils.simulateLoading().then(() => {
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
    console.log("Clients mounted");

    getComputers();
  }, []);

  const processTableFields = (computer) => {
    const computerProperties = [
      "computerID",
      "computerName",
      "ipAddress",
      "osVersion",
      "lastConnection",
    ];

    return computerProperties.map((prop) => (
      <td key={prop}>
        {prop == "lastConnection"
          ? computer.lastConnection
            ? new Date(computer.lastConnection).toLocaleString("en-GB", {
                formatMatcher: "best fit",
              })
            : "N/A"
          : computer[prop]}
      </td>
    ));
  };

  return (
    <Container fluid>
      <ToastContainer position="bottom-end" className="mb-4 me-4">
        <Toast
          onClose={() => setShowToast(false)}
          show={showToast}
          className={
            toastData.success ? "toastSuccess p-2" : "toastFailure p-2"
          }
          delay={6000}
          autohide
        >
          <b>{toastData.message}</b>
        </Toast>
      </ToastContainer>
      <Row className="g-2">
        <Col xs="12">
          <TitleCard
            title={"Clients"}
            icon={"network-wired"}
            handleRefresh={handleRefresh}
            isLoading={isLoading}
            updateTime={updateTime}
          />
        </Col>
        <Col xs="12" xl="8">
          <Card>
            <CardHeader>
              <Row className="align-items-center">
                <Col xs="auto" as="h4" className="title mb-0">
                  Connected clients
                </Col>
                <Col className="text-end">
                  <Button
                    onClick={() => setShowAddClientModal(true)}
                    disabled={!dbConnection && !apiConnection}
                  >
                    <FontAwesomeIcon icon="plus" /> Add Client
                  </Button>
                </Col>
              </Row>
            </CardHeader>
            <Table bordered hover className="m-0">
              <thead>
                <tr>
                  <th>#</th>
                  <th>Name</th>
                  <th>IP-address</th>
                  <th>OS-version</th>
                  <th>Last connection</th>
                </tr>
              </thead>
              <tbody>
                {computers.map((computer) => (
                  <tr key={computer.computerID}>
                    {processTableFields(computer)}
                    <td className="p-1 ">
                      <Row className="g-1 justify-content-center">
                        <Col xs="auto">
                          <Button onClick={() => handleDetailedCard(computer)}>
                            <FontAwesomeIcon icon="circle-info" />
                          </Button>
                        </Col>
                        <Col xs="auto">
                          <Button
                            variant="danger"
                            onClick={() => {
                              setSelectedComputer(computer);
                              setShowConfirmDeleteModal(true);
                            }}
                          >
                            <FontAwesomeIcon icon="trash-can" />
                          </Button>
                        </Col>
                      </Row>
                    </td>
                  </tr>
                ))}
              </tbody>
            </Table>
          </Card>
        </Col>
        <Col xs="12" xl="4">
          <Card>
            <CardHeader as={"h4"} className="text-center mb-2 title">
              Update Planner
            </CardHeader>
            <div className="text-center biggerText">
              <b>TBA</b>
            </div>
          </Card>
        </Col>
        <Col xs="12" xl="8">
          {showDetailedCard && (
            <DetailedCard
              key={selectedComputer ? selectedComputer.computerID : null}
              hide={() => {
                setShowDetailedCard(false);
              }}
              selectedComputer={selectedComputer}
              setSelectedComputer={setSelectedComputer}
              handleRefresh={handleRefresh}
              deleteClient={() => setShowConfirmDeleteModal(true)}
              handleToast={handleToast}
            />
          )}
        </Col>
      </Row>
      {showAddClientModal && (
        <AddClientModal
          show={showAddClientModal}
          hide={() => setShowAddClientModal(false)}
          handleRefresh={handleRefresh}
          handleToast={handleToast}
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
          handleToast={handleToast}
        />
      )}
    </Container>
  );
};

export default Clients;
