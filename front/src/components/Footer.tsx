import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faTwitter, faInstagram, faFacebook } from "@fortawesome/free-brands-svg-icons";

import "../scss/Footer.scss";

function Footer() {
   return (
      <>
         <footer className="footer">
            <div className="info">
               <div className="links">
                  <a href="#">Home</a>
                  <a href="#">About</a>
                  <a href="#">Contact</a>
               </div>

               <div className="social">
                  <a href="#">
                     <span>Facebook</span>
                     <FontAwesomeIcon icon={faFacebook} />
                  </a>
                  <a href="#">
                     <span>Twitter</span>
                     <FontAwesomeIcon icon={faTwitter} />
                  </a>
                  <a href="#">
                     <span>Instagram</span>
                     <FontAwesomeIcon icon={faInstagram} />
                  </a>
               </div>

               <div className="contact">
                  <p>
                     <i className="fas fa-envelope"></i>
                     <span>
                        <a href="mailto:erzen@erzen.tk">erzen@erzen.tk</a>
                     </span>
                  </p>
                  <p>
                     <i className="fas fa-phone"></i>
                     <span>
                        <a href="tel:+38344123456">(+383) 44 123 456</a>
                     </span>
                  </p>
               </div>
            </div>

            <div className="copy">
               <p>&copy; 2024 Lionel Messi Podcast</p>
            </div>
         </footer>
      </>
   );
}

export default Footer;
