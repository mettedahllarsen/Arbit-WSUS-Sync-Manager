import { useEffect, useState } from "react";
// import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Container, Row, Col } from "react-bootstrap";
import TitleCard from "../Cards/TitleCard";
import Utils from "../../Utils/Utils";

const SyncSettings = () => {
  const [isLoading, setLoading] = useState(false);

  useEffect(() => {
    console.log("SyncSettings mounted");
  }, []);

  const handleRefresh = () => {
    setLoading(true);
    Utils.simulateLoading().then(() => {
      setLoading(false);
    });
  };

  return (
    <Container fluid>
      <Row>
        <Col>
          <TitleCard
            title={"Syncronization Settings"}
            icon={"rotate"}
            handleRefresh={handleRefresh}
            isLoading={isLoading}
          />
        </Col>
      </Row>
    </Container>
  );
};

export default SyncSettings;
