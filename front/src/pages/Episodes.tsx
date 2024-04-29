import { useEffect, useState } from "react";
import type Episode from "../types/Episode";
import {
   checkIfValidTitle,
   checkIfValidDescription,
   formatedDate,
   checkIfValidImageURL,
   formatLengthToTime,
} from "../services/formatting_tools";
import check_if_logged_in from "../services/is_logged";
import Navigation from "../components/Navigation";
import "../scss/Episodes.scss";
import { useNavigate } from "react-router-dom";

async function getEpisodes(page: number): Promise<Episode[]> {
   const headers = new Headers();
   headers.append("Content-Type", "application/json");

   const requestOptions = {
      method: "GET",
      headers: headers,
      redirect: "follow" as RequestRedirect,
      credentials: "include" as RequestCredentials,
   };

   return await fetch(`https://api.erzen.tk/episodes?page=${page}`, requestOptions)
      .then((response) => response.json())
      .then((result: Episode[]) => {
         return result;
      })
      .catch((error) => {
         console.error("Error:", error);
         return []; // Return an empty array in case of an error
      });
}

// Function to search for episodes

async function searchEpisodes(search: string = "", page: number = 1): Promise<Episode[]> {
   const headers = new Headers();
   headers.append("Content-Type", "application/json");

   const requestOptions = {
      method: "GET",
      headers: headers,
      redirect: "follow" as RequestRedirect,
      credentials: "include" as RequestCredentials,
   };

   return await fetch(
      `https://api.erzen.tk/episodes/search?query=${search}&page=${page}`,
      requestOptions
   )
      .then((response) => response.json())
      .then((result: Episode[]) => {
         return result;
      })
      .catch((error) => {
         console.error("Error:", error);
         return []; // Return an empty array in case of an error
      });
}

function Episodes() {
   // Change the document title
   document.title = "Episodes - Mergim Cahani";

   const [episodes, setEpisodes] = useState<Episode[]>([]);
   const [episodesLoaded, setEpisodesLoaded] = useState(false);
   const [loggedIn, setLoggedIn] = useState(false);
   const [page, setPage] = useState(1);
   const [search, setSearch] = useState("");
   const [searchPage, setSearchPage] = useState(1);
   const [searching, setSearching] = useState(false);
   const [loading, setLoading] = useState(true);

   const navigate = useNavigate();

   useEffect(() => {
      check_if_logged_in().then((value) => {
         setLoggedIn(value);
      });
   }, []);

   useEffect(() => {
      if (!searching) {
         setLoading(true);
         getEpisodes(page).then((data) => {
            setEpisodes(data);
            setEpisodesLoaded(true);
            setLoading(false);
         });
      }
   }, [page, searching]);

   useEffect(() => {
      const performSearch = async () => {
         if (search.trim() !== "") {
            setLoading(true);
            const data = await searchEpisodes(search, searchPage);
            setEpisodes(data);
            setEpisodesLoaded(true);
            setLoading(false);
         }
      };

      if (searching) {
         performSearch();
      }
   }, [searching, searchPage]);

   useEffect(() => {
      if (search == "") {
         setSearching(false);
      }
   }, [search]);

   return (
      <>
         {loggedIn == true ? (
            <Navigation title="Mergim Canhasi" loggedIn={true} />
         ) : (
            <Navigation title="Mergim Canhasi" />
         )}

         <div className="main">
            <div className="episodes">
               {!loading && episodesLoaded && episodes.length > 0 ? (
                  <div className="top">
                     <h1>Episodes</h1>
                     <form
                        className="search-form"
                        onSubmit={(e) => {
                           e.preventDefault();
                           if (search.trim() !== "") {
                              setSearching(true);
                              setSearchPage(1);
                              searchEpisodes(search, searchPage).then((data) => {
                                 setEpisodes(data);
                                 setEpisodesLoaded(true);
                              });
                           } else {
                              setSearching(false);
                           }
                        }}
                     >
                        <input
                           type="search"
                           name="search"
                           value={search}
                           onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                              setSearch(event.target.value);
                           }}
                           placeholder="Search for episodes"
                        />
                     </form>
                  </div>
               ) : null}

               <div className="episodes-list">
                  {loading && <h1>Loading episodes...</h1>}
                  {!loading && Array.isArray(episodes) && episodes.length > 0
                     ? episodes.map((episode) => (
                          <div
                             key={episode.id}
                             className="episode"
                             onClick={() => navigate(`/episode/${episode.id}`)}
                          >
                             <img
                                src={checkIfValidImageURL(episode.posterImg)}
                                alt={episode.title}
                             />
                             <div className="episode-info">
                                <h2>{checkIfValidTitle(episode.title)}</h2>
                                <p>{checkIfValidDescription(episode.description)}</p>
                                <p>{formatedDate(episode.createdDate)}</p>
                                <p>{formatLengthToTime(episode.length)}</p>
                             </div>
                          </div>
                       ))
                     : !loading &&
                       episodesLoaded && (
                          <p>
                             {searching ? (
                                <h1 className="center">No results found.</h1>
                             ) : (
                                <h1 className="center">No more episodes found on page {page}.</h1>
                             )}
                          </p>
                       )}
               </div>
            </div>

            <div className="pagination">
               <button
                  onClick={() => (searching ? setSearchPage(searchPage - 1) : setPage(page - 1))}
                  disabled={searching ? searchPage === 1 : page === 1}
               >
                  Previous
               </button>
               <button
                  onClick={() => (searching ? setSearchPage(searchPage + 1) : setPage(page + 1))}
                  disabled={!episodes.length}
               >
                  Next
               </button>
            </div>
         </div>
      </>
   );
}

export default Episodes;
