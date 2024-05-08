import PropTypes from "prop-types";
import { Modal } from "react-bootstrap";

const ConfigurationsModal = (props) => {
  return (
    <Modal show={props.show} onHide={() => props.hide()}>
      Hej
    </Modal>
  );
};

ConfigurationsModal.propTypes = {
  show: PropTypes.bool.isRequired,
  hide: PropTypes.func.isRequired,
};

export default ConfigurationsModal;
