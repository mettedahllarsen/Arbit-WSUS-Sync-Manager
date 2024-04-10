import { useEffect } from 'react'

const App = () => {

  useEffect(() => {
    console.log("App mounted")
  }, []);

  return (
    <>
      {/* Header */}

      {/* Pages */}
      <p>This is WSUS-High</p>
    </>
  )
}

export default App;