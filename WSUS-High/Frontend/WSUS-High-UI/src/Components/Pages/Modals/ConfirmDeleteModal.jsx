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

const ConfirmDeleteModal = (props) => {
  const { show, hide, computer, handleRefresh } = props;

  const deleteComputer = async () => {
    const url = API_URL + "/api/computers/" + computer.computerID;

    try {
      const response = await axios.request({
        method: "delete",
        url: url,
      });

      handleRefresh();
      console.log(response.data);
    } catch (error) {
      Utils.handleAxiosError(error);
    }
  };

  useEffect(() => {
    console.log("Component ConfirmDeleteModal mounted");
  }, []);

  return (
    <Modal show={show} onHide={() => hide()} className="modal-margin">
      <ModalHeader className="p-2" closeButton>
        Remove Client: #{computer.computerID}
      </ModalHeader>
      <ModalBody>
        <h5 className="m-0">
          <b>Are you sure you want to remove this client?</b>
        </h5>
        <b className="text-danger">This process cannot be undone!</b>
      </ModalBody>
      <ModalFooter className="p-1 pt-0">
        <Row className="justify-content-center g-2">
          <Col xs="auto">
            <Button variant="outline-secondary" onClick={() => hide()}>
              Cancel
            </Button>
          </Col>
          <Col xs="auto">
            <Button
              variant="danger"
              onClick={() => {
                deleteComputer();
                hide();
              }}
            >
              Delete
            </Button>
          </Col>
        </Row>
      </ModalFooter>
    </Modal>
  );
};

export default ConfirmDeleteModal;
