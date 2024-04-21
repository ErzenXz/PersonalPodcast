import { useNavigate } from "react-router-dom";
import "../scss/Login.scss";
import { useState } from "react";

interface Register {
   email: string;
   password: string;
   username: string;
   fullName: string;
   birthday: string;
   confirmPassword: string;
}

let goHomePage = () => {};

function doLogin(email: string, password: string) {
   console.log("doLogin called");
   console.log("email: " + email);
   console.log("password: " + password);

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
   };

   fetch("http://localhost:5009/auth/login", requestOptions)
      .then((response) => response.json())
      .then((result) => console.log(result))
      .catch((error) => console.log("error", error));

   localStorage.setItem("token", "true");

   goHomePage();
   return true;
}

function doSignup(user: Register) {
   // Add your signup logic here
   console.log(user.email);
   console.log(user.password);
   console.log(user.username);
   console.log(user.fullName);
   console.log(user.birthday);
   console.log(user.confirmPassword);

   localStorage.setItem("token", "true");
   goHomePage();
}

function setBackgroundImageFromList() {
   const images = [
      "https://images.unsplash.com/photo-1545431613-51ec097943c6?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
      "https://images.unsplash.com/photo-1523821741446-edb2b68bb7a0?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
      "https://images.unsplash.com/photo-1521133573892-e44906baee46?q=80&w=1974&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
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
   const navigate = useNavigate();
   goHomePage = () => navigate("/");

   const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
      event.preventDefault();
      if (showLogin) {
         // Call the login function
         doLogin(email, password);
      } else {
         const user: Register = {
            email,
            password,
            username,
            fullName,
            birthday,
            confirmPassword,
         };

         doSignup(user);
      }
   };

   return (
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
   );
}

export default Login;
