import { useEffect } from "react";
import {
  Modal,
  ModalHeader,
  ModalBody,
  ModalFooter,
  Button,
  Row,
  Col,
} from "react-bootstrap";
import axios from "axios";
import Utils from "../../../Utils/Utils";
import { API_URL } from "../../../Utils/Settings";

const ConfirmUpdateModal = (props) => {
  const { show, hide, computer, handleRefresh, handleRevert, updates } = props;

  const updateComputer = async () => {
    const url = API_URL + "/api/computers/" + computer.computerID;
    const data = JSON.stringify({
      ComputerID: computer.computerID,
      ComputerName: updates.name,
      IPAddress: updates.ip,
      OsVersion: updates.os,
      LastConnection: new Date(),
    });

    try {
      /*const response = */ await axios.request({
        method: "put",
        maxBodyLength: Infinity,
        url: url,
        headers: {
          "Content-Type": "application/json",
        },
        data: data,
      });

      handleRefresh();
    } catch (error) {
      Utils.handleAxiosError(error);
    }
  };

  useEffect(() => {
    console.log("Component ConfirmUpdateModal mounted");
  }, []);

  return (
    <Modal show={show} onHide={() => hide()} className="modal-margin">
      <ModalHeader className="p-2" closeButton>
        Update Client
      </ModalHeader>
      <ModalBody>
        <Row className="g-4">
          <Col xs="12" className="text-center">
            <Row className="g-0">
              <Col xs="12" as="h5" className="m-0">
                <b>Are you sure?</b>
              </Col>
              <Col xs="12" className="text-danger">
                <b>This process cannot be undone!</b>
              </Col>
            </Row>
          </Col>
          <Col xs="12">
            <Row className="g-2">
              <Col xs="12">The following changes were made:</Col>
              {updates.name != computer.computerName ? (
                <Col xs="12">
                  <Row>
                    <Col xs="3">
                      <b>Name:</b>
                    </Col>
                    <Col xs="auto">
                      {"'"}
                      <span className="text-danger">
                        {computer.computerName}
                      </span>
                      {"'"} <b>{"--->"}</b> {"'"}
                      <span className="text-success">{updates.name}</span>
                      {"'"}
                    </Col>
                  </Row>
                </Col>
              ) : null}
              {updates.ip != computer.ipAddress ? (
                <Col xs="12">
                  <Row>
                    <Col xs="3">
                      <b>Ip-address:</b>
                    </Col>
                    <Col xs="auto">
                      {"'"}
                      <span className="text-danger">{computer.ipAddress}</span>
                      {"'"} <b>{"--->"}</b> {"'"}
                      <span className="text-success">{updates.ip}</span>
                      {"'"}
                    </Col>
                  </Row>
                </Col>
              ) : null}
              {updates.os != computer.osVersion ? (
                <Col xs="12">
                  <Row>
                    <Col xs="3">
                      <b>OS-version:</b>
                    </Col>
                    <Col xs="auto">
                      {"'"}
                      <span className="text-danger">{computer.osVersion}</span>
                      {"'"} <b>{"--->"}</b> {"'"}
                      <span className="text-success">{updates.os}</span>
                      {"'"}
                    </Col>
                  </Row>
                </Col>
              ) : null}
            </Row>
          </Col>
        </Row>
      </ModalBody>
      <ModalFooter className="p-1 pt-0">
        <Row className="justify-content-center g-2">
          <Col xs="auto">
            <Button variant="outline-secondary" onClick={() => handleRevert()}>
              Cancel
            </Button>
          </Col>
          <Col xs="auto">
            <Button
              onClick={() => {
                updateComputer();
                hide();
              }}
            >
              Update
            </Button>
          </Col>
        </Row>
      </ModalFooter>
    </Modal>
  );
};

export default ConfirmUpdateModal;
