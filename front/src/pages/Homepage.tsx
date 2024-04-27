import "../scss/Homepage.scss";
import Mergim from "../assets/mergim.png";
import { Link } from "react-router-dom";
import Footer from "../components/Footer";
import { useEffect, useState } from "react";
//import type Category from "../types/Category";
import type Episode from "../types/Episode";

// async function getCategories(): Promise<Category[]> {
//    const headers = new Headers();
//    headers.append("Content-Type", "application/json");

//    const requestOptions = {
//       method: "GET",
//       headers: headers,
//       redirect: "follow" as RequestRedirect,
//       credentials: "include" as RequestCredentials,
//    };

//    return await fetch("https://api.erzen.tk/categories", requestOptions)
//       .then((response) => response.json())
//       .then((result: Category[]) => {
//          return result;
//       })
//       .catch((error) => {
//          console.error("Error:", error);
//          return []; // Return an empty array in case of an error
//       });
// }

async function getEpisodes(): Promise<Episode[]> {
   const headers = new Headers();
   headers.append("Content-Type", "application/json");

   const requestOptions = {
      method: "GET",
      headers: headers,
      redirect: "follow" as RequestRedirect,
      credentials: "include" as RequestCredentials,
   };

   return await fetch("https://api.erzen.tk/episodes?page=1", requestOptions)
      .then((response) => response.json())
      .then((result: Episode[]) => {
         return result;
      })
      .catch((error) => {
         console.error("Error:", error);
         return []; // Return an empty array in case of an error
      });
}

function formatDate(date: string): string {
   const options = { year: "numeric", month: "long", day: "numeric" };
   return new Date(date).toLocaleDateString("en-US", options as Intl.DateTimeFormatOptions);
}

function formatLengthToTime(length: number): string {
   const minutes = Math.floor(length / 60);
   const seconds = length % 60;
   return `${minutes}:${seconds}`;
}

function checkIfValidImageURL(url: string): string {
   if (url === null || url === "" || url === undefined || url == "string") {
      return "https://via.placeholder.com/150";
   }
   return url;
}

function Homepage() {
   // Change the document title
   document.title = "Home - Mergim Cahani";

   //const [categories, setCategories] = useState<Category[]>([]);
   const [episodes, setEpisodes] = useState<Episode[]>([]);

   useEffect(() => {
      //getCategories().then(setCategories);
      getEpisodes().then((data) => {
         setEpisodes(data);
      });
   }, []);

   return (
      <>
         <div className="homepage">
            <div className="item1">
               <h1>Welcome to Mergim Cahani Podcast!</h1>
               <p>Listen to your favorite podcasts</p>
            </div>
            <div className="item2">
               <img src={Mergim} alt="podcast" />
            </div>
         </div>

         <div className="recommendetEpisodes">
            <h2>Latest Episodes</h2>

            <div className="episodes">
               {episodes.map((episode) => (
                  <div key={episode.id} className="episode">
                     <div className="info">
                        <h3>{episode.title}</h3>
                        <p className="description">{episode.description}</p>
                        <div className="more">
                           <div className="rating">
                              <p>Rating: 5/5</p>
                           </div>
                           <div className="date">
                              <p>{formatDate(episode.createdDate)}</p>
                           </div>
                           <div className="duration">
                              <p>{formatLengthToTime(episode.length)}</p>
                           </div>
                           <div className="podcast">
                              <Link to="/podcast/1" />
                           </div>
                        </div>
                     </div>
                     <div className="image">
                        <img src={checkIfValidImageURL(episode.posterImg)} alt="episode" />
                     </div>
                  </div>
               ))}
            </div>

            <div className="viewMoreButton">
               <Link to="/episodes">View More</Link>
            </div>
         </div>

         <Footer />
      </>
   );
}

export default Homepage;
