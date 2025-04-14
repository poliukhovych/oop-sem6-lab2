import { withAuthenticationRequired } from "@auth0/auth0-react";
import { Navigate } from "react-router-dom";
import { useAuth0 } from "@auth0/auth0-react";
import { useEffect, useState } from "react";
import { jwtDecode } from "jwt-decode";
import { Spinner } from "react-bootstrap";

const ProtectedRouteWithRoles = ({ component, allowedRoles, ...args }) => {
  const { isAuthenticated, getAccessTokenSilently } = useAuth0();
  const [userRoles, setUserRoles] = useState(null);
  const [isLoadingRoles, setIsLoadingRoles] = useState(true);
  const Component = component;

  useEffect(() => {
    const fetchRoles = async () => {
      if (isAuthenticated) {
        try {
          const accessToken = await getAccessTokenSilently();
          const decodedToken = jwtDecode(accessToken);
          const roles = decodedToken?.["https://constanttalk777/api/roles"] || [];
          setUserRoles(roles);
        } catch (error) {
          console.error("Error fetching roles:", error);
          setUserRoles([]);
        } finally {
          setIsLoadingRoles(false);
        }
      } else {
        setIsLoadingRoles(false);
        setUserRoles([]);
      }
    };

    fetchRoles();
  }, [isAuthenticated, getAccessTokenSilently]);

  if (!isAuthenticated || isLoadingRoles || userRoles === null) {
    return (
      <div className="d-flex justify-content-center align-items-center vh-100">
        <Spinner animation="border" role="status">
          <span className="visually-hidden">{!isAuthenticated ? "Redirecting to Login..." : isLoadingRoles ? "Checking Roles..." : "Loading..."}</span>
        </Spinner>
      </div>
    );
  }

  if (!userRoles.some(role => allowedRoles.includes(role))) {
    return <Navigate to="/unauthorized" />;
  }

  return <Component {...args} />;
};

export const ProtectedRoute = withAuthenticationRequired(ProtectedRouteWithRoles, {
  onRedirecting: () => (
    <div className="d-flex justify-content-center align-items-center vh-100">
      <Spinner animation="border" role="status">
        <span className="visually-hidden">Redirecting to Login...</span>
      </Spinner>
    </div>
  ),
});