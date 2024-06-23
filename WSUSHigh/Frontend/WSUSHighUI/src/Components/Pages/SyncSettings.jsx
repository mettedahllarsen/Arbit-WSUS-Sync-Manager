import { useEffect, useState } from "react";
import { Row, Col } from "react-bootstrap";
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
    <>
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
    </>
  );
};

export default SyncSettings;
