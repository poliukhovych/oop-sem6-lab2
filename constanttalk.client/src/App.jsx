import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Home from "./pages/Home";
import LandingPage from "./pages/LandingPage";
import AdminDashboard from "./pages/AdminDashboard";
import UserDashboard from "./pages/UserDashboard";
import Unauthorized from "./pages/Unauthorized";
import { ProtectedRoute } from "./components/ProtectedRoute";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

const App = () => {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/login" element={<LandingPage />} />
        <Route path="/unauthorized" element={<Unauthorized />} />

        <Route
          path="/admin"
          element={<ProtectedRoute component={AdminDashboard} allowedRoles={["admin"]} />}
        />

        <Route
          path="/subscriber"
          element={<ProtectedRoute component={UserDashboard} allowedRoles={["subscriber"]} />}
        />
      </Routes>
      <ToastContainer />
    </Router>
  );
};

export default App;