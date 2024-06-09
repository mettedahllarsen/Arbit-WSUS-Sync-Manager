import { useCallback, useEffect, useState } from "react";
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
import Utils from "../../Utils/Utils";
import ConfirmUpdateModal from "../Pages/Modals/ConfirmUpdateModal";

const DetailedCard = (props) => {
  const { hide, computer, handleRefresh, deleteClient } = props;

  const [computerName, setComputerName] = useState(computer.computerName);
  const [ipAddress, setIpAddress] = useState(computer.ipAddress);
  const [osVersion, setOsVersion] = useState(computer.osVersion);

  const [invalidName, setInvalidName] = useState(false);
  const [nameMessage, setNameMessage] = useState("");

  const [invalidIp, setInvalidIp] = useState(false);
  const [ipMessage, setIpMessage] = useState("");

  const [invalidOs, setInvalidOs] = useState(false);
  const [osMessage, setOsMessage] = useState("");

  const [edit, setEdit] = useState(false);

  const [updateValues, setUpdateValues] = useState({
    name: null,
    ip: null,
    os: null,
  });
  const [showConirmUpdateModal, setShowConfirmUpdateModal] = useState(false);

  const [noChanges, setNoChanges] = useState(true);

  const dontUpdateSameValues = useCallback(() => {
    if (
      computer.computerName == computerName &&
      computer.ipAddress == ipAddress &&
      computer.osVersion == osVersion
    ) {
      setNoChanges(true);
    } else {
      setNoChanges(false);
    }
  }, [
    computer.computerName,
    computer.ipAddress,
    computer.osVersion,
    computerName,
    ipAddress,
    osVersion,
  ]);

  useEffect(() => {
    dontUpdateSameValues();
  }, [computerName, dontUpdateSameValues, ipAddress, osVersion]);

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

  const osHandler = (e) => {
    const input = e.target.value;
    const result = Utils.osHandler();
    setInvalidOs(result);
    setOsMessage("Invalid OS-version");
    setOsVersion(input);
  };

  const handleRevert = () => {
    setShowConfirmUpdateModal(false);
    setNoChanges(true);
    setEdit(false);
    setComputerName(computer.computerName);
    setIpAddress(computer.ipAddress);
    setOsVersion(computer.osVersion);
  };

  const handleUpdateModal = () => {
    setUpdateValues({ name: computerName, ip: ipAddress, os: osVersion });
    setShowConfirmUpdateModal(true);
  };

  return (
    <Card>
      <CardHeader className="mb-2">
        <Row>
          <Col as={"h5"} className="title mb-0">
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
                  <FormLabel>Ip-address</FormLabel>
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
                    {ipMessage}
                  </Form.Control.Feedback>
                </FormGroup>
              </Col>
              <Col>
                <FormGroup>
                  <FormLabel>OS-version</FormLabel>
                  <FormControl
                    type="text"
                    placeholder="OS-version"
                    defaultValue={osVersion}
                    onChange={osHandler}
                    isInvalid={invalidOs}
                    disabled={!edit}
                    readOnly={!edit}
                  />
                  <Form.Control.Feedback type="invalid">
                    {osMessage}
                  </Form.Control.Feedback>
                </FormGroup>
              </Col>
            </Row>
          </Form>
          <Row>
            <Col xs="6">
              <Button
                variant="secondary"
                className="w-100"
                onClick={handleRevert}
                disabled={!edit}
                hidden={!edit}
              >
                {noChanges && noChanges ? "Go back" : "Undo changes"}
              </Button>
            </Col>
            <Col xs="6">
              <Button
                variant="primary"
                className="w-100"
                onClick={handleUpdateModal}
                disabled={!edit || invalidName || invalidIp || noChanges}
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
      {showConirmUpdateModal && (
        <ConfirmUpdateModal
          show={showConirmUpdateModal}
          hide={() => {
            setShowConfirmUpdateModal(false);
          }}
          computer={computer}
          handleRevert={handleRevert}
          handleRefresh={handleRefresh}
          updates={updateValues}
        />
      )}
    </Card>
  );
};

export default DetailedCard;
