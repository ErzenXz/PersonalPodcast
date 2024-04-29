import { useEffect, useState } from "react";
import check_if_logged_in from "../services/is_logged";
import Navigation from "../components/Navigation";
import Modal from "../components/Modal"; // A new component for the popup
import "../scss/Account.scss";

// Function to update the password
async function updatePassword(oldPassword: string, newPassword: string) {
   const headers = new Headers();
   headers.append("Content-Type", "application/json");

   const user = JSON.parse(localStorage.getItem("user")!);
   const email = user.email;

   const requestOptions = {
      method: "POST",
      headers: headers,
      redirect: "follow" as RequestRedirect,
      credentials: "include" as RequestCredentials,
      body: JSON.stringify({ email, oldPassword, newPassword }),
   };

   return await fetch(`https://api.erzen.tk/auth/change-password`, requestOptions)
      .then((response) => response.json())
      .then((result) => {
         return result;
      })
      .catch((error) => {
         console.error("Error:", error);
         return { error: true }; // Return an empty array in case of an error
      });
}

// Function to log out
async function logOUT() {
   const headers = new Headers();
   headers.append("Content-Type", "application/json");

   const requestOptions = {
      method: "GET",
      headers: headers,
      redirect: "follow" as RequestRedirect,
      credentials: "include" as RequestCredentials,
   };

   return await fetch(`https://api.erzen.tk/auth/logout`, requestOptions)
      .then((response) => response.json())
      .then((result) => {
         if (result.message === "User logged out successfully!") {
            localStorage.removeItem("user");
            window.location.href = "/login";
         }
      })
      .catch((error) => {
         console.error("Error:", error);
      });
}

function Account() {
   // Change the document title
   document.title = "Account - Mergim Cahani";

   const [loggedIn, setLoggedIn] = useState(false);
   const [showModal, setShowModal] = useState(false);
   const [user, setUser] = useState({
      fullName: "",
      username: "",
      email: "",
      role: "",
      id: 0,
   });

   const [oldPassword, setOldPassword] = useState("");
   const [newPassword, setNewPassword] = useState("");

   useEffect(() => {
      const user2 = JSON.parse(localStorage.getItem("user")!);
      check_if_logged_in().then((value) => {
         setLoggedIn(value);
         if (!value) {
            window.location.href = "/login";
         } else {
            // Update the user state
            setUser(user2);
         }
      });
   }, []);

   // Function to handle the 'Update Password' button click
   const handleUpdatePassword = () => {
      setShowModal(true);
   };

   return (
      <>
         <Navigation title={user.fullName} loggedIn={loggedIn} />

         <div className="main_content">
            {loggedIn && (
               <div className="user-details">
                  <h2>{user.fullName}</h2>
                  <p>Email: {user.email}</p>
                  <p>Role: {user.role}</p>
                  {/* Add more user details here */}
                  <button onClick={handleUpdatePassword}>Update Password</button>
                  {/* Add other buttons for updating user details */}
               </div>
            )}
            {showModal && (
               <Modal onClose={() => setShowModal(false)}>
                  <h2>Update Password</h2>
                  <input
                     type="password"
                     placeholder="Old Password"
                     value={oldPassword}
                     onChange={(e) => setOldPassword(e.target.value)}
                     required
                  />
                  <input
                     type="password"
                     placeholder="Old Password"
                     value={newPassword}
                     onChange={(e) => setNewPassword(e.target.value)}
                     required
                  />
                  <button
                     onClick={() => {
                        updatePassword(oldPassword, newPassword).then((data) => {
                           if (data.error) {
                              alert("An error occurred while updating the password.");
                           } else if (
                              data.Message ===
                                 "Email, old password and new password are required." ||
                              data.Message === "Email must be between 5 and 100 characters." ||
                              data.Message ===
                                 "Old password must be between 8 and 100 characters." ||
                              data.Message ===
                                 "New password must be between 8 and 100 characters." ||
                              data.Message === "User not found." ||
                              data.Message === "Invalid password."
                           ) {
                              alert(data.Message);
                           } else {
                              alert("Password updated successfully.");
                           }
                        });
                     }}
                  >
                     Update Password
                  </button>
               </Modal>
            )}

            {/* Log out button */}
            <button
               onClick={() => {
                  logOUT();
               }}
            >
               Log Out
            </button>
         </div>
      </>
   );
}

export default Account;
