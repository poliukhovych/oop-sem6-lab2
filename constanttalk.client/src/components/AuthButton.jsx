import { useAuth0 } from "@auth0/auth0-react";
import { Button } from "react-bootstrap";

function AuthButton() {
  const { loginWithRedirect, logout, isAuthenticated } = useAuth0();

  return isAuthenticated ? (
    <Button
      variant="outline-light"
      size="lg"
      className="fw-bold px-4 py-2"
      onClick={() => logout({ returnTo: window.location.origin })}
      style={{
        borderRadius: "25px",
        boxShadow: "0 4px 8px rgba(0, 0, 0, 0.3)",
        transition: "transform 0.2s ease-in-out",
      }}
      onMouseOver={(e) => (e.target.style.transform = "scale(1.05)")}
      onMouseOut={(e) => (e.target.style.transform = "scale(1)")}
    >
      Log out
    </Button>
  ) : (
    <Button
      variant="outline-warning"
      size="lg"
      className="fw-bold px-4 py-2"
      onClick={() => loginWithRedirect()}
      style={{
        borderRadius: "25px",
        boxShadow: "0 4px 8px rgba(0, 0, 0, 0.3)",
        transition: "transform 0.2s ease-in-out",
      }}
      onMouseOver={(e) => (e.target.style.transform = "scale(1.05)")}
      onMouseOut={(e) => (e.target.style.transform = "scale(1)")}
    >
      Log in
    </Button>
  );
}

export default AuthButton;