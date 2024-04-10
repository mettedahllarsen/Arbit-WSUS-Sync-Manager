import { useEffect } from 'react';

const HeaderTitle = () => {
    useEffect(() => {
    console.log("Component HeaderTitle mounted");
  }, []);

    return (
        <div className='text-start ms-4'>
            <h5>
                <strong>WSUS-LOW</strong>
            </h5>

            <span>
                <strong>Hostname:</strong>
            </span>

            <span className='text-secondary'>
                <strong>{window.location.hostname}</strong>
            </span>
        </div>
    );
};

export default HeaderTitle;