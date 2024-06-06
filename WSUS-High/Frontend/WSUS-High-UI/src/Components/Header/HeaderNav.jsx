import { useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Button } from "react-bootstrap";
import ConfigurationsModal from "./Modals/ConfigurationsModal";

const HeaderNav = () => {
  const [showConfigurations, setShowConfigurations] = useState(false);
  return (
    <>
      <Button
        size="lg"
        variant="danger"
        onClick={() => setShowConfigurations(true)}
        className="me-1"
      >
        <FontAwesomeIcon icon="gear" /> Configurations
      </Button>

      {showConfigurations && (
        <ConfigurationsModal
          show={showConfigurations}
          hide={() => setShowConfigurations(false)}
        />
      )}
    </>
  );
};

export default HeaderNav;
