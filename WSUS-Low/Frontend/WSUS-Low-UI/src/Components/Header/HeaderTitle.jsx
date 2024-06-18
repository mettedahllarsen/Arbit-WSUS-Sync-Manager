import { useEffect } from "react";

const HeaderTitle = () => {
  useEffect(() => {
    console.log("Component HeaderTitle mounted");
  }, []);

  return (
    <div className="text-start ms-4">
      <h5 className="text-dark">
        <strong>WSUS LOW UI</strong>
      </h5>

      <span className="text-dark biggerText">
        <strong>Hostname: </strong>
      </span>

      <span className="text-secondary biggerText">
        <strong>{window.location.hostname}</strong>
      </span>
    </div>
  );
};

export default HeaderTitle;
