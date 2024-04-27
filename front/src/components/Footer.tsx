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
                  <a href="https://www.facebook.com/cahani" target="_blank">
                     <span>Facebook</span>
                     <FontAwesomeIcon icon={faFacebook} />
                  </a>
                  <a href="https://twitter.com/MergimCahani" target="_blank">
                     <span>Twitter</span>
                     <FontAwesomeIcon icon={faTwitter} />
                  </a>
                  <a href="https://www.instagram.com/mergca/" target="_blank">
                     <span>Instagram</span>
                     <FontAwesomeIcon icon={faInstagram} />
                  </a>
               </div>

               <div className="contact">
                  <p>
                     <i className="fas fa-envelope"></i>
                     <span>
                        <a href="mailto:mcahani@gmail.com">mcahani@gmail.com</a>
                     </span>
                  </p>
                  <p>
                     <i className="fas fa-phone"></i>
                     <span>
                        <a href="tel:+38345649012">(+383) 45 649 012</a>
                     </span>
                  </p>
               </div>
            </div>

            <div className="copy">
               <p>&copy; 2024 Mergim Cahani Podcast</p>
            </div>
         </footer>
      </>
   );
}

export default Footer;
