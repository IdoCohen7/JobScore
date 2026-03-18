import {
  Alert,
  Box,
  Button,
  Chip,
  CircularProgress,
  Divider,
  List,
  ListItem,
  ListItemText,
  Stack,
  Typography,
} from "@mui/material";

const getFullName = (item) => {
  const first = item?.firstName || item?.firsName || "";
  const last = item?.lastName || "";
  return `${first} ${last}`.trim() || "Unknown user";
};

function ChatParticipantsPanel({
  chatroom,
  innerUsers,
  outerUsers,
  loading,
  error,
  isCurrentUserAdmin,
  actingOnUserId,
  deletingChatroom,
  onRefresh,
  onAddUser,
  onRemoveUser,
  onPromoteUser,
  onDeleteChatroom,
}) {
  return (
    <Box className="chat-manage-panel">
      <Stack spacing={0.5} sx={{ mb: 1 }}>
        <Typography variant="overline" className="chat-overline">
          Room Management
        </Typography>
        <Typography variant="h6" className="chat-sidebar-title" noWrap>
          {chatroom?.title || "Conversation"}
        </Typography>
      </Stack>

      <Stack direction="row" spacing={1} sx={{ mb: 1.5 }}>
        <Button
          size="small"
          variant="outlined"
          onClick={onRefresh}
          disabled={loading}
        >
          Refresh
        </Button>
        <Button
          size="small"
          variant="outlined"
          color="error"
          onClick={onDeleteChatroom}
          disabled={!isCurrentUserAdmin || deletingChatroom || !chatroom?.id}
        >
          {deletingChatroom ? "Deleting..." : "Delete room"}
        </Button>
      </Stack>

      {error ? (
        <Alert severity="error" sx={{ mb: 1.5 }}>
          {error}
        </Alert>
      ) : null}

      {loading ? (
        <Box className="chat-sidebar-center-state" sx={{ minHeight: 120 }}>
          <CircularProgress size={24} />
        </Box>
      ) : null}

      {!loading ? (
        <>
          <Typography variant="subtitle2" sx={{ mb: 0.5 }}>
            Members
          </Typography>
          <List dense className="chat-manage-list">
            {innerUsers.map((member) => {
              const displayName = getFullName(member);
              const isBusy = actingOnUserId === member.id;

              return (
                <ListItem
                  key={member.id}
                  className="chat-manage-list-item"
                  secondaryAction={
                    isCurrentUserAdmin ? (
                      <Stack direction="row" spacing={0.5}>
                        <Button
                          size="small"
                          variant="text"
                          onClick={() => onPromoteUser(member.id)}
                          disabled={member.isAdmin || isBusy}
                        >
                          Admin
                        </Button>
                        <Button
                          size="small"
                          color="error"
                          variant="text"
                          onClick={() => onRemoveUser(member.id)}
                          disabled={isBusy}
                        >
                          Remove
                        </Button>
                      </Stack>
                    ) : null
                  }
                >
                  <ListItemText
                    primary={
                      <Stack direction="row" spacing={0.8} alignItems="center">
                        <Typography variant="body2" sx={{ fontWeight: 600 }}>
                          {displayName}
                        </Typography>
                        {member.isAdmin ? (
                          <Chip size="small" label="Admin" />
                        ) : null}
                      </Stack>
                    }
                  />
                </ListItem>
              );
            })}
            {innerUsers.length === 0 ? (
              <ListItem>
                <ListItemText primary="No members found." />
              </ListItem>
            ) : null}
          </List>

          <Divider sx={{ my: 1.2 }} />

          <Typography variant="subtitle2" sx={{ mb: 0.5 }}>
            Available users
          </Typography>
          <List dense className="chat-manage-list">
            {outerUsers.map((candidate) => {
              const displayName = getFullName(candidate);
              const isBusy = actingOnUserId === candidate.id;

              return (
                <ListItem
                  key={candidate.id}
                  className="chat-manage-list-item"
                  secondaryAction={
                    isCurrentUserAdmin ? (
                      <Button
                        size="small"
                        variant="contained"
                        onClick={() => onAddUser(candidate.id)}
                        disabled={isBusy}
                      >
                        Add
                      </Button>
                    ) : null
                  }
                >
                  <ListItemText primary={displayName} />
                </ListItem>
              );
            })}
            {outerUsers.length === 0 ? (
              <ListItem>
                <ListItemText primary="No users to add." />
              </ListItem>
            ) : null}
          </List>

          {!isCurrentUserAdmin ? (
            <Alert severity="info" sx={{ mt: 1.2 }}>
              You can view participants, but only room admins can manage
              members.
            </Alert>
          ) : null}
        </>
      ) : null}
    </Box>
  );
}

export default ChatParticipantsPanel;
