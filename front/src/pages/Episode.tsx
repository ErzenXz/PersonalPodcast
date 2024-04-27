import "../scss/Episode.scss";
import { useParams } from "react-router-dom";
import Plyr from "plyr-react";
import "plyr-react/plyr.css";
import check_if_logged_in from "../services/is_logged";
import { useEffect, useState } from "react";
import type Episode from "../types/Episode";
import Navigation from "../components/Navigation";
import type Comment from "../types/Comment";
import {
   formatLengthToTime,
   formatedDate,
   checkIfValidTitle,
   checkIfValidAudioURL,
   checkIfValidDescription,
   checkIfValidImageURL,
   checkIfValidVideoURL,
   checkIfVideoOrAudioURL,
} from "../services/formatting_tools";

function checkIfEpisodeExists(episode: Episode): boolean {
   if (
      episode === null ||
      episode === undefined ||
      episode.id === null ||
      episode.id === undefined
   ) {
      return false;
   }
   return true;
}

async function getEpisode(id: string): Promise<Episode | null> {
   const headers = new Headers();
   headers.append("Content-Type", "application/json");

   const requestOptions = {
      method: "GET",
      headers: headers,
      redirect: "follow" as RequestRedirect,
      credentials: "include" as RequestCredentials,
   };

   return await fetch(`https://api.erzen.tk/episodes/${id}`, requestOptions)
      .then((response) => response.json())
      .then((result: Episode) => {
         return result;
      })
      .catch((error) => {
         console.error("Error:", error);
         return null; // Return null in case of an error
      });
}

// Function to get the comments of the episode

async function getComments(id: string, page: number = 1): Promise<Comment[] | null> {
   const headers = new Headers();
   headers.append("Content-Type", "application/json");

   const requestOptions = {
      method: "GET",
      headers: headers,
      redirect: "follow" as RequestRedirect,
      credentials: "include" as RequestCredentials,
   };

   return await fetch(`https://api.erzen.tk/comments/episodes/${id}?page=${page}`, requestOptions)
      .then((response) => response.json())
      .then((result: Comment[]) => {
         return result;
      })
      .catch((error) => {
         console.error("Error:", error);
         return null; // Return null in case of an error
      });
}

