import { useParams } from "react-router-dom";
import Navigation from "../components/Navigation";

function Podcast() {
   const params = useParams<{ podcastId: string }>();

   if (!params.podcastId) {
      return <h1>Podcast</h1>;
   }

   const loggedInValue = Boolean(localStorage.getItem("token"));

   return (
      <>
         {loggedInValue == true ? (
            <Navigation title="Podcast App" loggedIn={true} />
         ) : (
            <Navigation title="Podcast App" />
         )}
         <h1>Podcast {params.podcastId}</h1>
      </>
   );
}

export default Podcast;
