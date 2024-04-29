import { useEffect, useState } from "react";
import check_if_logged_in from "../services/is_logged";
import Navigation from "../components/Navigation";
import Modal from "../components/Modal"; // A new component for the popup
import "../scss/Account.scss";

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
                  <form>
                     <input type="password" placeholder="Old Password" required />
                     <input type="password" placeholder="New Password" required />
                     <input type="password" placeholder="Confirm New Password" required />
                     <button type="submit">Update</button>
                  </form>
               </Modal>
            )}
         </div>
      </>
   );
}

export default Account;
