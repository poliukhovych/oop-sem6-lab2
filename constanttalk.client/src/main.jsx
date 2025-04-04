import React from "react";
import ReactDOM from "react-dom/client";
import App from "./App";
import { Auth0Provider } from "@auth0/auth0-react";
import "./index.css";
import 'bootstrap/dist/css/bootstrap.min.css';

const domain = "dev-c88fazm2qayf8am2.us.auth0.com";
const clientId = "6DHoNRlMJoqsZLyXwDinHQAiCEGeguJW";
const audience = "https://constanttalk777/api";

ReactDOM.createRoot(document.getElementById("root")).render(
  <Auth0Provider
    domain={domain}
    clientId={clientId}
    authorizationParams={{
      redirect_uri: "http://localhost:5173/",
      audience: audience,
      scope: "openid profile email read:users read:bills block:users unblock:users",
    }}
    cacheLocation="localstorage"
  >
    <App />
  </Auth0Provider>
);