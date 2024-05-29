import { useEffect } from "react";
import { Modal, ModalHeader, ModalTitle, ModalBody } from "react-bootstrap";

const ConfigurationsModal = (props) => {
  const { show, hide } = props;

  useEffect(() => {
    console.log("Component ConfigurationModal mounted");
  }, []);
  return (
    <Modal show={show} onHide={() => hide()} className="modal-margin">
      <ModalHeader className="py-1" closeButton>
        <ModalTitle as={"h3"} className="title">
          Configurations
        </ModalTitle>
      </ModalHeader>
      <ModalBody>Content</ModalBody>
    </Modal>
  );
};

export default ConfigurationsModal;
