import { useEffect, useState } from "react";
import axios from "axios";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  Modal,
  ModalHeader,
  ModalTitle,
  ModalBody,
  Form,
  FormGroup,
  FormLabel,
  FormControl,
  Button,
} from "react-bootstrap";
import { API_URL } from "../../../Utils/Settings";
import Utils from "../../../Utils/Utils";

const AddClientModal = (props) => {
  const { show, hide, handleRefresh } = props;
  const [invalidName, setInvalidName] = useState(false);
  const [nameMessage, setNameMessage] = useState("");

  const [invalidIp, setInvalidIp] = useState(false);

  const [computerName, setComputerName] = useState(null);
  const [ipAddress, setIpAddress] = useState(null);
  const [osVersion, setOsVersion] = useState(null);

  useEffect(() => {
    console.log("Component AddClientModal mounted");
  }, []);

  const nameHandler = (e) => {
    const input = e.target.value;
    const result = Utils.nameHandler(input);
    if (result.invalid == true) {
      setInvalidName(true);
      setNameMessage(result.message);
    } else {
      setInvalidName(false);
      setComputerName(input);
    }
  };

  const ipHandler = (e) => {
    const input = e.target.value;
    if (Utils.ipHandler(input)) {
      setInvalidIp(true);
    } else {
      setInvalidIp(false);
      setIpAddress(input);
    }
  };

  const osHandler = (e) => {
    setOsVersion(e.target.value);
  };

  const addClient = async () => {
    const url = API_URL + "/api/Computers";
    const data = JSON.stringify({
      ComputerName: computerName,
      IPAddress: ipAddress,
      OsVersion: osVersion,
      LastConnection: new Date(),
    });

    try {
      const response = await axios.request({
        method: "post",
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
    <Modal show={show} onHide={() => hide()} className="modal-margin">
      <ModalHeader closeButton className="py-1">
        <ModalTitle className="title">
          <FontAwesomeIcon icon="plus" /> Add New Client
        </ModalTitle>
      </ModalHeader>

      <ModalBody>
        <Form className="mb-4">
          <FormGroup className="mb-3">
            <FormLabel>Name</FormLabel>
            <FormControl
              type="text"
              placeholder="Name"
              onChange={nameHandler}
              isInvalid={invalidName}
            />
            <Form.Control.Feedback type="invalid">
              {nameMessage}
            </Form.Control.Feedback>
          </FormGroup>
          <FormGroup className="mb-3">
            <FormLabel>IP Address</FormLabel>
            <FormControl
              type="text"
              placeholder="Ip-Address"
              onChange={ipHandler}
              isInvalid={invalidIp}
            />
            <Form.Control.Feedback type="invalid">
              Invalid Ip-address
            </Form.Control.Feedback>
          </FormGroup>
          <FormGroup>
            <FormLabel>OS Version</FormLabel>
            <FormControl
              type="text"
              placeholder="OS Version"
              onChange={osHandler}
            />
          </FormGroup>
        </Form>
        <div className="text-center">
          <Button
            onClick={() => {
              addClient();
              hide();
            }}
          >
            Add Client
          </Button>
        </div>
      </ModalBody>
    </Modal>
  );
};

export default AddClientModal;