function Episode() {
   const params = useParams<{ episodeId: string }>();

   const [loggedIn, setLoggedIn] = useState(false);
   const [episode, setEpisode] = useState<Episode | null>(null);
   //const [playTime, setPlayTime] = useState(0);
   const [loading, setLoading] = useState(true);
   const [loadingComments, setLoadingComments] = useState(true);
   const [comments, setComments] = useState<Comment[]>([]);
   const [page, setPage] = useState(1);

   const [textarea, setTextarea] = useState("");

   const maxWords = 100;

   useEffect(() => {
      check_if_logged_in().then((value) => {
         setLoggedIn(value);
      });

      if (params.episodeId) {
         const episodeId: string = params.episodeId;
         getEpisode(episodeId).then((episode) => {
            setEpisode(episode);
            setLoading(false);
            getComments(episodeId, page).then((result) => {
               setComments(result || []);
               setLoadingComments(false);
            });
         });
      } else {
         setEpisode(null);
         setLoading(false);
      }
   }, [params.episodeId, page]);

   const handlePreviousPage = () => {
      if (page > 1) {
         setLoadingComments(true);
         setPage(page - 1);
      }
   };

   const handleNextPage = () => {
      if (comments.length < 10) {
         setLoadingComments(true);
         setPage(page + 1);
      }
   };

   const submitComment = async (e: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
      e.preventDefault();
      if (textarea === "") {
         return;
      }

      // Check if the comment has more than 100 words

      if (textarea.split(" ").length > maxWords) {
         alert("Your comment has more than 100 words");
         return;
      }

      const headers = new Headers();
      headers.append("Content-Type", "application/json");
      headers.append("Authorization", `Bearer ${localStorage.getItem("accessToken")}`);

      const requestOptions = {
         method: "POST",
         headers: headers,
         redirect: "follow" as RequestRedirect,
         credentials: "include" as RequestCredentials,
         body: JSON.stringify({
            message: textarea,
            episodeId: episode?.id,
            userId: JSON.parse(localStorage.getItem("user") || "{}").id,
            date: new Date().toISOString(),
         }),
      };

      fetch(`https://api.erzen.tk/comments`, requestOptions)
         .then((response) => response.json())
         .then((result: Comment) => {
            setComments([result, ...comments]);
            setTextarea("");
         })
         .catch((error) => {
            console.error("Error:", error);
         });
   };

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
            ) : episode && checkIfEpisodeExists(episode) ? (
               <div className="items-E">
                  <div className="item2-E">
                     <div className="episodes-E">
                        <div className="episode-E">
                           <div className="info-E">
                              <h3 className="title-E">{checkIfValidTitle(episode.title)}</h3>
                              <div className="more-E">
                                 <div className="date-E">
                                    <p>{formatedDate(episode.createdDate)}</p>
                                 </div>

                                 <div className="duration-E">
                                    <p>{formatLengthToTime(episode.length)}</p>
                                 </div>
                              </div>

                              <p className="description-E">
                                 {checkIfValidDescription(episode.description)}
                              </p>
                              {checkIfVideoOrAudioURL(
                                 episode.videoFileUrl,
                                 episode.audioFileUrl
                              ) ? (
                                 <div className="audio-E">
                                    <Plyr
                                       source={{
                                          type: "audio",
                                          title: checkIfValidTitle(episode.title),
                                          sources: [
                                             {
                                                src: checkIfValidAudioURL(episode.audioFileUrl),
                                                type: "audio/mp3",
                                             },
                                          ],
                                       }}
                                    />
                                 </div>
                              ) : checkIfValidVideoURL(episode.videoFileUrl) ? (
                                 <div className="video-E">
                                    <Plyr
                                       source={{
                                          type: "video",
                                          title: checkIfValidTitle(episode.title),
                                          sources: [
                                             {
                                                src: episode.videoFileUrl,
                                                type: "video/mp4",
                                                size: 720,
                                             },
                                          ],
                                       }}
                                    />
                                 </div>
                              ) : (
                                 <></>
                              )}
                           </div>

                           {checkIfVideoOrAudioURL(episode.videoFileUrl, episode.audioFileUrl) ? (
                              <div className="image-E">
                                 <img src={checkIfValidImageURL(episode.posterImg)} alt="Episode" />
                              </div>
                           ) : (
                              <></>
                           )}
                        </div>
                     </div>
                  </div>
               </div>
            ) : (
               <h1>Episode not found</h1>
            )}

            <div className="comments">
               {loggedIn ? (
                  <div className="create">
                     <form>
                        <textarea
                           onChange={(e) => {
                              setTextarea(e.target.value);
                           }}
                           value={textarea}
                           placeholder="Write your comment here"
                        ></textarea>
                        <button onClick={submitComment}>Submit</button>
                     </form>
                  </div>
               ) : (
                  <h2>Please log in to leave a comment</h2>
               )}
               {loggedIn && (
                  <div className="list">
                     {loadingComments ? (
                        <h2 className="center">Loading comments...</h2>
                     ) : Array.isArray(comments) && comments.length > 0 ? (
                        comments.map((comment) => (
                           <div className="comment" key={comment.id}>
                              <div className="info">
                                 <p className="message">{comment.message}</p>
                                 <div className="date">
                                    <p>{formatedDate(comment.date)}</p>
                                 </div>
                              </div>
                           </div>
                        ))
                     ) : (
                        <h2 className="center">No comments</h2>
                     )}
                  </div>
               )}

               {comments.length >= 10 && (
                  <div className="pagination">
                     <button onClick={handlePreviousPage}>Previous</button>
                     <button onClick={handleNextPage}>Next</button>
                  </div>
               )}
            </div>
         </div>
      </>
   );
}

export default Episode;
