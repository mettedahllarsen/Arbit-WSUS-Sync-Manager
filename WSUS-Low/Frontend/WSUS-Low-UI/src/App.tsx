import { useEffect } from 'react'
import Header from './Components/Header/Header';

const App = () => {

  useEffect(() => {
    console.log("App mounted")
  }, []);

  return (
    <>
      <Header />

      {/* Pages */}
      <p>This is WSUS-Low</p>
    </>
  )
}

export default App;
