import React from "react";
import ReactDOM from "react-dom/client";

// import App from "./App.tsx";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import App from "./App";
import Login from "./pages/Login";
import Podcasts from "./pages/Podcasts";
import Podcast from "./pages/Podcast";

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
      element: <div>Episodes</div>,
   },
   {
      path: "/about",
      element: <div>About</div>,
   },
   {
      path: "/account",
      element: <div>Account</div>,
   },
]);

ReactDOM.createRoot(document.getElementById("root")!).render(
   <React.StrictMode>
      <RouterProvider router={router} />

import App from "./App.tsx";

ReactDOM.createRoot(document.getElementById("root")!).render(
   <React.StrictMode>
      <App />

   </React.StrictMode>
);
