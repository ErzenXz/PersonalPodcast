import { useEffect, useState } from "react";
import check_if_logged_in from "../services/is_logged";
import Navigation from "../components/Navigation";
import "../scss/About.scss";

function About() {
   // Change the document title
   document.title = "About - Mergim Cahani";

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

         <div className="about">
            <h1>About</h1>
            <p>
               Hello there! My name is Mergim Cahani and I am a software developer. I have been
               working with web technologies for over 10 years now. I have worked with a variety of
               technologies, including HTML, CSS, JavaScript, PHP, and MySQL. I have also worked
               with a variety of frameworks and libraries, including React, Angular, and Vue.js. I
               am currently working as a full-stack developer at a software company in Prishtina,
               Kosovo.
            </p>
         </div>
      </>
   );
}

export default About;
