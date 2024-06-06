import { useEffect } from "react";
import {
  Modal,
  ModalHeader,
  ModalTitle,
  ModalBody,
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
      <ModalHeader>
        <ModalTitle className="w-100 text-center">
          Are you sure you want to delete <b>{computer.computerName}</b>?
        </ModalTitle>
      </ModalHeader>
      <ModalBody className="text-center">
        <Row>
          <Col>
            <Button
              onClick={() => {
                deleteComputer();
                hide();
              }}
            >
              Confirm
            </Button>
          </Col>
          <Col>
            <Button variant="danger" onClick={() => hide()}>
              Cancel
            </Button>
          </Col>
        </Row>
      </ModalBody>
    </Modal>
  );
};

export default ConfirmDeleteModal;
