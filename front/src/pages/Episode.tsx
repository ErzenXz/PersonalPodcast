import "../scss/Episode.scss";
import { useParams } from "react-router-dom";
import check_if_logged_in from "../services/is_logged";
import { useEffect, useState } from "react";
import type Episode from "../types/Episode";
import Navigation from "../components/Navigation";
import type Comment from "../types/Comment";

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
      return "https://images.unsplash.com/photo-1529641484336-ef35148bab06?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D";
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

function checkIfValidVideoURL(url: string): boolean {
   if (url === null || url === "" || url === undefined || url == "string") {
      return false;
   }
   return true;
}

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
   const [playTime, setPlayTime] = useState(0);
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
                              <div className="audio-E">
                                 <audio controls>
                                    <source
                                       src={checkIfValidAudioURL(episode.audioFileUrl)}
                                       type="audio/mpeg"
                                    />
                                    Your browser does not support the audio element.
                                 </audio>
                              </div>

                              {checkIfValidVideoURL(episode.videoFileUrl) ? (
                                 <div className="video-E">
                                    <video controls>
                                       <source src={episode.videoFileUrl} type="video/mp4" />
                                       Your browser does not support the video element.
                                    </video>
                                 </div>
                              ) : (
                                 <></>
                              )}
                           </div>

                           <div className="image-E">
                              <img src={checkIfValidImageURL(episode.posterImg)} alt="Episode" />
                           </div>
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
