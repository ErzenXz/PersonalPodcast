import Homepage from "./pages/Homepage";
import Navigation from "./components/Navigation";
import { useEffect, useState } from "react";
import check_if_logged_in from "./services/is_logged";
import "./index.css";

function App() {
   const [loggedIn, setLoggedIn] = useState(false);

   useEffect(() => {
      check_if_logged_in().then((value) => {
         setLoggedIn(value);
      });
   }, []);

   return (
      <>
         {loggedIn == true ? (
            <Navigation title="Mergim Canhasi" loggedIn={true} />
         ) : (
            <Navigation title="Mergim Canhasi" />
         )}
         <Homepage />
      </>
   );
}

export default App;
