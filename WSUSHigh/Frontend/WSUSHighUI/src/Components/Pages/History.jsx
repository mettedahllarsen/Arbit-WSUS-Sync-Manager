import { useEffect, useState } from "react";
import { Row, Col } from "react-bootstrap";
import TitleCard from "../Cards/TitleCard";
import Utils from "../../Utils/Utils";

const History = () => {
  const [isLoading, setLoading] = useState(false);

  useEffect(() => {
    console.log("Activity mounted");
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
            title={"History"}
            icon={"clock-rotate-left"}
            handleRefresh={handleRefresh}
            isLoading={isLoading}
          />
        </Col>
      </Row>
    </>
  );
};

export default History;
