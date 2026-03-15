import {
  BrowserRouter as Router,
  Routes,
  Route,
  Navigate,
} from "react-router-dom";
import { CssBaseline, ThemeProvider, createTheme } from "@mui/material";
import { Provider } from "react-redux";
import { store } from "./store/store";
import Login from "./components/Login";
import Register from "./components/Register";
import Home from "./components/Home";
import Buzzwords from "./components/Buzzwords";
import Metrics from "./components/Metrics";
import Rules from "./components/Rules";
import tokenService from "./utils/tokenService";
import "./App.css";

const theme = createTheme({
  palette: {
    primary: {
      main: "#1976d2",
    },
    secondary: {
      main: "#dc004e",
    },
    background: {
      default: "#f1f1f1",
      paper: "#ffffff",
    },
  },
});

const getAuthState = () => {
  const token = tokenService.getToken();
  if (!token || tokenService.isTokenExpired(token)) {
    tokenService.removeToken();
    return { isAuthenticated: false, isAdmin: false };
  }

  const payload = tokenService.decodeToken(token);
  const isAdminClaim = payload?.IsAdmin;
  const isAdmin = isAdminClaim === true || isAdminClaim === "True";

  return { isAuthenticated: true, isAdmin };
};

function App() {
  const { isAuthenticated, isAdmin } = getAuthState();

  return (
    <Provider store={store}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <Router>
          <Routes>
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route
              path="/home"
              element={
                isAuthenticated ? <Home /> : <Navigate to="/login" replace />
              }
            />
            <Route
              path="/buzzwords"
              element={
                isAuthenticated ? (
                  isAdmin ? (
                    <Buzzwords />
                  ) : (
                    <Navigate to="/home" replace />
                  )
                ) : (
                  <Navigate to="/login" replace />
                )
              }
            />
            <Route
              path="/metrics"
              element={
                isAuthenticated ? (
                  isAdmin ? (
                    <Metrics />
                  ) : (
                    <Navigate to="/home" replace />
                  )
                ) : (
                  <Navigate to="/login" replace />
                )
              }
            />
            <Route
              path="/rules"
              element={
                isAuthenticated ? (
                  isAdmin ? (
                    <Rules />
                  ) : (
                    <Navigate to="/home" replace />
                  )
                ) : (
                  <Navigate to="/login" replace />
                )
              }
            />
            <Route path="/" element={<Navigate to="/home" replace />} />
            <Route path="*" element={<Navigate to="/home" replace />} />
          </Routes>
        </Router>
      </ThemeProvider>
    </Provider>
  );
}

export default App;
