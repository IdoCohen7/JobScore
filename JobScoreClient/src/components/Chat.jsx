import { Box, Container, Paper, Typography } from "@mui/material";

function Chat() {
  return (
    <Container component="main" maxWidth="md">
      <Box className="page-container">
        <Paper elevation={3} className="content-paper">
          <Typography variant="h5" gutterBottom className="page-title">
            Chat
          </Typography>
          <Typography variant="body1" color="text.secondary">
            Chat functionality will be implemented soon.
          </Typography>
        </Paper>
      </Box>
    </Container>
  );
}

export default Chat;
