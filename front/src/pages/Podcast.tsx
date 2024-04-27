import { useNavigate, useParams } from "react-router-dom";
import Navigation from "../components/Navigation";
import { useEffect, useState } from "react";
import type Podcast from "../types/Podcast";
import check_if_logged_in from "../services/is_logged";
import type Episode from "../types/Episode";
import PodcastCard from "../components/PodcastCard";
import "../scss/Podcast.scss";

async function getPodcast(id: string): Promise<Podcast | null> {
   const headers = new Headers();
   headers.append("Content-Type", "application/json");

   const requestOptions = {
      method: "GET",
      headers: headers,
      redirect: "follow" as RequestRedirect,
      credentials: "include" as RequestCredentials,
   };

   return await fetch(`https://api.erzen.tk/podcasts/${id}`, requestOptions)
      .then((response) => response.json())
      .then((result: Podcast) => {
         return result;
      })
      .catch((error) => {
         console.error("Error:", error);
         return null; // Return null in case of an error
      });
}

async function getAllEpisodesFromPodcast(id: string): Promise<Episode[]> {
   const headers = new Headers();
   headers.append("Content-Type", "application/json");

   const requestOptions = {
      method: "GET",
      headers: headers,
      redirect: "follow" as RequestRedirect,
      credentials: "include" as RequestCredentials,
   };

   return await fetch(`https://api.erzen.tk/podcasts/${id}/episodes`, requestOptions)
      .then((response) => response.json())
      .then((result: Episode[]) => {
         return result;
      })
      .catch((error) => {
         console.error("Error:", error);
         return []; // Return an empty array in case of an error
      });
}

const formatedDate = (date: string): string => {
   const d = new Date(date);
   return `${d.getDate()}/${d.getMonth() + 1}/${d.getFullYear()}`;
};

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

function checkIfValidAudioURL(url: string): string {
   if (url === null || url === "" || url === undefined || url == "string") {
      return "https://personal-podcast-life-2.s3.amazonaws.com/72d1dc39-139c-46b2-b39b-72fdfadd6596.mp3";
   }
   return url;
}

function checkIfValidTitle(title: string): string {
   if (title === null || title === "" || title === undefined || title == "string") {
      return "No title";
   }
   return title;
}

function checkIfValidDescription(description: string): string {
   if (
      description === null ||
      description === "" ||
      description === undefined ||
      description == "string"
   ) {
      return "No description";
   }
   return description;
}

// Function to add the play time of the episode to the database
async function addPlayTimeToDatabase(episodeId: number, playTime: number) {
   // Replace `API_ENDPOINT` with the actual endpoint URL
   await fetch("https://api.erzen.tk/analytics", {
      method: "POST",
      headers: {
         "Content-Type": "application/json",
         Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
      },
      body: JSON.stringify({
         episodeId: episodeId,
         userId: JSON.parse(localStorage.getItem("user") || "{}").id,
         firstPlay: new Date().toISOString(),
         lastPlay: new Date().toISOString(),
         length: playTime,
      }),
   })
      .then((response) => response.json())
      .then((data) => {
         console.log("Success:", data);
      })
      .catch((error) => {
         console.error("Error:", error);

         // If the access token is expired, refresh it and try again
         // refreshToken().then((value) => {
         //    if (value) {
         //       addPlayTimeToDatabase(episodeId, playTime);
         //    }
         // });
      });
}

addPlayTimeToDatabase(12, 1);

function Podcast() {
   const params = useParams<{ podcastId: string }>();
   const [loggedIn, setLoggedIn] = useState(false);
   const [podcast, setPodcast] = useState<Podcast | null>(null);
   const [episodes, setEpisodes] = useState<Episode[]>([]);
   const [loading, setLoading] = useState(true);
   const [playTime, setPlayTime] = useState(0);

   const navigate = useNavigate();

   useEffect(() => {
      const interval = setInterval(() => {
         if (playTime > 0) {
            addPlayTimeToDatabase(episodes[0].id, playTime);
         }
      }, 5000);
      return () => clearInterval(interval);
   }, [playTime, episodes]);

   useEffect(() => {
      check_if_logged_in().then((value) => {
         setLoggedIn(value);
      });

      if (params.podcastId) {
         const podcastId: string = params.podcastId;
         getPodcast(podcastId).then((value: Podcast | null) => {
            if (value) {
               setPodcast(value);
               getAllEpisodesFromPodcast(podcastId).then((data) => {
                  setEpisodes(data);
                  setLoading(false);
               });
            } else {
               setPodcast(null);
               setLoading(false);
            }
         });
      } else {
         setPodcast(null);
         setLoading(false);
      }
   }, [params.podcastId]);

   const goToEpisode = (id: number) => navigate("/episode/" + id);

   return (
      <>
         {loggedIn == true ? (
            <Navigation title="Mergim Canhasi" loggedIn={true} />
         ) : (
            <Navigation title="Mergim Canhasi" />
         )}

         <div className="main">
            {loading ? (
               <h1>Loading...</h1>
            ) : podcast ? (
               <>
                  <div className="items">
                     <div className="item1">
                        <PodcastCard podcast={podcast} />
                     </div>

                     <div className="item2">
                        <div className="episodes">
                           {episodes.map((episode) => (
                              <div
                                 key={episode.id}
                                 className="episode"
                                 onClick={() => goToEpisode(episode.id)}
                              >
                                 <div className="info">
                                    <h3>{checkIfValidTitle(episode.title)}</h3>
                                    <p className="description">
                                       {checkIfValidDescription(episode.description)}
                                    </p>
                                    <audio
                                       controls
                                       className="audio"
                                       onTimeUpdate={(e) => {
                                          const audioElement = e.target as HTMLAudioElement;
                                          setPlayTime(Math.floor(audioElement?.currentTime || 0));
                                       }}
                                    >
                                       <source
                                          src={checkIfValidAudioURL(episode.audioFileUrl)}
                                          type="audio/mpeg"
                                          id={"audio-" + episode.id}
                                       />
                                       Your browser does not support the audio element.
                                    </audio>
                                    <div className="more">
                                       <div className="date">
                                          <p>{formatedDate(episode.createdDate)}</p>
                                       </div>
                                       <div className="duration">
                                          <p>{formatLengthToTime(episode.length)}</p>
                                       </div>
                                    </div>
                                 </div>
                                 <div className="image">
                                    <img
                                       src={checkIfValidImageURL(episode.posterImg)}
                                       alt="episode"
                                    />
                                 </div>
                              </div>
                           ))}
                        </div>
                     </div>
                  </div>
               </>
            ) : (
               <h1>Podcast not found</h1>
            )}
         </div>
      </>
   );
}

export default Podcast;
