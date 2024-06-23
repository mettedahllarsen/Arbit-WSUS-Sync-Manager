import { useEffect, useState } from "react";
import { Row, Col } from "react-bootstrap";
import TitleCard from "../Cards/TitleCard";
import Utils from "../../Utils/Utils";

const Updates = () => {
  const [isLoading, setLoading] = useState(false);

  useEffect(() => {
    console.log("Updates mounted");
  }, []);

  const handleRefresh = () => {
    setLoading(true);
    Utils.simulateLoading().then(() => {
      setLoading(false);
    });
  };

  return (
    <>
      <Row className="g-2">
        <Col>
          <TitleCard
            title={"Updates"}
            icon={"file-arrow-down"}
            handleRefresh={handleRefresh}
            isLoading={isLoading}
          />
        </Col>
      </Row>
    </>
  );
};

export default Updates;
