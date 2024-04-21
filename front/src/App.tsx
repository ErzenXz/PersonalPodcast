//import { useState } from "react";


// import Login from "./components/Login";
import Homepage from "./pages/Homepage";
import Navigation from "./components/Navigation";

function App() {
   const loggedInValue = Boolean(localStorage.getItem("token"));


import Login from "./components/Login";


function App() {
   return (
      <>

         {loggedInValue == true ? (
            <Navigation title="Podcast App" loggedIn={true} />
         ) : (
            <Navigation title="Podcast App" />
         )}

         <Homepage />
=======
         <Login />

      </>
   );
}

export default App;
