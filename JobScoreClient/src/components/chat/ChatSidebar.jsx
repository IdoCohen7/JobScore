import {
  Box,
  Button,
  CircularProgress,
  IconButton,
  List,
  ListItemButton,
  ListItemText,
  Stack,
  Tooltip,
  Typography,
} from "@mui/material";
import AddRoundedIcon from "@mui/icons-material/AddRounded";
import RefreshRoundedIcon from "@mui/icons-material/RefreshRounded";
import ForumRoundedIcon from "@mui/icons-material/ForumRounded";

function ChatSidebar({
  chatrooms,
  selectedChatId,
  loading,
  error,
  canCreateChatroom,
  onSelect,
  onCreateClick,
  onRefresh,
}) {
  return (
    <Box className="chat-sidebar-shell">
      <Stack direction="row" alignItems="center" justifyContent="space-between">
        <Box>
          <Typography variant="overline" className="chat-overline">
            Workspace
          </Typography>
          <Typography variant="h6" className="chat-sidebar-title">
            Chat Groups
          </Typography>
        </Box>

        <Stack direction="row" spacing={0.5}>
          <Tooltip title="Refresh">
            <span>
              <IconButton size="small" onClick={onRefresh} disabled={loading}>
                <RefreshRoundedIcon fontSize="small" />
              </IconButton>
            </span>
          </Tooltip>
          {canCreateChatroom ? (
            <Tooltip title="Create chatroom">
              <span>
                <IconButton size="small" onClick={onCreateClick}>
                  <AddRoundedIcon fontSize="small" />
                </IconButton>
              </span>
            </Tooltip>
          ) : null}
        </Stack>
      </Stack>

      <Box className="chat-sidebar-list-wrap">
        {loading ? (
          <Box className="chat-sidebar-center-state">
            <CircularProgress size={24} />
          </Box>
        ) : null}

        {!loading && error ? (
          <Box className="chat-sidebar-center-state">
            <Typography variant="body2" color="error" sx={{ mb: 1 }}>
              {error}
            </Typography>
            <Button variant="outlined" size="small" onClick={onRefresh}>
              Try again
            </Button>
          </Box>
        ) : null}

        {!loading && !error && chatrooms.length === 0 ? (
          <Box className="chat-sidebar-center-state">
            <ForumRoundedIcon color="disabled" sx={{ fontSize: 30, mb: 1 }} />
            <Typography variant="body2" color="text.secondary">
              {canCreateChatroom
                ? "No chats yet. Create one to start."
                : "No chats yet."}
            </Typography>
          </Box>
        ) : null}

        {!loading && !error && chatrooms.length > 0 ? (
          <List disablePadding className="chat-room-list">
            {chatrooms.map((chatroom) => {
              const isSelected = selectedChatId === chatroom.id;
              return (
                <ListItemButton
                  key={chatroom.id}
                  selected={isSelected}
                  onClick={() => onSelect(chatroom.id)}
                  className="chat-room-item"
                >
                  <ListItemText
                    primary={chatroom.title || `Chat ${chatroom.id}`}
                    secondary={new Date(
                      chatroom.createdAt,
                    ).toLocaleDateString()}
                    primaryTypographyProps={{
                      fontWeight: isSelected ? 700 : 500,
                      noWrap: true,
                    }}
                    secondaryTypographyProps={{
                      noWrap: true,
                    }}
                  />
                </ListItemButton>
              );
            })}
          </List>
        ) : null}
      </Box>
    </Box>
  );
}

export default ChatSidebar;
