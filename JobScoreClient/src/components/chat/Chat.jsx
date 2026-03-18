import { useCallback, useEffect, useMemo, useState } from "react";
import {
  Alert,
  Box,
  Container,
  Drawer,
  IconButton,
  Paper,
  Stack,
  Typography,
  useMediaQuery,
  useTheme,
} from "@mui/material";
import MenuRoundedIcon from "@mui/icons-material/MenuRounded";
import { useSelector } from "react-redux";
import chatService from "../../services/chatService";
import chatHubService from "../../services/chatHubService";
import ChatSidebar from "./ChatSidebar";
import ChatWindow from "./ChatWindow";
import CreateChatroomDialog from "./CreateChatroomDialog";
import ChatParticipantsPanel from "./ChatParticipantsPanel";

const normalizeChatrooms = (payload) => {
  if (!Array.isArray(payload)) {
    return [];
  }

  return payload.map((chatroom) => ({
    id: Number(chatroom?.id),
    title: chatroom?.title || "Untitled chat",
    createdAt: chatroom?.createdAt,
  }));
};

const normalizeMessages = (payload, currentUserName) => {
  if (!Array.isArray(payload)) {
    return [];
  }

  const normalizedCurrentUser = (currentUserName || "").trim().toLowerCase();

  return payload.map((message, index) => {
    const senderName =
      `${message?.firstName || ""} ${message?.lastName || ""}`.trim() ||
      "Unknown user";

    return {
      id: Number(message?.id ?? index + 1),
      senderName,
      content: message?.content || "",
      createdAt: message?.createdAt,
      isOwn:
        normalizedCurrentUser.length > 0 &&
        senderName.toLowerCase() === normalizedCurrentUser,
    };
  });
};

