import api from "../api/axios";

const chatService = {
  Create: (title) => {
    return api.post("/api/Chat", title);
  },
  SendMessage: (chatId, content) => {
    return api.post(`/api/Chat/${chatId}/messages`, content);
  },
  GetMessages: (chatId) => {
    return api.get(`/api/Chat/${chatId}/messages`);
  },
  GetOuterUsers: (chatId) => {
    return api.get(`/api/Chat/${chatId}/outer-users`);
  },
  GetInnerUsers: (chatId) => {
    return api.get(`/api/Chat/${chatId}/inner-users`);
  },
  AddUser: (chatId, targetUserId) => {
    return api.post(`/api/Chat/${chatId}/users/${targetUserId}`);
  },
  RemoveUser: (chatId, targetUserId) => {
    return api.delete(`/api/Chat/${chatId}/users/${targetUserId}`);
  },
  SetAdmin: (chatId, targetUserId) => {
    return api.post(`/api/Chat/${chatId}/admins/${targetUserId}`);
  },
  GetMyChatrooms: () => {
    return api.get("/api/Chat/my");
  },
  DeleteChatroom: (chatId) => {
    return api.delete(`/api/Chat/${chatId}`);
  },
};

export default chatService;
