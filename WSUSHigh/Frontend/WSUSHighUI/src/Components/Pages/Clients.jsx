import { useEffect, useState } from "react";
import axios from "axios";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  Row,
  Col,
  Button,
  Table,
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

  return (
    <>
      <Row className="g-2">
        <Col xs="12">
          <TitleCard
            title={"Clients"}
            icon={"network-wired"}
            handleRefresh={handleRefresh}
            isLoading={isLoading}
            updateTime={updateTime}
            extraButton={
              <Col className="text-end me-2">
                <Button
                  onClick={() => setShowAddClientModal(true)}
                  disabled={!dbConnection && !apiConnection}
                >
                  <FontAwesomeIcon icon="plus" /> New Client
                </Button>
              </Col>
            }
          />
        </Col>
        <Col xs="12">
          <Table bordered hover className="my-1">
            <thead>
              <tr>
                <th>IP-address</th>
                <th>Name</th>
                <th>OS-version</th>
                <th>Last connection</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {computers.map((computer) => (
                <tr key={computer.computerID}>
                  <td>{computer.ipAddress}</td>
                  <td>{computer.computerName}</td>
                  <td>{computer.osVersion ? computer.osVersion : "N/A"}</td>
                  <td>
                    {new Date(computer.lastConnection).toLocaleString("en-GB", {
                      formatMatcher: "best fit",
                    })}
                  </td>
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
        </Col>
        <Col>
          {showDetailedCard && (
            <DetailedCard
              key={selectedComputer ? selectedComputer.computerID : null}
              hide={() => {
                setShowDetailedCard(false);
              }}
              selectedComputer={selectedComputer}
              setSelectedComputer={setSelectedComputer}
              handleRefresh={handleRefresh}
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
    </>
  );
};

export default Clients;
