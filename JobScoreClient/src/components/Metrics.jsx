import { useEffect, useMemo, useState } from "react";
import {
  Box,
  Card,
  CardContent,
  CircularProgress,
  Container,
  FormControl,
  InputLabel,
  MenuItem,
  Paper,
  Select,
  Typography,
  useMediaQuery,
  useTheme,
} from "@mui/material";
import { PieChart } from "@mui/x-charts/PieChart";
import { LineChart } from "@mui/x-charts/LineChart";
import NavBar from "./NavBar";
import metricService from "../services/metricService";
import "./components.css";

function Metrics() {
  const [averageScore, setAverageScore] = useState(null);
  const [violations, setViolations] = useState([]);
  const [weeklyTrendline, setWeeklyTrendline] = useState([]);
  const [monthlyTrendline, setMonthlyTrendline] = useState([]);
  const [trendPeriod, setTrendPeriod] = useState("weekly");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("sm"));
  const isTablet = useMediaQuery(theme.breakpoints.down("md"));

  useEffect(() => {
    fetchMetrics();
  }, []);

  const fetchMetrics = async () => {
    setLoading(true);
    setError("");

    try {
      const [avgResponse, violationResponse, weeklyResponse, monthlyResponse] =
        await Promise.all([
          metricService.GetAverageScore(),
          metricService.GetViolationDistribution(),
          metricService.GetWeeklyTrendline(),
          metricService.GetMonthlyTrendline(),
        ]);

      setAverageScore(Number(avgResponse.data ?? 0));
      setViolations(
        Array.isArray(violationResponse.data) ? violationResponse.data : [],
      );
      setWeeklyTrendline(
        Array.isArray(weeklyResponse.data) ? weeklyResponse.data : [],
      );
      setMonthlyTrendline(
        Array.isArray(monthlyResponse.data) ? monthlyResponse.data : [],
      );
    } catch (err) {
      const message =
        err.response?.data?.error ||
        err.response?.data?.message ||
        "Failed to load metrics. Please try again.";
      setError(message);
    } finally {
      setLoading(false);
    }
  };

  const pieData = useMemo(
    () =>
      violations.map((item, index) => ({
        id: index,
        label: item.name,
        value: item.count,
      })),
    [violations],
  );

  const activeTrendline =
    trendPeriod === "weekly" ? weeklyTrendline : monthlyTrendline;
  const trendlineLabels = activeTrendline.map((point) => point.label);
  const trendlineScores = activeTrendline.map((point) =>
    Number(point.averageScore),
  );

  return (
    <Box>
      <NavBar />
      <Container component="main" maxWidth="lg">
        <Box className="page-container">
          <Paper elevation={3} className="content-paper">
            <Typography variant="h5" gutterBottom className="page-title">
              Metrics Dashboard
            </Typography>

            {loading ? (
              <Box className="metrics-loading">
                <CircularProgress />
              </Box>
            ) : error ? (
              <Typography color="error">{error}</Typography>
            ) : (
              <Box className="metrics-grid">
                <Card className="metric-card" elevation={2}>
                  <CardContent>
                    <Typography variant="overline" className="metric-label">
                      Average Score
                    </Typography>
                    <Typography variant="h3" className="metric-value">
                      {averageScore?.toFixed(2) ?? "0.00"}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      Overall score across all submitted job descriptions.
                    </Typography>
                  </CardContent>
                </Card>

                <Paper className="chart-card" elevation={2}>
                  <Typography variant="h6" className="chart-title">
                    Violation Distribution
                  </Typography>
                  {pieData.length > 0 ? (
                    <PieChart
                      height={isMobile ? 260 : 300}
                      series={[
                        {
                          data: pieData,
                          innerRadius: isMobile ? 35 : 45,
                          outerRadius: isMobile ? 90 : 110,
                          paddingAngle: 2,
                          cornerRadius: 4,
                        },
                      ]}
                      slotProps={{
                        legend: {
                          direction: isTablet ? "row" : "column",
                          position: isTablet
                            ? { vertical: "bottom", horizontal: "middle" }
                            : { vertical: "middle", horizontal: "right" },
                        },
                      }}
                    />
                  ) : (
                    <Typography color="text.secondary" className="chart-empty">
                      No violation data available.
                    </Typography>
                  )}
                </Paper>

                <Paper
                  className="chart-card metrics-trendline-card"
                  elevation={2}
                >
                  <Box className="metrics-trendline-header">
                    <Typography variant="h6" className="chart-title">
                      Score Trendline
                    </Typography>
                    <FormControl size="small" className="trendline-select">
                      <InputLabel id="trend-period-label">Period</InputLabel>
                      <Select
                        labelId="trend-period-label"
                        value={trendPeriod}
                        label="Period"
                        onChange={(event) => setTrendPeriod(event.target.value)}
                      >
                        <MenuItem value="weekly">Weekly</MenuItem>
                        <MenuItem value="monthly">Monthly</MenuItem>
                      </Select>
                    </FormControl>
                  </Box>

                  {trendlineScores.length > 0 ? (
                    <LineChart
                      height={isMobile ? 260 : 320}
                      xAxis={[{ scaleType: "point", data: trendlineLabels }]}
                      yAxis={[{ min: 0, max: 100 }]}
                      series={[
                        {
                          id: "avg-score",
                          label: "Average Score",
                          data: trendlineScores,
                          area: true,
                          showMark: true,
                        },
                      ]}
                      margin={{
                        left: isMobile ? 38 : 50,
                        right: isMobile ? 10 : 20,
                        top: 20,
                        bottom: isMobile ? 30 : 40,
                      }}
                    />
                  ) : (
                    <Typography color="text.secondary" className="chart-empty">
                      No trendline data available.
                    </Typography>
                  )}
                </Paper>
              </Box>
            )}
          </Paper>
        </Box>
      </Container>
    </Box>
  );
}

export default Metrics;
