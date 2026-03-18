import { useState, useEffect, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import { setUser } from "../store/slices/userSlice";
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
import EvaluationResult from "./EvaluationResult";

function Home() {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [jobTitle, setJobTitle] = useState("");
  const [jobDescription, setJobDescription] = useState("");
  const [evaluating, setEvaluating] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [evaluationResult, setEvaluationResult] = useState(null);
  const [evaluateError, setEvaluateError] = useState("");
  const [submitMessage, setSubmitMessage] = useState("");
  const [submitError, setSubmitError] = useState("");
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const user = useSelector((state) => state.user.user);

  const fetchUserData = useCallback(async () => {
    if (user) {
      setLoading(false);
      return;
    }

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
  }, [dispatch, user]);

  useEffect(() => {
    fetchUserData();
  }, [fetchUserData]);

  const handleEvaluate = async () => {
    setEvaluating(true);
    setEvaluateError("");
    setSubmitMessage("");
    try {
      console.log("Evaluating job description:", { jobTitle, jobDescription });
      const response = await jobDescriptionService.Evaluate(
        jobTitle,
        jobDescription,
      );

      setEvaluationResult({
        score: Number(response.data?.score ?? 0),
        violations: Array.isArray(response.data?.violations)
          ? response.data.violations
          : [],
      });
    } catch (err) {
      console.error("Evaluation error:", err);
      console.error("Error response:", err.response?.data);
      setEvaluationResult(null);
      setEvaluateError(
        err.response?.data?.error ||
          err.response?.data?.message ||
          "Could not evaluate this job description. Please try again.",
      );
    } finally {
      setEvaluating(false);
    }
  };

  const handleSubmit = async () => {
    setSubmitting(true);
    setSubmitError("");
    setSubmitMessage("");
    try {
      console.log("Submitting job description:", { jobTitle, jobDescription });
      await jobDescriptionService.Create(jobTitle, jobDescription);

      // Clear form after successful submission
      setJobTitle("");
      setJobDescription("");
      setEvaluationResult(null);
      setSubmitMessage("Success! Job description submitted.");
    } catch (err) {
      console.error("Submit error:", err);
      console.error("Error response:", err.response?.data);
      setSubmitError(
        err.response?.data?.error ||
          err.response?.data?.message ||
          "Submit failed. Please check the data and try again.",
      );
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
      <Container component="main" maxWidth="lg">
        <Box className="page-container">
          <Box
            className={`home-layout ${
              evaluationResult
                ? "home-layout-with-panel"
                : "home-layout-centered"
            }`}
          >
            <Paper elevation={3} className="content-paper home-main-card">
              <Typography variant="h5" gutterBottom className="page-title">
                Job Description Evaluator
              </Typography>

              <TextField
                style={{ marginTop: "7px" }}
                fullWidth
                label="Job Title"
                placeholder="Enter job title..."
                value={jobTitle}
                onChange={(e) => {
                  setJobTitle(e.target.value);
                  setSubmitMessage("");
                  setSubmitError("");
                }}
                className="job-title-input"
                variant="outlined"
              />

              <TextField
                style={{ marginTop: "10px" }}
                multiline
                rows={12}
                fullWidth
                label="Job Description"
                placeholder="Paste your job description here..."
                value={jobDescription}
                onChange={(e) => {
                  setJobDescription(e.target.value);
                  setSubmitMessage("");
                  setSubmitError("");
                }}
                className="job-description-input"
                variant="outlined"
              />

              {evaluateError && (
                <Alert severity="error" className="home-alert">
                  {evaluateError}
                </Alert>
              )}

              {submitError && (
                <Alert severity="error" className="home-alert">
                  {submitError}
                </Alert>
              )}

              {submitMessage && (
                <Alert severity="success" className="home-alert">
                  {submitMessage}
                </Alert>
              )}

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

            <EvaluationResult result={evaluationResult} />
          </Box>
        </Box>
      </Container>
    </Box>
  );
}

export default Home;
