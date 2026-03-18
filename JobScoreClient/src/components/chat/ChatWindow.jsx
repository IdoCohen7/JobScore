import {
  Avatar,
  Box,
  Button,
  CircularProgress,
  Paper,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import SendRoundedIcon from "@mui/icons-material/SendRounded";
import ChatBubbleOutlineRoundedIcon from "@mui/icons-material/ChatBubbleOutlineRounded";
import GroupRoundedIcon from "@mui/icons-material/GroupRounded";
import { useEffect, useMemo, useRef, useState } from "react";

const formatTimestamp = (value) => {
  if (!value) {
    return "";
  }

  const parsed = new Date(value);
  if (Number.isNaN(parsed.getTime())) {
    return "";
  }

  return parsed.toLocaleString([], {
    month: "short",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
};

function ChatWindow({
  chatroom,
  loading,
  error,
  messages,
  sendingMessage,
  onRetry,
  onSendMessage,
  onOpenManage,
}) {
  const [draft, setDraft] = useState("");
  const messagesContainerRef = useRef(null);

  const title = chatroom?.title || "Select a conversation";
  const canSend = Boolean(chatroom?.id && draft.trim()) && !sendingMessage;

  const subtitle = useMemo(() => {
    if (!chatroom?.id) {
      return "Choose a room from the left to begin chatting.";
    }

    return `${messages.length} message${messages.length === 1 ? "" : "s"}`;
  }, [chatroom, messages.length]);

  useEffect(() => {
    const container = messagesContainerRef.current;
    if (!container) {
      return;
    }

    container.scrollTo({
      top: container.scrollHeight,
      behavior: "smooth",
    });
  }, [messages, loading]);

  const handleSubmit = () => {
    if (!canSend) {
      return;
    }

    onSendMessage(draft.trim());
    setDraft("");
  };

  return (
    <Paper elevation={0} className="chat-main-shell">
      <Box className="chat-main-header">
        <Stack
          direction="row"
          alignItems="center"
          justifyContent="space-between"
          spacing={1}
        >
          <Box sx={{ minWidth: 0 }}>
            <Typography variant="h6" className="chat-main-title" noWrap>
              {title}
            </Typography>
            <Typography variant="body2" color="text.secondary" noWrap>
              {subtitle}
            </Typography>
          </Box>

          <Button
            variant="outlined"
            size="small"
            startIcon={<GroupRoundedIcon />}
            onClick={onOpenManage}
            disabled={!chatroom?.id}
          >
            Manage
          </Button>
        </Stack>
      </Box>

      <Box className="chat-main-messages" ref={messagesContainerRef}>
        {!chatroom?.id ? (
          <Box className="chat-main-empty-state">
            <ChatBubbleOutlineRoundedIcon
              color="disabled"
              sx={{ fontSize: 42, mb: 1 }}
            />
            <Typography variant="body1">No chat selected</Typography>
            <Typography variant="body2" color="text.secondary">
              Select a conversation to see messages.
            </Typography>
          </Box>
        ) : null}

        {chatroom?.id && loading ? (
          <Box className="chat-main-empty-state">
            <CircularProgress size={26} />
          </Box>
        ) : null}

        {chatroom?.id && !loading && error ? (
          <Box className="chat-main-empty-state">
            <Typography variant="body2" color="error" sx={{ mb: 1 }}>
              {error}
            </Typography>
            <Button variant="outlined" size="small" onClick={onRetry}>
              Reload messages
            </Button>
          </Box>
        ) : null}

        {chatroom?.id && !loading && !error && messages.length === 0 ? (
          <Box className="chat-main-empty-state">
            <Typography variant="body1">No messages yet</Typography>
            <Typography variant="body2" color="text.secondary">
              Start the conversation with your first message.
            </Typography>
          </Box>
        ) : null}

        {chatroom?.id && !loading && !error && messages.length > 0 ? (
          <Stack spacing={1.5}>
            {messages.map((message) => {
              const initials = (message.senderName || "U")
                .split(" ")
                .map((part) => part[0])
                .join("")
                .slice(0, 2)
                .toUpperCase();

              return (
                <Box
                  key={message.id}
                  className={`chat-message-row ${message.isOwn ? "chat-message-own" : ""}`}
                >
                  {!message.isOwn && (
                    <Avatar className="chat-message-avatar">{initials}</Avatar>
                  )}

                  <Box className="chat-message-bubble">
                    <Typography
                      variant="caption"
                      className="chat-message-author"
                    >
                      {message.senderName || "Unknown"}
                    </Typography>
                    <Typography
                      variant="body2"
                      className="chat-message-content"
                    >
                      {message.content || ""}
                    </Typography>
                    <Typography variant="caption" className="chat-message-time">
                      {formatTimestamp(message.createdAt)}
                    </Typography>
                  </Box>
                </Box>
              );
            })}
          </Stack>
        ) : null}
      </Box>

      <Box className="chat-main-composer">
        <TextField
          size="small"
          placeholder={
            chatroom?.id
              ? "Write your message..."
              : "Select a chat to start typing"
          }
          fullWidth
          value={draft}
          onChange={(event) => setDraft(event.target.value)}
          onKeyDown={(event) => {
            if (event.key === "Enter" && !event.shiftKey) {
              event.preventDefault();
              handleSubmit();
            }
          }}
          disabled={!chatroom?.id || sendingMessage}
        />
        <Button
          variant="contained"
          onClick={handleSubmit}
          disabled={!canSend}
          startIcon={<SendRoundedIcon />}
        >
          Send
        </Button>
      </Box>
    </Paper>
  );
}

export default ChatWindow;
