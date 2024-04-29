import refreshToken from "./refresh_token";

async function authenticatorPulse(): Promise<void> {
   // Get the token expiration date and the access token from the local storage

   const tokenExpireDate = localStorage.getItem("tokenExpireDate");
   const accessToken = localStorage.getItem("accessToken");

   // Check if the token is expired
   // Convert the stored expiration date to a Date object and compare it with the current UTC date
   if (!tokenExpireDate || new Date() > new Date(tokenExpireDate) || !accessToken) {
      refreshToken();
      console.log("Token refreshed");
   }
}

export default authenticatorPulse;
