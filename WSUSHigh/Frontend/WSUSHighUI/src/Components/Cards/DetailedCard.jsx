import { useCallback, useEffect, useState } from "react";
import {
  Card,
  CardHeader,
  ListGroup,
  ListGroupItem,
  CloseButton,
  Row,
  Col,
  Form,
  FormGroup,
  FormLabel,
  FormControl,
  Button,
} from "react-bootstrap";
import Utils from "../../Utils/Utils";
import ConfirmUpdateModal from "../Pages/Modals/ConfirmUpdateModal";
import { API_URL } from "../../Utils/Settings";
import axios from "axios";

const DetailedCard = (props) => {
  const {
    hide,
    selectedComputer,
    setSelectedComputer,
    handleRefresh,
    handleToast,
  } = props;

  const [computerName, setComputerName] = useState(
    selectedComputer.computerName
  );
  const [invalidName, setInvalidName] = useState(false);
  const [nameMessage, setNameMessage] = useState("");

  const [ipAddress, setIpAddress] = useState(selectedComputer.ipAddress);
  const [invalidIp, setInvalidIp] = useState(false);
  const [ipMessage, setIpMessage] = useState("");

  const [edit, setEdit] = useState(false);
  const [noChanges, setNoChanges] = useState(true);

  const [updateValues, setUpdateValues] = useState({
    name: null,
    ip: null,
  });
  const [showConfirmUpdateModal, setShowConfirmUpdateModal] = useState(false);

  const dontUpdateSameValues = useCallback(() => {
    if (
      selectedComputer.computerName == computerName &&
      selectedComputer.ipAddress == ipAddress
    ) {
      setNoChanges(true);
    } else {
      setNoChanges(false);
    }
  }, [
    selectedComputer.computerName,
    selectedComputer.ipAddress,
    computerName,
    ipAddress,
  ]);

  const nameHandler = (e) => {
    const input = e.target.value;
    const result = Utils.nameHandler(input);
    setInvalidName(result.invalid);
    result.invalid ? setNameMessage(result.message) : null;
    setComputerName(input);
  };

  const ipHandler = (e) => {
    const input = e.target.value;
    const result = Utils.ipHandler(input);
    setInvalidIp(result);
    setIpMessage("Invalid Ip-address");
    setIpAddress(input);
  };

  const handleRevert = () => {
    setShowConfirmUpdateModal(false);
    setNoChanges(true);
    setEdit(false);
    setInvalidIp(false);
    setInvalidName(false);
    setComputerName(selectedComputer.computerName);
    setIpAddress(selectedComputer.ipAddress);
  };

  const handleUpdateModal = () => {
    setUpdateValues({ name: computerName, ip: ipAddress });
    setShowConfirmUpdateModal(true);
  };

  const editClient = async () => {
    const url = API_URL + "/api/Computers/" + selectedComputer.computerID;
    const data = JSON.stringify({
      computerName: updateValues.name,
      ipAddress: updateValues.ip,
      osVersion: selectedComputer.osVersion,
      lastConnection: selectedComputer.lastConnection,
    });
    try {
      await axios.request({
        method: "put",
        maxBodyLength: Infinity,
        url: url,
        headers: {
          "Content-Type": "application/json",
        },
        data: data,
      });
      handleToast(true, "Successfully updated client info");
      setSelectedComputer((prevProps) => ({
        ...prevProps,
        computerName: updateValues.name,
        ipAddress: updateValues.ip,
      }));
      setEdit(false);
      handleRefresh();
    } catch (error) {
      Utils.handleAxiosError(error);
      handleToast(false, "Failed to update client info");
    }
  };

  const getVersion = async () => {
    try {
      const response = await axios.get(
        API_URL + "/api/Computers" + selectedComputer.computerID + "/version"
      );
      const version = response.data;
      if (version != selectedComputer.osVersion) {
        const url = API_URL + "/api/Computers/" + selectedComputer.computerID;
        const data = JSON.stringify({
          computerName: selectedComputer.computerName,
          ipAddress: selectedComputer.ipAddress,
          osVersion: version,
          lastConnection: selectedComputer.lastConnection,
        });
        try {
          await axios.request({
            method: "put",
            maxBodyLength: Infinity,
            url: url,
            headers: {
              "Content-Type": "application/json",
            },
            data: data,
          });
          setSelectedComputer((prevProps) => ({
            ...prevProps,
            osVersion: version,
          }));
          handleRefresh();
        } catch (error) {
          Utils.handleAxiosError(error);
          handleToast(false, "Failed to change OS-version");
        }
      }
    } catch (error) {
      Utils.handleAxiosError(error);
      handleToast(false, "Failed to find OS-version");
    }
  };

  const updateClient = async () => {
    try {
      await axios.get(
        API_URL + "/api/Computers" + selectedComputer.computerID + "/version"
      );
      handleToast(true, "Successfully updated client");
    } catch (error) {
      Utils.handleAxiosError(error);
      handleToast(false, "Failed to update client");
    }
  };

  useEffect(() => {
    dontUpdateSameValues();
  }, [computerName, dontUpdateSameValues, ipAddress]);

  return (
    <Card className="detailedCard">
      <CardHeader>
        <Row>
          <Col xs="auto" as={"h5"} className="mb-0">
            Client
          </Col>
          <Col className="text-end">
            <CloseButton onClick={() => hide()} />
          </Col>
        </Row>
      </CardHeader>
      <ListGroup>
        <ListGroupItem>
          <Form className="mb-2">
            <FormGroup className="mb-2">
              <FormLabel className="mb-1">
                <b>Name</b>
              </FormLabel>
              <FormControl
                type="text"
                placeholder="Name"
                value={computerName}
                onChange={nameHandler}
                isInvalid={invalidName}
                disabled={!edit}
                readOnly={!edit}
              />
              <Form.Control.Feedback type="invalid">
                {nameMessage}
              </Form.Control.Feedback>
            </FormGroup>
            <FormGroup>
              <FormLabel className="mb-1">
                <b>IP-address</b>
              </FormLabel>
              <FormControl
                type="text"
                placeholder="IP-address"
                value={ipAddress}
                onChange={ipHandler}
                isInvalid={invalidIp}
                disabled={!edit}
                readOnly={!edit}
              />
              <Form.Control.Feedback type="invalid">
                {ipMessage}
              </Form.Control.Feedback>
            </FormGroup>
          </Form>
          <Row className="g-2">
            <Col>
              <Button
                size="sm"
                variant={!edit ? "secondary" : "outline-secondary"}
                onClick={!edit ? setEdit : handleRevert}
                className="w-100"
              >
                {!edit ? "Edit Information" : "Back"}
              </Button>
            </Col>
            <Col>
              <Button
                size="sm"
                className="w-100"
                onClick={handleUpdateModal}
                disabled={invalidName || invalidIp || noChanges}
                hidden={!edit}
              >
                Change
              </Button>
            </Col>
          </Row>
        </ListGroupItem>
        <ListGroupItem>
          <Row className="text-center">
            <b>OS-version:</b>
            <span className="mb-3">
              {selectedComputer.osVersion ? (
                selectedComputer.osVersion
              ) : (
                <span className="text-danger">N/A</span>
              )}
            </span>
            <Col>
              <Button
                onClick={getVersion}
                className="me-3"
                disabled={edit}
                size="sm"
              >
                Get OS-version
              </Button>
              <Button onClick={updateClient} disabled={edit} size="sm">
                Update Client
              </Button>
            </Col>
          </Row>
        </ListGroupItem>
      </ListGroup>
      {showConfirmUpdateModal && (
        <ConfirmUpdateModal
          show={showConfirmUpdateModal}
          hide={() => {
            setShowConfirmUpdateModal(false);
          }}
          before={{
            name: selectedComputer.computerName,
            ip: selectedComputer.ipAddress,
          }}
          after={updateValues}
          handleRevert={handleRevert}
          updateClient={editClient}
        />
      )}
    </Card>
  );
};

export default DetailedCard;
