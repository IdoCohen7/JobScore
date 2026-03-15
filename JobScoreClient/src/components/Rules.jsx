import { useEffect, useMemo, useState } from "react";
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Container,
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
import ruleService from "../services/ruleService";

function Rules() {
  const [rules, setRules] = useState([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  useEffect(() => {
    const loadRules = async () => {
      setLoading(true);
      setError("");

      try {
        const response = await ruleService.GetRules();
        const rulesData = Array.isArray(response.data) ? response.data : [];
        setRules(
          rulesData.map((rule) => ({
            ...rule,
            weight: Number(rule.weight) || 0,
          })),
        );
      } catch (loadError) {
        setError(loadError.response?.data?.error || "Failed to load rules.");
      } finally {
        setLoading(false);
      }
    };

    loadRules();
  }, []);

  const totalWeight = useMemo(() => {
    return rules.reduce((sum, rule) => sum + (Number(rule.weight) || 0), 0);
  }, [rules]);

  const handleWeightChange = (id, value) => {
    const normalizedWeight = Math.max(0, Number(value) || 0);

    setRules((previousRules) =>
      previousRules.map((rule) =>
        rule.id === id ? { ...rule, weight: normalizedWeight } : rule,
      ),
    );

    setSuccess("");
    setError("");
  };

  const handleSave = async () => {
    setError("");
    setSuccess("");

    if (totalWeight !== 100) {
      setError("Sum of all rule weights must be exactly 100 before saving.");
      return;
    }

    const payload = rules.map((rule) => ({
      id: rule.id,
      weight: Number(rule.weight) || 0,
    }));

    setSaving(true);
    try {
      await ruleService.UpdateRulesWeight(payload);
      setSuccess("Rule weights saved successfully.");
    } catch (saveError) {
      setError(
        saveError.response?.data?.error || "Failed to save rule weights.",
      );
    } finally {
      setSaving(false);
    }
  };

  return (
    <Box>
      <Container component="main" maxWidth="lg">
        <Box className="page-container">
          <Paper elevation={3} className="content-paper">
            <Typography variant="h5" gutterBottom className="page-title">
              Rules Configuration
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
              <>
                <TableContainer
                  component={Paper}
                  variant="outlined"
                  className="responsive-table-container"
                >
                  <Table
                    size="small"
                    aria-label="rules table"
                    className="responsive-table"
                  >
                    <TableHead>
                      <TableRow>
                        <TableCell sx={{ fontWeight: 700 }}>Id</TableCell>
                        <TableCell sx={{ fontWeight: 700 }}>Title</TableCell>
                        <TableCell sx={{ fontWeight: 700 }}>
                          Description
                        </TableCell>
                        <TableCell
                          align="right"
                          sx={{ fontWeight: 700, width: 170 }}
                        >
                          Weight
                        </TableCell>
                      </TableRow>
                    </TableHead>

                    <TableBody>
                      {rules.map((rule) => (
                        <TableRow key={rule.id} hover>
                          <TableCell>{rule.id}</TableCell>
                          <TableCell>{rule.title}</TableCell>
                          <TableCell>{rule.description}</TableCell>
                          <TableCell align="right">
                            <TextField
                              type="number"
                              size="small"
                              value={rule.weight}
                              onChange={(event) =>
                                handleWeightChange(rule.id, event.target.value)
                              }
                              slotProps={{
                                htmlInput: {
                                  min: 0,
                                },
                              }}
                              sx={{ width: 120 }}
                            />
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </TableContainer>

                <Box
                  sx={{
                    mt: 2,
                    display: "flex",
                    justifyContent: "space-between",
                    alignItems: "center",
                    gap: 2,
                    flexWrap: "wrap",
                  }}
                >
                  <Typography
                    variant="subtitle1"
                    color={totalWeight === 100 ? "success.main" : "error.main"}
                    sx={{ fontWeight: 700 }}
                  >
                    Total Weight: {totalWeight} / 100
                  </Typography>

                  <Button
                    variant="contained"
                    onClick={handleSave}
                    disabled={saving || rules.length === 0}
                    className="submit-button"
                  >
                    {saving ? "Saving..." : "Save"}
                  </Button>
                </Box>
              </>
            )}
          </Paper>
        </Box>
      </Container>
    </Box>
  );
}

export default Rules;
