import "../scss/Homepage.scss";
import Messi from "../assets/messi.webp";
import { Link } from "react-router-dom";
import Footer from "../components/Footer";

function Homepage() {
   // Change the document title
   document.title = "Homepage - Lionel Messi Podcast";

   return (
      <>
         <div className="homepage">
            <div className="item1">
               <h1>Welcome to Lionel Messi Podcast!</h1>
               <p>Listen to your favorite podcasts</p>
            </div>
            <div className="item2">
               <img src={Messi} alt="podcast" />
            </div>
         </div>

         <div className="categories">
            <div className="category">
               <h2>Category 1</h2>
               <p>Category 1 description</p>
            </div>
            <div className="category">
               <h2>Category 2</h2>
               <p>Category 2 description</p>
            </div>
            <div className="category">
               <h2>Category 3</h2>
               <p>Category 3 description</p>
            </div>
            <div className="category">
               <h2>Category 4</h2>
               <p>Category 4 description</p>
            </div>
         </div>

         <div className="recommendetEpisodes">
            <h2>Latest Episodes</h2>
            <div className="episode">
               <div className="info">
                  <h3 className="title">Episode 1</h3>
                  <p className="description">Episode 1 description</p>
                  <div className="more">
                     <div className="rating">
                        <p>Rating: 5/5</p>
                     </div>
                     <div className="date">
                        <p>10/10/2021</p>
                     </div>
                     <div className="duration">
                        <p>30 minutes</p>
                     </div>
                     <div className="podcast">
                        <p>Soccer</p>
                     </div>
                  </div>
               </div>
               <div className="image">
                  <img src={Messi} alt="episode" />
               </div>
            </div>
            <div className="episode">
               <div className="info">
                  <h3 className="title">Episode 1</h3>
                  <p className="description">Episode 1 description</p>
                  <div className="more">
                     <div className="rating">
                        <p>Rating: 5/5</p>
                     </div>
                     <div className="date">
                        <p>10/10/2021</p>
                     </div>
                     <div className="duration">
                        <p>30 minutes</p>
                     </div>
                     <div className="podcast">
                        <p>Soccer</p>
                     </div>
                  </div>
               </div>
               <div className="image">
                  <img src={Messi} alt="episode" />
               </div>
            </div>
            <div className="episode">
               <div className="info">
                  <h3 className="title">Episode 1</h3>
                  <p className="description">Episode 1 description</p>
                  <div className="more">
                     <div className="rating">
                        <p>Rating: 5/5</p>
                     </div>
                     <div className="date">
                        <p>10/10/2021</p>
                     </div>
                     <div className="duration">
                        <p>30 minutes</p>
                     </div>
                     <div className="podcast">
                        <p>Soccer</p>
                     </div>
                  </div>
               </div>
               <div className="image">
                  <img src={Messi} alt="episode" />
               </div>
            </div>
            <div className="episode">
               <div className="info">
                  <h3 className="title">Episode 1</h3>
                  <p className="description">Episode 1 description</p>
                  <div className="more">
                     <div className="rating">
                        <p>Rating: 5/5</p>
                     </div>
                     <div className="date">
                        <p>10/10/2021</p>
                     </div>
                     <div className="duration">
                        <p>30 minutes</p>
                     </div>
                     <div className="podcast">
                        <p>Soccer</p>
                     </div>
                  </div>
               </div>
               <div className="image">
                  <img src={Messi} alt="episode" />
               </div>
            </div>
            <div className="episode">
               <div className="info">
                  <h3 className="title">Episode 1</h3>
                  <p className="description">Episode 1 description</p>
                  <div className="more">
                     <div className="rating">
                        <p>Rating: 5/5</p>
                     </div>
                     <div className="date">
                        <p>10/10/2021</p>
                     </div>
                     <div className="duration">
                        <p>30 minutes</p>
                     </div>
                     <div className="podcast">
                        <p>Soccer</p>
                     </div>
                  </div>
               </div>
               <div className="image">
                  <img src={Messi} alt="episode" />
               </div>
            </div>
            <div className="episode">
               <div className="info">
                  <h3 className="title">Episode 1</h3>
                  <p className="description">Episode 1 description</p>
                  <div className="more">
                     <div className="rating">
                        <p>Rating: 5/5</p>
                     </div>
                     <div className="date">
                        <p>10/10/2021</p>
                     </div>
                     <div className="duration">
                        <p>30 minutes</p>
                     </div>
                     <div className="podcast">
                        <p>Soccer</p>
                     </div>
                  </div>
               </div>
               <div className="image">
                  <img src={Messi} alt="episode" />
               </div>
            </div>
            <div className="episode">
               <div className="info">
                  <h3 className="title">Episode 1</h3>
                  <p className="description">Episode 1 description</p>
                  <div className="more">
                     <div className="rating">
                        <p>Rating: 5/5</p>
                     </div>
                     <div className="date">
                        <p>10/10/2021</p>
                     </div>
                     <div className="duration">
                        <p>30 minutes</p>
                     </div>
                     <div className="podcast">
                        <p>Soccer</p>
                     </div>
                  </div>
               </div>
               <div className="image">
                  <img src={Messi} alt="episode" />
               </div>
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
