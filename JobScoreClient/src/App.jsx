import {
  BrowserRouter as Router,
  Routes,
  Route,
  Navigate,
  Outlet,
} from "react-router-dom";
import { CssBaseline, ThemeProvider, createTheme } from "@mui/material";
import { Provider } from "react-redux";
import { store } from "./store/store";
import Login from "./components/Login";
import Register from "./components/Register";
import Home from "./components/Home";
import Chat from "./components/Chat";
import Buzzwords from "./components/Buzzwords";
import Metrics from "./components/Metrics";
import Rules from "./components/Rules";
import NavBar from "./components/NavBar";
import tokenService from "./utils/tokenService";

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

function ProtectedLayout() {
  const { isAuthenticated } = getAuthState();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return (
    <>
      <NavBar />
      <Outlet />
    </>
  );
}

function AdminLayout() {
  const { isAuthenticated, isAdmin } = getAuthState();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (!isAdmin) {
    return <Navigate to="/home" replace />;
  }

  return (
    <>
      <NavBar />
      <Outlet />
    </>
  );
}

function App() {
  return (
    <Provider store={store}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <Router>
          <Routes>
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />

            <Route element={<ProtectedLayout />}>
              <Route path="/home" element={<Home />} />
              <Route path="/chat" element={<Chat />} />
            </Route>

            <Route element={<AdminLayout />}>
              <Route path="/buzzwords" element={<Buzzwords />} />
              <Route path="/metrics" element={<Metrics />} />
              <Route path="/rules" element={<Rules />} />
            </Route>

            <Route path="/" element={<Navigate to="/home" replace />} />
            <Route path="*" element={<Navigate to="/home" replace />} />
          </Routes>
        </Router>
      </ThemeProvider>
    </Provider>
  );
}

export default App;
