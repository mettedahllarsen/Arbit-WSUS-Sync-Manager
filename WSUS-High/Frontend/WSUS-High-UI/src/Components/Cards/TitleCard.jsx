import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Card, Row, Col, Button, Spinner } from "react-bootstrap";

// TODO: Solution for extra buttons
const TitleCard = (props) => {
  const { title, icon, handleRefresh, isLoading } = props;

  return (
    <Card className="px-3 py-2">
      <Row className="align-items-center">
        <Col as="h3" xs="auto" className="title m-0" data-testid="pageTitle">
          <FontAwesomeIcon icon={icon} className="me-2" />
          {title}
        </Col>
        <Col xs="auto">
          <span>
            <b>Last updated</b>:{" "}
            {new Date().toLocaleString("en-GB", {
              formatMatcher: "best fit",
            })}
          </span>
        </Col>
        <Col className="text-end">
          <Button
            data-testid="refreshBtn"
            variant="primary"
            onClick={() => handleRefresh()}
          >
            {isLoading ? (
              <Spinner animation="border" role="status" size="sm" />
            ) : (
              <FontAwesomeIcon data-testid="refreshIcon" icon="rotate" />
            )}
          </Button>
        </Col>
      </Row>
    </Card>
  );
};

export default TitleCard;
