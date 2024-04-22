import Navigation from "../components/Navigation";

function Podcasts() {
   document.title = "Podcasts - Lionel Messi Podcast";

   const profiles = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

   const profileList = profiles.map((profile) => {
      return (
         <div key={profile} className="profile">
            <img src="https://via.placeholder.com/150" alt="profile" />
            <h3>Profile Name</h3>
         </div>
      );
   });

   const loggedInValue = Boolean(localStorage.getItem("token"));

   return (
      <>
         {loggedInValue == true ? (
            <Navigation title="Podcast App" loggedIn={true} />
         ) : (
            <Navigation title="Podcast App" />
         )}

         {profileList}
      </>
   );
}

export default Podcasts;
