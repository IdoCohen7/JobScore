import { useEffect, useState } from "react";
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Container,
  IconButton,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from "@mui/material";
import DeleteOutlineIcon from "@mui/icons-material/DeleteOutline";
import NavBar from "./NavBar";
import buzzwordService from "../services/buzzwordService";
import "./components.css";

function Buzzwords() {
  const [buzzwords, setBuzzwords] = useState([]);
  const [newBuzzword, setNewBuzzword] = useState("");
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [deletingId, setDeletingId] = useState(null);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const loadBuzzwords = async () => {
    setLoading(true);
    setError("");

    try {
      const response = await buzzwordService.GetBuzzwords();
      const list = Array.isArray(response.data) ? response.data : [];
      setBuzzwords(list);
    } catch (loadError) {
      setError(loadError.response?.data?.error || "Failed to load buzzwords.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadBuzzwords();
  }, []);

  const handleCreateBuzzword = async (event) => {
    event.preventDefault();
    setError("");
    setSuccess("");

    const name = newBuzzword.trim();
    if (!name) {
      setError("Buzzword name is required.");
      return;
    }

    setSubmitting(true);
    try {
      await buzzwordService.CreateBuzzword(name);
      setNewBuzzword("");
      setSuccess("Buzzword added successfully.");
      await loadBuzzwords();
    } catch (createError) {
      setError(createError.response?.data?.error || "Failed to add buzzword.");
    } finally {
      setSubmitting(false);
    }
  };

  const handleDeleteBuzzword = async (id) => {
    setError("");
    setSuccess("");

    setDeletingId(id);
    try {
      await buzzwordService.DeleteBuzzword(id);
      setSuccess("Buzzword deleted successfully.");
      setBuzzwords((previous) => previous.filter((item) => item.id !== id));
    } catch (deleteError) {
      setError(
        deleteError.response?.data?.error || "Failed to delete buzzword.",
      );
    } finally {
      setDeletingId(null);
    }
  };

  return (
    <Box>
      <NavBar />
      <Container component="main" maxWidth="lg">
        <Box className="page-container">
          <Paper elevation={3} className="content-paper">
            <Typography variant="h5" gutterBottom className="page-title">
              Buzzwords Management
            </Typography>

            {error && (
              <Alert severity="error" sx={{ mb: 2 }}>
                {error}
              </Alert>
            )}

            {success && (
              <Alert severity="success" sx={{ mb: 2 }}>
                {success}
              </Alert>
            )}

            <Box
              component="form"
              onSubmit={handleCreateBuzzword}
              className="buzzword-form"
              sx={{
                display: "flex",
                gap: 2,
                alignItems: "flex-start",
                mb: 2.5,
                flexWrap: "wrap",
              }}
            >
              <TextField
                label="New buzzword"
                value={newBuzzword}
                onChange={(event) => setNewBuzzword(event.target.value)}
                size="small"
                sx={{ minWidth: 260, flex: 1 }}
              />
              <Button
                type="submit"
                variant="contained"
                disabled={submitting}
                className="submit-button"
              >
                {submitting ? "Adding..." : "Add Buzzword"}
              </Button>
            </Box>

            {loading ? (
              <Box
                sx={{
                  display: "flex",
                  justifyContent: "center",
                  alignItems: "center",
                  py: 6,
                }}
              >
                <CircularProgress />
              </Box>
            ) : (
              <TableContainer
                component={Paper}
                variant="outlined"
                className="responsive-table-container"
              >
                <Table
                  size="small"
                  aria-label="buzzwords table"
                  className="responsive-table"
                >
                  <TableHead>
                    <TableRow>
                      <TableCell sx={{ fontWeight: 700 }}>Name</TableCell>
                      <TableCell align="right" sx={{ fontWeight: 700 }}>
                        Count
                      </TableCell>
                      <TableCell
                        align="right"
                        sx={{ fontWeight: 700, width: 120 }}
                      >
                        Actions
                      </TableCell>
                    </TableRow>
                  </TableHead>

                  <TableBody>
                    {buzzwords.length === 0 ? (
                      <TableRow>
                        <TableCell colSpan={3} align="center" sx={{ py: 3 }}>
                          No buzzwords found.
                        </TableCell>
                      </TableRow>
                    ) : (
                      buzzwords.map((buzzword) => (
                        <TableRow key={buzzword.id} hover>
                          <TableCell>{buzzword.name}</TableCell>
                          <TableCell align="right">{buzzword.count}</TableCell>
                          <TableCell align="right">
                            <IconButton
                              color="error"
                              aria-label={`Delete ${buzzword.name}`}
                              onClick={() => handleDeleteBuzzword(buzzword.id)}
                              disabled={deletingId === buzzword.id}
                            >
                              <DeleteOutlineIcon />
                            </IconButton>
                          </TableCell>
                        </TableRow>
                      ))
                    )}
                  </TableBody>
                </Table>
              </TableContainer>
            )}
          </Paper>
        </Box>
      </Container>
    </Box>
  );
}

export default Buzzwords;
