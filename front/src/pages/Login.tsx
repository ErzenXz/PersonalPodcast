import { useNavigate } from "react-router-dom";
import "../scss/Login.scss";
import { useEffect, useState } from "react";
import Popup from "../components/Popup";

interface Register {
   email: string;
   password: string;
   username: string;
   fullName: string;
   birthday: string;
   confirmPassword: string;
}

let goHomePage = () => {};

async function doLogin(email: string, password: string): Promise<string> {
   const myHeaders = new Headers();
   myHeaders.append("Content-Type", "application/json");

   const raw = JSON.stringify({
      email,
      password,
   });

   const requestOptions = {
      method: "POST",
      headers: myHeaders,
      body: raw,
      redirect: "follow" as RequestRedirect,
      credentials: "include" as RequestCredentials,
   };

   let messageToReturn = "";

   await fetch("https://api.erzen.tk/auth/login", requestOptions)
      .then((response) => response.json())
      .then((result) => {
         const expireIn15Mins = new Date(Date.now() + 15 * 60000);

         switch (result.code) {
            case 38:
               messageToReturn = "You have successfully logged in!";

               localStorage.setItem("accessToken", result.accessToken);
               localStorage.setItem("tokenExpireDate", expireIn15Mins.toISOString());
               goHomePage();
               break;
            case 37:
               messageToReturn =
                  "The password you provided is incorrect. Please check the password and try again.";
               break;
            case 3:
               messageToReturn = "Email and password are required.";
               break;
            case 4:
               messageToReturn = "Email must be between 5 and 100 characters.";
               break;
            case 2:
               messageToReturn = "Vald password is required.";
               break;
            case 44:
               messageToReturn =
                  "You have been temporarily blocked from our system due to multiple failed login attempts. Please try again in 10 minutes.";
               break;
            case 36:
               messageToReturn =
                  "The email you provided is not registered in our system. Please check the email and try again.";
               break;
            default:
               messageToReturn = "Something went wrong";
         }
      })
      .catch((error) => console.log("error", error));
   return messageToReturn;
}

async function doSignup(user: Register): Promise<string> {
   // Validate the user input

   if (
      !user.email ||
      !user.password ||
      !user.username ||
      !user.fullName ||
      !user.birthday ||
      !user.confirmPassword
   ) {
      return "All fields are required.";
   }

   if (user.email.length < 5 || user.email.length > 100) {
      return "Email must be between 5 and 100 characters.";
   }

   if (user.password.length < 8 || user.password.length > 100) {
      return "Password must be between 8 and 100 characters.";
   }

   if (user.username.length < 3 || user.username.length > 20) {
      return "Username must be at least 3 characters.";
   }

   if (user.fullName.length < 3 || user.fullName.length > 100) {
      return "Full name must be between 3 and 100 characters.";
   }

   if (!user.birthday) {
      return "Birthday is required.";
   }

   if (user.password !== user.confirmPassword) {
      return "Passwords do not match.";
   }

   if (user.birthday > new Date().toISOString().split("T")[0]) {
      return "Birthday cannot be in the future.";
   }

   const myHeaders = new Headers();
   myHeaders.append("Content-Type", "application/json");

   const raw = JSON.stringify({
      username: user.username,
      fullName: user.fullName,
      email: user.email,
      password: user.password,
      birthdate: user.birthday,
   });

   const requestOptions = {
      method: "POST",
      headers: myHeaders,
      body: raw,
      redirect: "follow" as RequestRedirect,
      credentials: "include" as RequestCredentials,
   };

   let messageToReturn = "";

   await fetch("https://api.erzen.tk/auth/register", requestOptions)
      .then((response) => response.json())
      .then((result) => {
         switch (result.code) {
            case 43:
               messageToReturn = "You have successfully signed up!";
               break;
            case 3:
               messageToReturn = "All fields are required.";
               break;
            case 4:
               messageToReturn = "Email must be between 5 and 100 characters.";
               break;
            case 5:
               messageToReturn = "Password must be between 8 and 100 characters.";
               break;
            case 6:
               messageToReturn = "Birthday is required.";
               break;
            case 7:
               messageToReturn = "Email already in use.";
               break;
            case 8:
               messageToReturn = "Username already in use.";
               break;
            case 44:
               messageToReturn =
                  "You have been temporarily blocked by our automated system due to multiple failed login attempts. Please try again in 10 minutes.";
               break;
            default:
               messageToReturn = "Something went wrong";
         }
      })
      .catch((error) => console.log("error", error));
   return messageToReturn;
}

