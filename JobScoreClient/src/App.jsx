import { useEffect, useRef } from "react";
import {
  BrowserRouter as Router,
  Routes,
  Route,
  Navigate,
  Outlet,
} from "react-router-dom";
import { CssBaseline, ThemeProvider, createTheme } from "@mui/material";
import { Provider, useDispatch, useSelector } from "react-redux";
import { store } from "./store/store";
import { initializeAuth } from "./store/slices/userSlice";
import Login from "./components/Login";
import Register from "./components/Register";
import Home from "./components/Home";
import Chat from "./components/chat/Chat";
import Buzzwords from "./components/Buzzwords";
import Metrics from "./components/Metrics";
import Rules from "./components/Rules";
import NavBar from "./components/NavBar";

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

function ProtectedLayout() {
  const isAuthenticated = useSelector((state) => state.user.isAuthenticated);

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
  const isAuthenticated = useSelector((state) => state.user.isAuthenticated);
  const isAdmin = useSelector((state) => Boolean(state.user.user?.isAdmin));

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

function AppContent() {
  const dispatch = useDispatch();
  const hasInitializedRef = useRef(false);
  const isAuthInitialized = useSelector((state) => state.user.authInitialized);

  useEffect(() => {
    if (hasInitializedRef.current) {
      return;
    }

    hasInitializedRef.current = true;
    dispatch(initializeAuth());
  }, [dispatch]);

  if (!isAuthInitialized) {
    return null;
  }

  return (
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
  );
}

function App() {
  return (
    <Provider store={store}>
      <AppContent />
    </Provider>
  );
}

export default App;
