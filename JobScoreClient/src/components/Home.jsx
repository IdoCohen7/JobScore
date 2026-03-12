import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useSelector, useDispatch } from "react-redux";
import { setUser, clearUser } from "../store/slices/userSlice";
import {
  Box,
  Container,
  Paper,
  Typography,
  CircularProgress,
  Alert,
  Button,
  Stack,
  TextField,
} from "@mui/material";

import authService from "../services/authService";
import jobDescriptionService from "../services/jobDescriptionSerivce";
import tokenService from "../utils/tokenService";
import NavBar from "./NavBar";
import "./components.css";

function Home() {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [jobTitle, setJobTitle] = useState("");
  const [jobDescription, setJobDescription] = useState("");
  const [evaluating, setEvaluating] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const user = useSelector((state) => state.user.user);

  useEffect(() => {
    fetchUserData();
  }, []);

  const fetchUserData = async () => {
    try {
      console.log("Fetching user data...");
      const response = await authService.Me();
      dispatch(setUser(response.data));
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
    // Clear user from Redux
    dispatch(clearUser());
    console.log("Logged out, token and user data cleared");
    navigate("/login");
  };

  const handleEvaluate = async () => {
    setEvaluating(true);
    try {
      console.log("Evaluating job description:", { jobTitle, jobDescription });
      const response = await jobDescriptionService.Evaluate(
        jobTitle,
        jobDescription,
      );
      console.log("Evaluation response:", response.data);
      console.log("Score:", response.data.score);
      console.log("Violations:", response.data.violations);
    } catch (err) {
      console.error("Evaluation error:", err);
      console.error("Error response:", err.response?.data);
    } finally {
      setEvaluating(false);
    }
  };

  const handleSubmit = async () => {
    setSubmitting(true);
    try {
      console.log("Submitting job description:", { jobTitle, jobDescription });
      const response = await jobDescriptionService.Create(
        jobTitle,
        jobDescription,
      );
      console.log("Submit response:", response.data);
      // Clear form after successful submission
      setJobTitle("");
      setJobDescription("");
      // TODO: Show success message or redirect
    } catch (err) {
      console.error("Submit error:", err);
      console.error("Error response:", err.response?.data);
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return (
      <Container component="main" maxWidth="sm">
        <Box className="loading-container">
          <CircularProgress />
        </Box>
      </Container>
    );
  }

  if (error) {
    return (
      <Container component="main" maxWidth="sm">
        <Box className="error-container">
          <Alert severity="error" className="error-alert">
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
    <Box>
      <NavBar />
      <Container component="main" maxWidth="lg">
        <Box className="page-container">
          <Paper elevation={3} className="content-paper">
            <Typography variant="h5" gutterBottom className="page-title">
              Job Description Evaluator
            </Typography>

            <TextField
              fullWidth
              label="Job Title"
              placeholder="Enter job title..."
              value={jobTitle}
              onChange={(e) => setJobTitle(e.target.value)}
              className="job-title-input"
              variant="outlined"
            />

            <TextField
              multiline
              rows={12}
              fullWidth
              label="Job Description"
              placeholder="Paste your job description here..."
              value={jobDescription}
              onChange={(e) => setJobDescription(e.target.value)}
              className="job-description-input"
              variant="outlined"
            />

            <Stack
              direction="row"
              spacing={2}
              justifyContent="flex-end"
              className="button-group"
            >
              <Button
                variant="outlined"
                color="primary"
                onClick={handleEvaluate}
                disabled={
                  !jobDescription.trim() || !jobTitle.trim() || evaluating
                }
                size="large"
              >
                {evaluating ? <CircularProgress size={24} /> : "Evaluate"}
              </Button>
              <Button
                variant="contained"
                color="primary"
                onClick={handleSubmit}
                disabled={
                  !jobDescription.trim() || !jobTitle.trim() || submitting
                }
                size="large"
              >
                {submitting ? <CircularProgress size={24} /> : "Submit"}
              </Button>
            </Stack>
          </Paper>
        </Box>
      </Container>
    </Box>
  );
}

export default Home;
