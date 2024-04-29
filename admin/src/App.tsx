import { Admin, Resource, fetchUtils, houseLightTheme, houseDarkTheme } from "react-admin";

const lightTheme = houseLightTheme;
const darkTheme = houseDarkTheme;

import restProvider from "ra-data-simple-rest";
import PodcastList from "./components/PodcastList";
import PodcastCreate from "./components/PodcastCreate";
import EpisodeList from "./components/EpisodeList";
import EpisodeCreate from "./components/EpisodeCreate";
import EpisodeEdit from "./components/EpisodeEdit";
import ListCategory from "./components/ListCategory";
import CreateCategory from "./components/CreateCategory";
import EditCategory from "./components/CategoryEdit";
import ListUsers from "./components/UsersList";
import ListComments from "./components/ListComments";

// Function to get the accessToken from the localStorage

const getAccessToken = () => localStorage.getItem("accessToken");

import { Options } from "ra-core";
import PodcastEdit from "./components/PodcastEdit";
import ShowEpisode from "./components/ShowEpisode";
import authenticatorPulse from "./service/authenticatorPulse";
import UsersEdit from "./components/UsersEdit";
import CommentCreate from "./CommentCreate";
import CommentEdit from "./components/CommentEdit";

const httpClient = (url: string, options: Options = { headers: new Headers() }) => {
   if (!options.headers) {
      options.headers = new Headers({ Accept: "application/json" });
   }
   const token = getAccessToken();
   (options.headers as Headers).set("Authorization", `Bearer ${token}`);
   return fetchUtils.fetchJson(url, options);
};

const authProvider = {
   login: async ({ username, password }: { username: string; password: string }) => {
      const request = new Request("https://api.erzen.tk/auth/login", {
         method: "POST",
         redirect: "follow" as RequestRedirect,
         credentials: "include" as RequestCredentials,
         body: JSON.stringify({ email: username, password }),
         headers: new Headers({ "Content-Type": "application/json" }),
      });
      try {
         const response = await fetch(request);
         if (response.status < 200 || response.status >= 300) {
            throw new Error(response.statusText);
         }
         const { accessToken } = await response.json();
         localStorage.setItem("accessToken", accessToken);
         const expireIn15Mins = new Date(Date.now() + 15 * 60000);
         localStorage.setItem("tokenExpireDate", expireIn15Mins.toISOString());
      } catch (error) {
         throw new Error("Login failed");
      }
   },
   logout: async () => {
      const request = new Request("https://api.erzen.tk/auth/logout", {
         method: "POST",
         redirect: "follow" as RequestRedirect,
         credentials: "include" as RequestCredentials,
         headers: new Headers({
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("accessToken")}`, // Include the token
         }),
      });
      try {
         await fetch(request);
         localStorage.removeItem("accessToken"); // Remove the token after logging out
      } catch (error) {
         throw new Error("Logout failed");
      }
   },
   checkError: async () => {
      throw new Error("Error occurred");
   },
   checkAuth: async () => {
      const token = localStorage.getItem("accessToken");

      await authenticatorPulse();

      const request = new Request("https://api.erzen.tk/auth/info", {
         method: "GET",
         redirect: "follow" as RequestRedirect,
         credentials: "include" as RequestCredentials,
         headers: new Headers({
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
         }),
      });

      try {
         const response = await fetch(request);
         if (response.status < 200 || (response.status >= 300 && response.status !== 404)) {
            throw new Error(response.statusText);
         }
      } catch (error) {
         localStorage.removeItem("accessToken");
         throw new Error("Authentication failed");
      }
   },
   getPermissions: async () => {
      const request = new Request("https://api.erzen.tk/stats", {
         method: "GET",
         redirect: "follow" as RequestRedirect,
         credentials: "include" as RequestCredentials,
         headers: new Headers({
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
         }),
      });
      try {
         const response = await fetch(request);
         if (response.status < 200 || (response.status >= 300 && response.status !== 404)) {
            throw new Error(response.statusText);
         }
         const data = await response.json();
         if (data) {
            return "admin";
         } else {
            return "user";
         }
      } catch (error) {
         throw new Error("Failed to get permissions");
      }
   },
   getInfo: async () => {
      const request = new Request("https://api.erzen.tk/auth/info", {
         method: "GET",
         redirect: "follow" as RequestRedirect,
         credentials: "include" as RequestCredentials,
         headers: new Headers({
            "Content-Type": "application/json",
         }),
      });
      try {
         const response = await fetch(request);
         if (response.status < 200 || response.status >= 300) {
            throw new Error(response.statusText);
         }
         const data = await response.json();
         return {
            fullName: data.fullName,
            username: data.username,
            email: data.email,
            role: data.role,
            id: data.id,
         };
      } catch (error) {
         throw new Error("Failed to get user info");
      }
   },
};

const App = () => (
   <Admin
      theme={lightTheme}
      darkTheme={darkTheme}
      dataProvider={restProvider("https://api.erzen.tk", httpClient)}
      authProvider={{
         ...authProvider,
      }}
   >
      <Resource
         name="podcasts"
         list={PodcastList}
         create={PodcastCreate}
         edit={PodcastEdit}
         options={{ responseValidationMode: "next 3xx 4xx 5xx" }}
      />

      <Resource
         name="episodes"
         list={EpisodeList}
         create={EpisodeCreate}
         edit={EpisodeEdit}
         show={ShowEpisode}
      />
      <Resource name="categories" list={ListCategory} create={CreateCategory} edit={EditCategory} />

      <Resource
         name="user"
         list={ListUsers}
         options={{ pagination: { page: 0 } }}
         edit={UsersEdit}
      />

      <Resource name="comments" list={ListComments} create={CommentCreate} edit={CommentEdit} />
   </Admin>
);

export default App;
