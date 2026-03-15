import { Avatar, Alert, Box, Chip, Paper, Typography } from "@mui/material";

function getScoreToneClass(score) {
  if (score >= 80) return "score-excellent";
  if (score >= 60) return "score-good";
  if (score >= 40) return "score-warning";
  return "score-critical";
}

function EvaluationResult({ result }) {
  if (!result) {
    return null;
  }

  return (
    <Box className="evaluation-side-panel">
      <Paper
        elevation={0}
        className={`evaluation-score-card ${getScoreToneClass(result.score)}`}
      >
        <Box className="evaluation-score-header">
          <Typography variant="overline" className="evaluation-score-label">
            Evaluation Score
          </Typography>
          <Chip
            size="small"
            label={`${result.violations.length} Violation${result.violations.length === 1 ? "" : "s"}`}
            className="evaluation-violation-chip"
          />
        </Box>
        <Box className="evaluation-score-row">
          <Avatar className="evaluation-score-avatar">%</Avatar>
          <Typography variant="h2" className="evaluation-score-value">
            {result.score.toFixed(0)}
          </Typography>
        </Box>
        <Typography variant="body2" className="evaluation-score-hint">
          Score starts at 100 and decreases for each rule violation.
        </Typography>
      </Paper>

      <Paper elevation={0} className="violations-card">
        <Typography variant="h6" className="violations-title">
          Rule Violations
        </Typography>

        {result.violations.length === 0 ? (
          <Alert severity="success" className="violations-empty">
            No violations found. Great job.
          </Alert>
        ) : (
          <Box className="violations-list">
            {result.violations.map((violation) => (
              <Box key={violation.ruleId} className="violation-item">
                <Box>
                  <Typography variant="subtitle1" className="violation-title">
                    {violation.ruleTitle}
                  </Typography>
                  <Typography variant="caption" className="violation-meta">
                    Rule ID: {violation.ruleId}
                  </Typography>
                </Box>
                <Chip
                  size="small"
                  color="error"
                  variant="outlined"
                  label={`-${Number(violation.impact).toFixed(0)} pts`}
                />
              </Box>
            ))}
          </Box>
        )}
      </Paper>
    </Box>
  );
}

export default EvaluationResult;
