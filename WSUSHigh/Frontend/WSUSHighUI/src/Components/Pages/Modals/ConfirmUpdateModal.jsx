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

const ConfirmUpdateModal = (props) => {
  const { show, hide, before, after, updateClient, handleRevert } = props;

  useEffect(() => {
    console.log("Component ConfirmUpdateModal mounted");
  }, []);

  return (
    <Modal show={show} onHide={() => hide()} className="modalBox">
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
              {after.name != before.name ? (
                <Col xs="12">
                  <Row>
                    <Col xs="3">
                      <b>Name:</b>
                    </Col>
                    <Col xs="auto">
                      {"'"}
                      <span className="text-danger">{before.name}</span>
                      {"'"} <b>{"--->"}</b> {"'"}
                      <span className="text-success">{after.name}</span>
                      {"'"}
                    </Col>
                  </Row>
                </Col>
              ) : null}
              {after.ip != before.ip ? (
                <Col xs="12">
                  <Row>
                    <Col xs="3">
                      <b>Ip-address:</b>
                    </Col>
                    <Col xs="auto">
                      {"'"}
                      <span className="text-danger">{before.ip}</span>
                      {"'"} <b>{"--->"}</b> {"'"}
                      <span className="text-success">{after.ip}</span>
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
                updateClient();
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
