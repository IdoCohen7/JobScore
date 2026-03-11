import { Box } from "@mui/material";

function LogoHeader() {
  return (
    <Box
      component="img"
      src="/logo.png"
      alt="Logo"
      sx={{
        width: { xs: 100, sm: 120, md: 150 },
        height: { xs: 100, sm: 120, md: 150 },
        mb: { xs: 2, sm: 3 },
        objectFit: "contain",
      }}
    />
  );
}

export default LogoHeader;
