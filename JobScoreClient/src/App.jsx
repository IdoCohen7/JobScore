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

function App() {
  return (
    <Provider store={store}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <Router>
          <Routes>
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route path="/home" element={<Home />} />
            <Route path="/buzzwords" element={<Buzzwords />} />
            <Route path="/metrics" element={<Metrics />} />
            <Route path="/rules" element={<Rules />} />
            <Route path="/" element={<Navigate to="/home" replace />} />
          </Routes>
        </Router>
      </ThemeProvider>
    </Provider>
  );
}

export default App;
