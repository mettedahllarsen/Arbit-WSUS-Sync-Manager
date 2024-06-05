import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Link } from "react-router-dom";

const SideBarButton = (props) => {
  const { title, icon, testId, isActive, onClick } = props;

  const handleRoute = () => {
    if (title == "Overview") {
      return "/";
    } else {
      const route = title.replaceAll(" ", "");
      return "/" + route;
    }
  };

  return (
    <Link
      to={handleRoute()}
      className={`btn nav-button ${isActive ? "active" : ""}`}
      data-testid={testId}
      onClick={onClick}
    >
      <FontAwesomeIcon icon={icon} className="nav-icon me-4" />
      {title}
    </Link>
  );
};

export default SideBarButton;
