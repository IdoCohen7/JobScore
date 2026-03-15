import { useSelector, useDispatch } from "react-redux";
import { useNavigate, useLocation } from "react-router-dom";
import {
  Box,
  Tabs,
  Tab,
  AppBar,
  Toolbar,
  Button,
  useMediaQuery,
  useTheme,
} from "@mui/material";
import { ExitToApp } from "@mui/icons-material";
import { clearUser } from "../store/slices/userSlice";
import tokenService from "../utils/tokenService";
import "./components.css";

function NavBar({ currentTab }) {
  const user = useSelector((state) => state.user.user);
  const isAdmin = user?.isAdmin || false;
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const location = useLocation();
  const theme = useTheme();
  const isSmallScreen = useMediaQuery(theme.breakpoints.down("sm"));

  // Determine current tab from URL if not provided
  const activeTab = currentTab || location.pathname.replace("/", "") || "home";
  const availableTabs = [
    "home",
    ...(isAdmin ? ["buzzwords", "metrics", "rules"] : []),
  ];
  const selectedTab = availableTabs.includes(activeTab) ? activeTab : false;

  const handleChange = (event, newValue) => {
    navigate(`/${newValue}`);
  };

  const handleLogout = () => {
    tokenService.removeToken();
    dispatch(clearUser());
    console.log("Logged out, token and user data cleared");
    navigate("/login");
  };

  return (
    <AppBar position="static" color="default" elevation={1}>
      <Toolbar className="navbar">
        <Tabs
          className="navbar-tabs"
          value={selectedTab}
          onChange={handleChange}
          indicatorColor="primary"
          textColor="primary"
          variant="scrollable"
          scrollButtons="auto"
          allowScrollButtonsMobile
        >
          <Tab label="Home" value="home" />
          {isAdmin && <Tab label="Buzzwords" value="buzzwords" />}
          {isAdmin && <Tab label="Metrics" value="metrics" />}
          {isAdmin && <Tab label="Rules" value="rules" />}
        </Tabs>

        <Box className="navbar-logo">
          <Box component="img" src="/logo.png" alt="JobScore Logo" />
        </Box>

        <Button
          variant="outlined"
          color="error"
          startIcon={<ExitToApp />}
          onClick={handleLogout}
          className="logout-button"
        >
          {isSmallScreen ? "Out" : "Logout"}
        </Button>
      </Toolbar>
    </AppBar>
  );
}

export default NavBar;
