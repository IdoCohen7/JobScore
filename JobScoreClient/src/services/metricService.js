import api from "../api/axios";

const metricService = {
  GetAverageScore: () => {
    return api.get("/api/Metric/average");
  },
  GetViolationDistribution: () => {
    return api.get("/api/Metric/violations");
  },
  GetWeeklyTrendline: () => {
    return api.get("/api/Metric/trendline/weekly");
  },
  GetMonthlyTrendline: () => {
    return api.get("/api/Metric/trendline/monthly");
  },
};

export default metricService;
