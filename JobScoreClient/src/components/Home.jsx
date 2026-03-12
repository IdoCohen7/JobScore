import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import {
  Box,
  Container,
  Paper,
  Typography,
  CircularProgress,
  Alert,
  Button,
  Divider,
  Stack,
  Chip,
} from "@mui/material";
import {
  Person,
  Email,
  AdminPanelSettings,
  ExitToApp,
} from "@mui/icons-material";
import authService from "../services/authService";
import tokenService from "../utils/tokenService";

function Home() {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const navigate = useNavigate();

  useEffect(() => {
    fetchUserData();
  }, []);

  const fetchUserData = async () => {
    try {
      console.log("Fetching user data...");
      const response = await authService.Me();
      setUser(response.data);
      console.log("User data loaded successfully:", response.data);
    } catch (err) {
      console.error("Error fetching user data:", err);
      console.error("Error response:", err.response?.data);
      console.error("Error status:", err.response?.status);

      const errorMessage =
        err.response?.data?.error ||
        err.response?.data?.message ||
        err.response?.status === 401
          ? "Session expired or invalid. Please log in again."
          : "Failed to load user data. Please try logging in again.";

      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = () => {
    // Clear the JWT token from localStorage
    tokenService.removeToken();
    console.log("Logged out, token cleared");
    navigate("/login");
  };

  if (loading) {
    return (
      <Container component="main" maxWidth="sm">
        <Box
          sx={{
            marginTop: 8,
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
          }}
        >
          <CircularProgress />
        </Box>
      </Container>
    );
  }

  if (error) {
    return (
      <Container component="main" maxWidth="sm">
        <Box sx={{ marginTop: 8 }}>
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
          <Button variant="contained" onClick={() => navigate("/login")}>
            Go to Login
          </Button>
        </Box>
      </Container>
    );
  }

  return (
    <Container component="main" maxWidth="sm">
      <Box
        sx={{
          marginTop: { xs: 4, sm: 8 },
          marginBottom: { xs: 4, sm: 0 },
          px: { xs: 2, sm: 0 },
        }}
      >
        <Paper
          elevation={3}
          sx={{
            p: { xs: 3, sm: 4 },
          }}
        >
          <Box
            sx={{
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
              mb: 3,
            }}
          >
            <Box>
              <Typography component="h1" variant="h4">
                Welcome Back!
              </Typography>
              {user?.isAdmin && (
                <Chip
                  label="Admin"
                  color="primary"
                  size="small"
                  icon={<AdminPanelSettings />}
                  sx={{ mt: 1 }}
                />
              )}
            </Box>
            <Button
              variant="outlined"
              color="error"
              startIcon={<ExitToApp />}
              onClick={handleLogout}
            >
              Logout
            </Button>
          </Box>

          <Divider sx={{ mb: 3 }} />

          <Stack spacing={3}>
            <Box sx={{ display: "flex", alignItems: "center", gap: 2 }}>
              <Person color="primary" />
              <Box>
                <Typography variant="caption" color="text.secondary">
                  Name
                </Typography>
                <Typography variant="body1">
                  {user?.firstName} {user?.lastName}
                </Typography>
              </Box>
            </Box>

            <Box sx={{ display: "flex", alignItems: "center", gap: 2 }}>
              <Email color="primary" />
              <Box>
                <Typography variant="caption" color="text.secondary">
                  Email
                </Typography>
                <Typography variant="body1">{user?.email}</Typography>
              </Box>
            </Box>
          </Stack>
        </Paper>
      </Box>
    </Container>
  );
}

export default Home;
