import { Box, Container, Paper, Typography } from "@mui/material";
import NavBar from "./NavBar";
import "./components.css";

function Metrics() {
  return (
    <Box>
      <NavBar />
      <Container component="main" maxWidth="lg">
        <Box className="page-container">
          <Paper elevation={3} className="content-paper">
            <Typography variant="h5" gutterBottom>
              Metrics Dashboard
            </Typography>
            <Typography color="text.secondary">
              Metrics content coming soon...
            </Typography>
          </Paper>
        </Box>
      </Container>
    </Box>
  );
}

export default Metrics;
