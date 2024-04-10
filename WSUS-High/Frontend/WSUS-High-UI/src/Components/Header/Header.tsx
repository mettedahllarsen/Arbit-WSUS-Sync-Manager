import { useEffect } from 'react';

const Header = () => {
    useEffect(() => {
        console.log("Component Header mounted");
    }, []);
    
    return (
        <>
        {/* Header Content */}
        </>
    );
}

export default Header;