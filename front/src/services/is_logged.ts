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

      // Check if the access token is expired
      const tokenExpireDate = localStorage.getItem("tokenExpireDate");
      // Check if the today's date is greater than the expiration date
      if (tokenExpireDate && new Date().getTime() > new Date(tokenExpireDate).getTime()) {
         refreshToken();
      }

      return true;
   } else {
      localStorage.removeItem("user");
      return false;
   }
}

export default check_if_logged_in;
