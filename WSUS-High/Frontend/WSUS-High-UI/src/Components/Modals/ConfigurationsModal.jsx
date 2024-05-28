import { Modal } from "react-bootstrap";

const ConfigurationsModal = (props) => {
  const { show, hide } = props;
  return (
    <Modal show={show} onHide={() => hide()} className="modal-margin">
      Hej
    </Modal>
  );
};

export default ConfigurationsModal;
