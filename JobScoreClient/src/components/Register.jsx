import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import {
  Box,
  Container,
  TextField,
  Button,
  Typography,
  Paper,
  IconButton,
  InputAdornment,
  Alert,
  CircularProgress,
  Stack,
} from "@mui/material";
import { Visibility, VisibilityOff } from "@mui/icons-material";
import authService from "../services/authService";
import LogoHeader from "./LogoHeader";

function Register() {
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    email: "",
    password: "",
    confirmPassword: "",
  });
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [fieldErrors, setFieldErrors] = useState({});
  const navigate = useNavigate();

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: value,
    });
    // Clear field error when user starts typing
    if (fieldErrors[name]) {
      setFieldErrors({
        ...fieldErrors,
        [name]: undefined,
      });
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setFieldErrors({});

    if (formData.password !== formData.confirmPassword) {
      setFieldErrors({ confirmPassword: ["Passwords do not match!"] });
      return;
    }

    setLoading(true);

    try {
      const response = await authService.Register(
        formData.firstName,
        formData.lastName,
        formData.email,
        formData.password,
        formData.confirmPassword,
      );
      console.log("Registration successful:", response.data);
      console.log("Response headers:", response.headers);
      console.log("Cookies:", document.cookie);
      // Navigate to login after successful registration
      navigate("/home");
    } catch (err) {
      console.error("Full registration error:", err.response?.data);

      // Handle field-specific validation errors
      if (err.response?.data?.errors) {
        setFieldErrors(err.response.data.errors);
      } else {
        // Handle general errors
        const errorMessage =
          err.response?.data?.error ||
          err.response?.data?.message ||
          err.response?.data?.title ||
          "Registration failed. Please try again.";

        setError(errorMessage);
      }
      console.error("Registration error:", err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container component="main" maxWidth="xs">
      <Box
        sx={{
          marginTop: { xs: 4, sm: 8 },
          marginBottom: { xs: 4, sm: 0 },
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          px: { xs: 2, sm: 0 },
        }}
      >
        <LogoHeader />

        {/* Form */}
        <Paper
          elevation={3}
          sx={{
            p: { xs: 3, sm: 4 },
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
            width: "100%",
          }}
        >
          <Typography component="h1" variant="h5" sx={{ mb: { xs: 2, sm: 3 } }}>
            Sign Up
          </Typography>

          {error && (
            <Alert severity="error" sx={{ width: "100%", mb: 2 }}>
              {error}
            </Alert>
          )}

          <Box component="form" onSubmit={handleSubmit} sx={{ width: "100%" }}>
            <Stack spacing={{ xs: 1.5, sm: 2 }}>
              <Stack
                direction={{ xs: "column", sm: "row" }}
                spacing={{ xs: 1.5, sm: 2 }}
              >
                <TextField
                  autoComplete="given-name"
                  name="firstName"
                  required
                  fullWidth
                  id="firstName"
                  label="First Name"
                  autoFocus
                  value={formData.firstName}
                  onChange={handleChange}
                  error={!!fieldErrors.firstName}
                  helperText={fieldErrors.firstName?.[0]}
                />
                <TextField
                  required
                  fullWidth
                  id="lastName"
                  label="Last Name"
                  name="lastName"
                  autoComplete="family-name"
                  value={formData.lastName}
                  onChange={handleChange}
                  error={!!fieldErrors.lastName}
                  helperText={fieldErrors.lastName?.[0]}
                />
              </Stack>
              <TextField
                required
                fullWidth
                id="email"
                label="Email Address"
                name="email"
                autoComplete="email"
                value={formData.email}
                onChange={handleChange}
                error={!!fieldErrors.email}
                helperText={fieldErrors.email?.[0]}
              />
              <TextField
                required
                fullWidth
                name="password"
                label="Password"
                type={showPassword ? "text" : "password"}
                id="password"
                autoComplete="new-password"
                value={formData.password}
                onChange={handleChange}
                error={!!fieldErrors.password}
                helperText={fieldErrors.password?.[0]}
                InputProps={{
                  endAdornment: (
                    <InputAdornment position="end">
                      <IconButton
                        aria-label="toggle password visibility"
                        onClick={() => setShowPassword(!showPassword)}
                        edge="end"
                      >
                        {showPassword ? <VisibilityOff /> : <Visibility />}
                      </IconButton>
                    </InputAdornment>
                  ),
                }}
              />
              <TextField
                required
                fullWidth
                name="confirmPassword"
                label="Confirm Password"
                type={showConfirmPassword ? "text" : "password"}
                id="confirmPassword"
                autoComplete="new-password"
                value={formData.confirmPassword}
                onChange={handleChange}
                error={!!fieldErrors.confirmPassword}
                helperText={fieldErrors.confirmPassword?.[0]}
                InputProps={{
                  endAdornment: (
                    <InputAdornment position="end">
                      <IconButton
                        aria-label="toggle password visibility"
                        onClick={() =>
                          setShowConfirmPassword(!showConfirmPassword)
                        }
                        edge="end"
                      >
                        {showConfirmPassword ? (
                          <VisibilityOff />
                        ) : (
                          <Visibility />
                        )}
                      </IconButton>
                    </InputAdornment>
                  ),
                }}
              />
            </Stack>
            <Button
              type="submit"
              fullWidth
              variant="contained"
              sx={{ mt: { xs: 2, sm: 3 }, mb: 2 }}
              disabled={loading}
            >
              {loading ? <CircularProgress size={24} /> : "Sign Up"}
            </Button>
            <Box sx={{ textAlign: "center" }}>
              <Link
                to="/login"
                style={{ textDecoration: "none", color: "#1976d2" }}
              >
                <Typography variant="body2">
                  Already have an account? Sign In
                </Typography>
              </Link>
            </Box>
          </Box>
        </Paper>
      </Box>
    </Container>
  );
}

export default Register;
