import "../scss/Login.scss";
import { useState } from "react";

function doLogin(email: string, password: string): boolean {
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

   return true;
}

function doSignup(event: React.FormEvent<HTMLFormElement>) {
   event.preventDefault();
   // Add your signup logic here
}

function Login() {
   const [showLogin, setShowLogin] = useState(true);
   const [email, setEmail] = useState("");
   const [password, setPassword] = useState("");

   const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
      event.preventDefault();
      if (showLogin) {
         // Call the login function
         doLogin(email, password);
      } else {
         // Call the signup function
         doSignup(event);
      }
   };

   return (
      <div className="container">
         <div className={showLogin ? "login" : "signup"}>
            <h1>{showLogin ? "Login" : "Signup"}</h1>
            <form onSubmit={handleSubmit}>
               {!showLogin && <input type="text" placeholder="Username" required />}
               {!showLogin && <input type="text" placeholder="Full Name" required />}
               <input
                  type="email"
                  autoComplete="email"
                  placeholder="Email"
                  required
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
               />
               {!showLogin && <input type="date" placeholder="Birthday" />}
               <input
                  type="password"
                  placeholder="Password"
                  required
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
               />
               {!showLogin && <input type="password" placeholder="Confirm Password" required />}
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
