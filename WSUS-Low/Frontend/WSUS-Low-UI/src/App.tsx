import { useEffect } from 'react'

const App = () => {

  useEffect(() => {
    console.log("App mounted")
  }, []);

  return (
    <>
      {/* Header */}

      {/* Pages */}
      <p>This is WSUS-Low</p>
    </>
  )
}

export default App;
