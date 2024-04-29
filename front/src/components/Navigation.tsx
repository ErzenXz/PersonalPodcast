import { Link, useNavigate } from "react-router-dom";
import "../scss/Navigation.scss";
import Logo from "../assets/logo.png";
import { useState } from "react";

interface NavigationProps {
   title?: string;
   loggedIn?: boolean;
}

function Navigation({ loggedIn }: NavigationProps) {
   const [isOpen, setIsOpen] = useState(false);
   const navigator = useNavigate();

   return (
      <nav className="nav">
         <div
            className="nav-h"
            onClick={() => {
               navigator("/");
            }}
         >
            <img src={Logo} alt="logo" />
         </div>
         <div className="hamburger" onClick={() => setIsOpen(!isOpen)}>
            <div></div>
            <div></div>
            <div></div>
         </div>
         <div className="sNav">
            <div className={isOpen ? "nav-ul open" : "nav-ul"}>
               <li className="nav-ul-li">
                  <Link to="/">Home</Link>
               </li>
               <li className="nav-ul-li">
                  <Link to="/podcasts">Podcasts</Link>
               </li>
               <li className="nav-ul-li">
                  <Link to="/episodes">Episodes</Link>
               </li>
               <li className="nav-ul-li">
                  <Link to="/about">About</Link>
               </li>
               {loggedIn ? (
                  <li className="nav-ul-li">
                     <Link to="/account">Account</Link>
                  </li>
               ) : (
                  <li className="nav-ul-li">
                     <Link to="/login">Login</Link>
                  </li>
               )}
            </div>
         </div>
      </nav>
   );
}

export default Navigation;
