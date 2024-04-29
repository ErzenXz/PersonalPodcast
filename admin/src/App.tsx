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

// Function to get the accessToken from the localStorage

const getAccessToken = () => localStorage.getItem("accessToken");

import { Options } from "ra-core";
import PodcastEdit from "./components/PodcastEdit";
import ShowEpisode from "./components/ShowEpisode";
import authenticatorPulse from "./service/authenticatorPulse";

const httpClient = (url: string, options: Options = { headers: new Headers() }) => {
   if (!options.headers) {
      options.headers = new Headers({ Accept: "application/json" });
   }
   const token = getAccessToken();
   (options.headers as Headers).set("Authorization", `Bearer ${token}`);
   return fetchUtils.fetchJson(url, options);
};

const authProvider = {
   login: ({ username, password }: { username: string; password: string }) => {
      const request = new Request("https://api.erzen.tk/auth/login", {
         method: "POST",
         body: JSON.stringify({ email: username, password }),
         headers: new Headers({ "Content-Type": "application/json" }),
      });
      return fetch(request)
         .then((response) => {
            if (response.status < 200 || response.status >= 300) {
               throw new Error(response.statusText);
            }
            return response.json();
         })
         .then(({ token }) => {
            localStorage.setItem("accessToken", token);
            const expireIn15Mins = new Date(Date.now() + 15 * 60000);
            localStorage.setItem("tokenExpireDate", expireIn15Mins.toISOString());
         });
   },
   logout: () => {
      const request = new Request("https://api.erzen.tk/auth/logout", {
         method: "POST",
         redirect: "follow" as RequestRedirect,
         credentials: "include" as RequestCredentials,
         headers: new Headers({
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("accessToken")}`, // Include the token
         }),
      });
      return fetch(request).then(() => {
         localStorage.removeItem("accessToken"); // Remove the token after logging out
         return Promise.resolve();
      });
   },
   checkError: () => {
      return Promise.reject();
   },
   checkAuth: () => {
      const request = new Request("https://api.erzen.tk/auth/info", {
         method: "GET",
         redirect: "follow" as RequestRedirect,
         credentials: "include" as RequestCredentials,
         headers: new Headers({
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
         }),
      });
      return fetch(request)
         .then((response) => {
            if (response.status < 200 || response.status >= 300) {
               throw new Error(response.statusText);
            }
            authenticatorPulse();
            return response.json();
         })
         .then(() => {
            return Promise.resolve();
         })
         .catch(() => {
            localStorage.removeItem("accessToken");
            return Promise.reject();
         });
   },
   getPermissions: () => {
      const request = new Request("https://api.erzen.tk/stats", {
         method: "GET",
         redirect: "follow" as RequestRedirect,
         credentials: "include" as RequestCredentials,
         headers: new Headers({
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("accessToken")}`, // Include the token
         }),
      });
      return fetch(request)
         .then((response) => {
            if (response.status < 200 || response.status >= 300) {
               throw new Error(response.statusText);
            }
            return response.json();
         })
         .then((data) => {
            if (data) {
               return "admin";
            } else {
               return "user";
            }
         });
   },
   getInfo: () => {
      const request = new Request("https://api.erzen.tk/auth/info", {
         method: "GET",
         redirect: "follow" as RequestRedirect,
         credentials: "include" as RequestCredentials,
         headers: new Headers({
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("accessToken")}`, // Include the token
         }),
      });
      return fetch(request)
         .then((response) => {
            if (response.status < 200 || response.status >= 300) {
               throw new Error(response.statusText);
            }
            return response.json();
         })
         .then((data) => {
            return {
               fullName: data.fullName,
               username: data.username,
               email: data.email,
               role: data.role,
               id: data.id,
            };
         });
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

      <Resource name="user/all" list={ListUsers} options={{ pagination: { page: 0 } }} />
   </Admin>
);

export default App;
