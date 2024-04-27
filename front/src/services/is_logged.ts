import AuthResponse from "../types/AuthResponse";
import StatusResponse from "../types/StatusResponse";
import refreshToken from "./refresh_token";

const headers = new Headers();
headers.append("Content-Type", "application/json");

const requestOptions = {
   method: "GET",
   headers: headers,
   redirect: "follow" as RequestRedirect,
   credentials: "include" as RequestCredentials,
};

async function check_if_logged_in(): Promise<boolean> {
   const response = await fetch("https://api.erzen.tk/auth/info", requestOptions);
   const result: AuthResponse | StatusResponse = await response.json();

   if (result.code !== 40 && result.code !== 41) {
      localStorage.setItem("user", JSON.stringify(result));

      // Check if the token is expired
      const tokenExpireDate = localStorage.getItem("tokenExpireDate");
      const accessToken = localStorage.getItem("accessToken");
      // Convert the stored expiration date to a Date object and compare it with the current UTC date
      if (!tokenExpireDate || new Date() > new Date(tokenExpireDate) || !accessToken) {
         refreshToken();
         console.log("Token refreshed");
      }

      return true;
   } else {
      localStorage.removeItem("user");
      return false;
   }
}

export default check_if_logged_in;
