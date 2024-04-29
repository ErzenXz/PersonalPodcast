import React from "react";
import ReactDOM from "react-dom/client";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import App from "./App";
import Login from "./pages/Login";
import Podcasts from "./pages/Podcasts";
import Podcast from "./pages/Podcast";
import Episode from "./pages/Episode";
import About from "./pages/About";
import Episodes from "./pages/Episodes";
import Account from "./pages/Account";

const router = createBrowserRouter([
   {
      path: "/",
      element: <App />,
   },
   {
      path: "/login",
      element: <Login />,
   },
   {
      path: "/podcasts",
      element: <Podcasts />,
   },
   {
      path: "/podcast/:podcastId",
      element: <Podcast />,
   },
   {
      path: "/episodes",
      element: <Episodes />,
   },
   {
      path: "/episode/:episodeId",
      element: <Episode />,
   },
   {
      path: "/about",
      element: <About />,
   },
   {
      path: "/account",
      element: <Account />,
   },
]);

ReactDOM.createRoot(document.getElementById("root")!).render(
   <React.StrictMode>
      <RouterProvider router={router} />
   </React.StrictMode>
);