function Chat() {
  const [chatrooms, setChatrooms] = useState([]);
  const [selectedChatId, setSelectedChatId] = useState(null);
  const [messages, setMessages] = useState([]);

  const [roomsLoading, setRoomsLoading] = useState(true);
  const [messagesLoading, setMessagesLoading] = useState(false);
  const [roomsError, setRoomsError] = useState("");
  const [messagesError, setMessagesError] = useState("");

  const [innerUsers, setInnerUsers] = useState([]);
  const [outerUsers, setOuterUsers] = useState([]);
  const [membersLoading, setMembersLoading] = useState(false);
  const [membersError, setMembersError] = useState("");
  const [actingOnUserId, setActingOnUserId] = useState(null);
  const [deletingChatroom, setDeletingChatroom] = useState(false);

  const [createDialogOpen, setCreateDialogOpen] = useState(false);
  const [createError, setCreateError] = useState("");
  const [creatingRoom, setCreatingRoom] = useState(false);

  const [sendingMessage, setSendingMessage] = useState(false);
  const [mobileSidebarOpen, setMobileSidebarOpen] = useState(false);
  const [manageDrawerOpen, setManageDrawerOpen] = useState(false);

  const theme = useTheme();
  const isTabletDown = useMediaQuery(theme.breakpoints.down("md"));
  const user = useSelector((state) => state.user.user);

  const currentUserName =
    `${user?.firstName || ""} ${user?.lastName || ""}`.trim();
  const currentUserId = Number(user?.id);
  const canCreateChatroom = Boolean(user?.isAdmin);

  const selectedChatroom = useMemo(() => {
    return chatrooms.find((chatroom) => chatroom.id === selectedChatId) || null;
  }, [chatrooms, selectedChatId]);

  const loadChatrooms = useCallback(async () => {
    setRoomsLoading(true);
    setRoomsError("");

    try {
      const response = await chatService.GetMyChatrooms();
      const normalized = normalizeChatrooms(response.data);

      setChatrooms(normalized);
      setSelectedChatId((previousChatId) => {
        if (normalized.length === 0) {
          return null;
        }

        const previousStillExists = normalized.some(
          (chatroom) => chatroom.id === previousChatId,
        );

        return previousStillExists ? previousChatId : normalized[0].id;
      });
    } catch (error) {
      setRoomsError(
        error?.response?.data?.error ||
          "Could not load chatrooms. Please try again.",
      );
    } finally {
      setRoomsLoading(false);
    }
  }, []);

  const loadMessages = useCallback(
    async (chatId) => {
      if (!chatId) {
        setMessages([]);
        return;
      }

      setMessagesLoading(true);
      setMessagesError("");

      try {
        const response = await chatService.GetMessages(chatId);
        setMessages(normalizeMessages(response.data, currentUserName));
      } catch (error) {
        setMessagesError(
          error?.response?.data?.error ||
            "Could not load messages. Please try again.",
        );
      } finally {
        setMessagesLoading(false);
      }
    },
    [currentUserName],
  );

  const loadParticipants = useCallback(async (chatId) => {
    if (!chatId) {
      setInnerUsers([]);
      setOuterUsers([]);
      return;
    }

    setMembersLoading(true);
    setMembersError("");

    try {
      const [innerResponse, outerResponse] = await Promise.all([
        chatService.GetInnerUsers(chatId),
        chatService.GetOuterUsers(chatId),
      ]);

      setInnerUsers(
        Array.isArray(innerResponse.data) ? innerResponse.data : [],
      );
      setOuterUsers(
        Array.isArray(outerResponse.data) ? outerResponse.data : [],
      );
    } catch (error) {
      setMembersError(
        error?.response?.data?.error ||
          "Could not load participant data. Please try again.",
      );
    } finally {
      setMembersLoading(false);
    }
  }, []);

  useEffect(() => {
    loadChatrooms();
  }, [loadChatrooms]);

  useEffect(() => {
    loadMessages(selectedChatId);
  }, [selectedChatId, loadMessages]);

  useEffect(() => {
    loadParticipants(selectedChatId);
  }, [selectedChatId, loadParticipants]);

  useEffect(() => {
    const unsubscribe = chatHubService.onReceiveMessage((message) => {
      const incoming = normalizeMessages([message], currentUserName)[0];
      if (!incoming) {
        return;
      }

      setMessages((previous) => {
        const alreadyExists = previous.some(
          (existingMessage) => existingMessage.id === incoming.id,
        );

        if (alreadyExists) {
          return previous;
        }

        return [...previous, incoming];
      });
    });

    return () => {
      unsubscribe();
    };
  }, [currentUserName]);

  useEffect(() => {
    if (!selectedChatId) {
      return;
    }

    let isActive = true;

    const connectToRoom = async () => {
      try {
        await chatHubService.joinRoom(selectedChatId);

        if (!isActive) {
          return;
        }

        console.log(`[ChatHub] Joined room group: ${selectedChatId}`);
      } catch (error) {
        console.error("[ChatHub] Failed to join room:", error);
      }
    };

    connectToRoom();

    return () => {
      isActive = false;
      chatHubService.leaveRoom(selectedChatId).catch((error) => {
        console.error("[ChatHub] Failed to leave room:", error);
      });
    };
  }, [selectedChatId]);

  useEffect(() => {
    return () => {
      chatHubService.stop().catch((error) => {
        console.error("[ChatHub] Failed to stop connection:", error);
      });
    };
  }, []);

  const handleCreateChatroom = async (title) => {
    setCreateError("");
    setCreatingRoom(true);

    try {
      const response = await chatService.Create(title);
      const createdChatId = Number(response?.data?.id);

      await loadChatrooms();

      if (Number.isFinite(createdChatId)) {
        setSelectedChatId(createdChatId);
      }

      setCreateDialogOpen(false);
    } catch (error) {
      setCreateError(
        error?.response?.data?.error ||
          "Could not create chatroom. Please try again.",
      );
    } finally {
      setCreatingRoom(false);
    }
  };

  const handleSendMessage = async (content) => {
    if (!selectedChatId || !content.trim()) {
      return;
    }

    setSendingMessage(true);
    setMessagesError("");

    try {
      await chatHubService.sendMessage(selectedChatId, content.trim());
    } catch (error) {
      setMessagesError(
        error?.response?.data?.error ||
          "Could not send message. Please try again.",
      );
    } finally {
      setSendingMessage(false);
    }
  };

  const isCurrentUserAdmin = useMemo(() => {
    if (!Number.isFinite(currentUserId)) {
      return false;
    }

    return innerUsers.some(
      (member) =>
        Number(member?.id) === currentUserId && Boolean(member?.isAdmin),
    );
  }, [innerUsers, currentUserId]);

  const performMemberAction = async (targetUserId, action) => {
    if (!selectedChatId || !isCurrentUserAdmin) {
      return;
    }

    setMembersError("");
    setActingOnUserId(targetUserId);

    try {
      await action();
      await loadParticipants(selectedChatId);
    } catch (error) {
      setMembersError(
        error?.response?.data?.error ||
          "Could not update participant settings. Please try again.",
      );
    } finally {
      setActingOnUserId(null);
    }
  };

  const handleDeleteChatroom = async () => {
    if (!selectedChatId || !isCurrentUserAdmin) {
      return;
    }

    setDeletingChatroom(true);
    setMembersError("");

    try {
      await chatService.DeleteChatroom(selectedChatId);
      setManageDrawerOpen(false);
      setMessages([]);
      await loadChatrooms();
    } catch (error) {
      setMembersError(
        error?.response?.data?.error ||
          "Could not delete chatroom. Please try again.",
      );
    } finally {
      setDeletingChatroom(false);
    }
  };

  const sidebar = (
    <ChatSidebar
      chatrooms={chatrooms}
      selectedChatId={selectedChatId}
      loading={roomsLoading}
      error={roomsError}
      canCreateChatroom={canCreateChatroom}
      onSelect={(chatId) => {
        setSelectedChatId(chatId);
        if (isTabletDown) {
          setMobileSidebarOpen(false);
        }
      }}
      onCreateClick={() => {
        setCreateError("");
        setCreateDialogOpen(true);
      }}
      onRefresh={loadChatrooms}
    />
  );

  return (
    <Container component="main" maxWidth="xl">
      <Box className="page-container">
        <Paper elevation={3} className="chat-page-shell">
          <Stack
            direction="row"
            alignItems="center"
            justifyContent="space-between"
            sx={{ mb: 2 }}
          >
            <Box>
              <Typography variant="h5" className="page-title" sx={{ mb: 0 }}>
                Team Chat
              </Typography>
            </Box>

            {isTabletDown ? (
              <IconButton
                onClick={() => setMobileSidebarOpen(true)}
                aria-label="Open conversations"
              >
                <MenuRoundedIcon />
              </IconButton>
            ) : null}
          </Stack>

          {roomsError && !roomsLoading ? (
            <Alert severity="error" sx={{ mb: 2 }}>
              {roomsError}
            </Alert>
          ) : null}

          <Box className="chat-layout-grid">
            {!isTabletDown ? (
              <Box className="chat-layout-sidebar">{sidebar}</Box>
            ) : null}

            <Box className="chat-layout-main">
              <ChatWindow
                chatroom={selectedChatroom}
                loading={messagesLoading}
                error={messagesError}
                messages={messages}
                sendingMessage={sendingMessage}
                onRetry={() => loadMessages(selectedChatId)}
                onSendMessage={handleSendMessage}
                onOpenManage={() => setManageDrawerOpen(true)}
              />
            </Box>
          </Box>
        </Paper>
      </Box>

      <Drawer
        anchor="left"
        open={mobileSidebarOpen}
        onClose={() => setMobileSidebarOpen(false)}
        PaperProps={{ sx: { width: "88vw", maxWidth: 360, p: 1.5 } }}
      >
        {sidebar}
      </Drawer>

      <CreateChatroomDialog
        open={createDialogOpen}
        creating={creatingRoom}
        error={createError}
        onClose={() => setCreateDialogOpen(false)}
        onCreate={handleCreateChatroom}
      />

      <Drawer
        anchor="right"
        open={manageDrawerOpen}
        onClose={() => setManageDrawerOpen(false)}
        PaperProps={{ sx: { width: "92vw", maxWidth: 420, p: 1.5 } }}
      >
        <ChatParticipantsPanel
          chatroom={selectedChatroom}
          innerUsers={innerUsers}
          outerUsers={outerUsers}
          loading={membersLoading}
          error={membersError}
          isCurrentUserAdmin={isCurrentUserAdmin}
          actingOnUserId={actingOnUserId}
          deletingChatroom={deletingChatroom}
          onRefresh={() => loadParticipants(selectedChatId)}
          onAddUser={(targetUserId) =>
            performMemberAction(targetUserId, () =>
              chatService.AddUser(selectedChatId, targetUserId),
            )
          }
          onRemoveUser={(targetUserId) =>
            performMemberAction(targetUserId, () =>
              chatService.RemoveUser(selectedChatId, targetUserId),
            )
          }
          onPromoteUser={(targetUserId) =>
            performMemberAction(targetUserId, () =>
              chatService.SetAdmin(selectedChatId, targetUserId),
            )
          }
          onDeleteChatroom={handleDeleteChatroom}
        />
      </Drawer>
    </Container>
  );
}

export default Chat;
