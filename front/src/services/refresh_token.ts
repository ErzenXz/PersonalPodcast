import RefreshToken from "../types/RefreshToken";
import StatusResponse from "../types/StatusResponse";

async function refreshToken(): Promise<boolean> {
   const headers = new Headers();
   headers.append("Content-Type", "application/json");

   const requestOptions = {
      method: "POST",
      headers: headers,
      redirect: "follow" as RequestRedirect,
      credentials: "include" as RequestCredentials,
   };

   const response = await fetch("https://api.erzen.tk/auth/refresh", requestOptions);
   const result: RefreshToken | StatusResponse = await response.json();

   if (result.code !== 40 && result.code !== 41) {
      if (typeof result === "object" && "accessToken" in result) {
         const expireIn15Mins = new Date(new Date().getTime() + 15 * 60 * 1000);
         localStorage.setItem("accessToken", result.accessToken);
         localStorage.setItem("tokenExpireDate", expireIn15Mins.getTime().toString());
         return true;
      } else {
         localStorage.removeItem("user");
         return false;
      }
   }

   return false;
}

export default refreshToken;
