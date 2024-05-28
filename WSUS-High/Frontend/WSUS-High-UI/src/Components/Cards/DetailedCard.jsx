import { useState } from "react";
// import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  Card,
  CardHeader,
  CardBody,
  CloseButton,
  Row,
  Col,
  Form,
  FormGroup,
  FormLabel,
  FormControl,
  Button,
} from "react-bootstrap";
import { API_URL } from "../../Utils/Settings";
import Utils from "../../Utils/Utils";
import axios from "axios";

const DetailedCard = (props) => {
  const { hide, computer, handleRefresh, deleteClient } = props;

  const [computerName, setComputerName] = useState(computer.computerName);
  const [ipAddress, setIpAddress] = useState(computer.ipAddress);
  const [osVersion, setOsVersion] = useState(computer.osVersion);

  const [invalidName, setInvalidName] = useState(false);
  const [nameMessage, setNameMessage] = useState("");

  const [invalidIp, setInvalidIp] = useState(false);

  const [edit, setEdit] = useState(false);

  const nameHandler = (e) => {
    const name = e.target.value;
    if (name.length > 15) {
      setNameMessage("Cannot be longer than 15 characters");
      setInvalidName(true);
    } else if (/[\\/:*?"<>|]/.test(name)) {
      setNameMessage('Cannot contain these characters: \\ / : * ? " < > |');
      setInvalidName(true);
    } else if (name.startsWith(".")) {
      setNameMessage("Cannot start with ' . '");
      setInvalidName(true);
    } else {
      setComputerName(name);
      setInvalidName(false);
    }
  };

  const ipHandler = (e) => {
    const ip = e.target.value;
    const ipPattern = /^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/;
    if (!ipPattern.test(ip)) {
      setInvalidIp(true);
    } else {
      setIpAddress(ip);
      setInvalidIp(false);
    }
  };

  const osHandler = (e) => {
    setOsVersion(e.target.value);
  };

  const handleRevert = () => {
    setEdit(false);
    setComputerName(computer.computerName);
    setIpAddress(computer.ipAddress);
    setOsVersion(computer.osVersion);
  };

  const updateClient = async () => {
    const url = API_URL + "/api/computers/" + computer.ComputerID;
    const data = JSON.stringify({
      ComputerName: computerName,
      IPAddress: ipAddress,
      OsVersion: osVersion,
      LastConnection: new Date(),
    });

    try {
      const response = await axios.request({
        method: "put",
        maxBodyLength: Infinity,
        url: url,
        headers: {
          "Content-Type": "application/json",
        },
        data: data,
      });

      handleRefresh();
      console.log(response.data);
    } catch (error) {
      Utils.handleAxiosError(error);
    }
  };

  return (
    <Card>
      <CardHeader className="mb-2">
        <Row>
          <Col as={"h3"} className="title mb-0">
            Actions
          </Col>
          <Col className="text-end">
            <CloseButton onClick={() => hide()} />
          </Col>
        </Row>
      </CardHeader>
      {computer && (
        <CardBody>
          <Form className="mb-4">
            <Row>
              <Col>
                <FormGroup className="mb-3">
                  <FormLabel>Name</FormLabel>
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
              </Col>
              <Col>
                <FormGroup className="mb-3">
                  <FormLabel>IP Address</FormLabel>
                  <FormControl
                    type="text"
                    placeholder="Ip-Address"
                    defaultValue={ipAddress}
                    onChange={ipHandler}
                    isInvalid={invalidIp}
                    disabled={!edit}
                    readOnly={!edit}
                  />
                  <Form.Control.Feedback type="invalid">
                    Invalid Ip-address
                  </Form.Control.Feedback>
                </FormGroup>
              </Col>
              <Col>
                <FormGroup>
                  <FormLabel>OS Version</FormLabel>
                  <FormControl
                    type="text"
                    placeholder="OS Version"
                    defaultValue={osVersion}
                    onChange={osHandler}
                    disabled={!edit}
                    readOnly={!edit}
                  />
                </FormGroup>
              </Col>
            </Row>
          </Form>
          <Row className="mb-4">
            <Col xs="6">
              <Button
                variant="primary"
                className="w-100"
                onClick={handleRevert}
                disabled={!edit}
                hidden={!edit}
              >
                Undo Changes
              </Button>
            </Col>
            <Col xs="6">
              <Button
                variant="primary"
                className="w-100"
                onClick={updateClient}
                disabled={!edit}
                hidden={!edit}
              >
                Update
              </Button>
            </Col>
            <Col xs="6">
              <Button
                variant="primary"
                className="w-100"
                onClick={setEdit}
                hidden={edit}
              >
                Edit Client
              </Button>
            </Col>
            <Col xs="6">
              <Button
                variant="danger"
                className="w-100"
                onClick={deleteClient}
                hidden={edit}
              >
                Delete Client
              </Button>
            </Col>
          </Row>
        </CardBody>
      )}
    </Card>
  );
};

export default DetailedCard;
