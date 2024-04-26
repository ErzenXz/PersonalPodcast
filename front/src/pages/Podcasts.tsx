import "../scss/Podcasts.scss";
import Navigation from "../components/Navigation";
import Podcast from "../types/Podcast";
import { useEffect, useState } from "react";
import check_if_logged_in from "../services/is_logged";
import PodcastCard from "../components/PodcastCard";
import Footer from "../components/Footer";

async function getPodcasts(page: number): Promise<Podcast[]> {
   const headers = new Headers();
   headers.append("Content-Type", "application/json");

   const requestOptions = {
      method: "GET",
      headers: headers,
      redirect: "follow" as RequestRedirect,
      credentials: "include" as RequestCredentials,
   };

   return await fetch(`https://api.erzen.tk/podcasts?page=${page}`, requestOptions)
      .then((response) => response.json())
      .then((result: Podcast[]) => {
         return result;
      })
      .catch((error) => {
         console.error("Error:", error);
         return []; // Return an empty array in case of an error
      });
}

function Podcasts() {
   document.title = "Podcasts - Mergim Canhasi";

   const [loggedIn, setLoggedIn] = useState(false);
   const [podcasts, setPodcasts] = useState<Podcast[]>([]);
   const [page, setPage] = useState(1);
   const [loading, setLoading] = useState(true);

   useEffect(() => {
      check_if_logged_in().then((value) => {
         setLoggedIn(value);
      });
      getPodcasts(page).then((value) => {
         if (value && value.length > 0) {
            setPodcasts(value);
            setLoading(false);
         } else {
            setPodcasts([]);
            setLoading(false);
         }
      });
   }, [page]);

   return (
      <>
         {loggedIn == true ? (
            <Navigation title="Mergim Canhasi" loggedIn={true} />
         ) : (
            <Navigation title="Mergim Canhasi" />
         )}

         <div className="main">
            <h1>Podcasts</h1>
            <div className="podcasts">
               {loading ? (
                  <>
                     <PodcastCard loading />
                     <PodcastCard loading />
                     <PodcastCard loading />
                  </>
               ) : podcasts.length > 0 ? (
                  podcasts.map((podcast) => <PodcastCard key={podcast.id} podcast={podcast} />)
               ) : (
                  <p>No podcasts found.</p>
               )}
            </div>

            <div className="pagination">
               <button
                  onClick={() => {
                     if (page > 1) {
                        setPage(page - 1);
                        setLoading(true);
                     }
                  }}
                  disabled={page == 1}
               >
                  Previous
               </button>
               <button
                  onClick={() => {
                     if (podcasts.length > 0) {
                        setPage(page + 1);
                        setLoading(true);
                     }
                  }}
               >
                  Next
               </button>
            </div>
         </div>

         <Footer />
      </>
   );
}

export default Podcasts;