function setBackgroundImageFromList() {
   const images = [
      "https://images.unsplash.com/photo-1545431613-51ec097943c6?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
      "https://images.unsplash.com/photo-1523821741446-edb2b68bb7a0?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
      "https://images.unsplash.com/photo-1521133573892-e44906baee46?q=80&w=1974&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
      "https://images.unsplash.com/photo-1529641484336-ef35148bab06?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
      "https://images.unsplash.com/photo-1559251606-c623743a6d76?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
      "https://images.unsplash.com/photo-1558470598-a5dda9640f68?q=80&w=2071&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
      "https://images.unsplash.com/photo-1447703693928-9cd89c8d3ac5?q=80&w=2071&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
      "https://images.unsplash.com/photo-1463130456064-77fda7f96d6b?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
      "https://images.unsplash.com/photo-1505778489066-159c5f4a6c0f?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
      "https://images.unsplash.com/photo-1669295384050-a1d4357bd1d7?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
   ];
   const randomIndex = Math.floor(Math.random() * images.length);
   return `url(${images[randomIndex]})`;
}

const backGroundImg = setBackgroundImageFromList();

function Login() {
   document.title = "Login - Lionel Messi Podcast";

   const [showLogin, setShowLogin] = useState(true);
   const [email, setEmail] = useState("");
   const [password, setPassword] = useState("");
   const [username, setUsername] = useState("");
   const [fullName, setFullName] = useState("");
   const [birthday, setBirthday] = useState("");
   const [confirmPassword, setConfirmPassword] = useState("");
   const [popupMessage, setPopupMessage] = useState("");

   const [update, setUpdate] = useState(false);

   const navigate = useNavigate();
   goHomePage = () => {
      setTimeout(() => {
         navigate("/");
      }, 500);
   };

   // Fetch the API to see if the user is logged in
   const myHeaders = new Headers();
   myHeaders.append("Content-Type", "application/json");

   const requestOptions = {
      method: "GET",
      headers: myHeaders,
      redirect: "follow" as RequestRedirect,
      credentials: "include" as RequestCredentials,
   };

   useEffect(() => {
      fetch("https://api.erzen.tk/auth/info", requestOptions)
         .then((response) => response.json())
         .then((result) => {
            if (result.code === 8) {
               goHomePage();
            } else {
               console.log("User is not logged in.");
            }
         })
         .catch((error) => console.log("error", error));
   }, []);

   const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
      setUpdate(!update);
      event.preventDefault();
      let response: string;
      if (showLogin) {
         setPopupMessage("Logging in...");
         // Call the login function
         response = await doLogin(email, password);
      } else {
         setPopupMessage("Signing up...");
         const user: Register = {
            email,
            password,
            username,
            fullName,
            birthday,
            confirmPassword,
         };

         response = await doSignup(user);

         if (response === "You have successfully signed up!") {
            response = await doLogin(email, password);
         }
      }
      setPopupMessage(response);
   };

   return (
      <>
         <div className="container loginC" style={{ backgroundImage: backGroundImg }}>
            <div className={showLogin ? "login" : "signup"}>
               <h1>{showLogin ? "Login" : "Signup"}</h1>
               <form onSubmit={handleSubmit}>
                  {!showLogin && (
                     <input
                        type="text"
                        placeholder="Username"
                        required
                        value={username}
                        onChange={(e) => {
                           setUsername(e.target.value);
                        }}
                     />
                  )}
                  {!showLogin && (
                     <input
                        type="text"
                        placeholder="Full Name"
                        required
                        value={fullName}
                        onChange={(e) => {
                           setFullName(e.target.value);
                        }}
                     />
                  )}
                  <input
                     type="email"
                     autoComplete="email"
                     placeholder="Email"
                     required
                     value={email}
                     onChange={(e) => setEmail(e.target.value)}
                  />
                  {!showLogin && (
                     <input
                        type="date"
                        placeholder="Birthday"
                        value={birthday}
                        onChange={(e) => {
                           setBirthday(e.target.value);
                        }}
                     />
                  )}
                  <input
                     type="password"
                     placeholder="Password"
                     required
                     value={password}
                     onChange={(e) => setPassword(e.target.value)}
                  />
                  {!showLogin && (
                     <input
                        type="password"
                        placeholder="Confirm Password"
                        required
                        value={confirmPassword}
                        onChange={(e) => {
                           setConfirmPassword(e.target.value);
                        }}
                     />
                  )}
                  <button type="submit">{showLogin ? "Login" : "Signup"}</button>
               </form>
               <a
                  href="#"
                  onClick={(e) => {
                     e.preventDefault();
                     setShowLogin(!showLogin);
                  }}
               >
                  {showLogin ? "Create a new account!" : "Already have an account?"}
               </a>
            </div>
         </div>

         {popupMessage && (
            <Popup message={popupMessage} duration={5000} delay={0} update={update} />
         )}
      </>
   );
}

export default Login;
