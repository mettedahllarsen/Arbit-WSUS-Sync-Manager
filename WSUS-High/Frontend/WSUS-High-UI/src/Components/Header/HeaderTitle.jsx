const HeaderTitle = () => {
  return (
    <div className="text-start ms-4">
      <h5 className="text-dark">
        <strong>WSUS HIGH UI</strong>
      </h5>

      <span className="biggerText text-dark">
        <strong>Hostname: </strong>
      </span>

      <span className="biggerText text-secondary">
        <strong>{window.location.hostname}</strong>
      </span>
    </div>
  );
};

export default HeaderTitle;
