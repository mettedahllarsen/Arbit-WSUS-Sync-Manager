import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Card, Row, Col, Button, Spinner } from "react-bootstrap";

const TitleCard = (props) => {
  const {
    title,
    icon,
    handleRefresh,
    isLoading,
    updateTime,
    extraButton,
  } = props;

  return (
    <Card className="ps-3 pe-2 py-2">
      <Row className="align-items-center g-0">
        <Col
          as="h3"
          xs="auto"
          className="title m-0 me-3"
          data-testid="pageTitle"
        >
          <FontAwesomeIcon icon={icon} className="me-2" />
          {title}
        </Col>
        <Col xs="auto">
          <span className="bigText">
            <b>Last updated</b>:{" "}
            {updateTime
              ? updateTime
              : new Date().toLocaleString("en-GB", {
                  formatMatcher: "best fit",
                })}
          </span>
        </Col>
        {extraButton ? extraButton : ""}
        <Col
          xs={extraButton ? "auto" : ""}
          className={extraButton ? "" : "text-end"}
        >
          <Button data-testid="refreshBtn" onClick={() => handleRefresh()}>
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
