import {
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from "@microsoft/signalr";
import { BASE_URL } from "../api/axios";
import tokenService from "../utils/tokenService";

const CHAT_HUB_PATH = "/chathub";

const getHubUrl = () => {
  const fallbackBaseUrl = window.location.origin;
  const baseUrl = (BASE_URL || fallbackBaseUrl).replace(/\/+$/, "");

  return `${baseUrl}${CHAT_HUB_PATH}`;
};

class ChatHubService {
  constructor() {
    this.connection = null;
    this.startPromise = null;
    this.currentRoomId = null;
  }

  getConnection() {
    if (!this.connection) {
      this.connection = new HubConnectionBuilder()
        .withUrl(getHubUrl(), {
          accessTokenFactory: () => tokenService.getToken() || "",
        })
        .withAutomaticReconnect([0, 2000, 5000, 10000])
        .configureLogging(LogLevel.Warning)
        .build();

      this.connection.onreconnected(async () => {
        if (!Number.isFinite(this.currentRoomId)) {
          return;
        }

        try {
          await this.connection.invoke("JoinRoom", this.currentRoomId);
          console.log(`[ChatHub] Rejoined room group: ${this.currentRoomId}`);
        } catch (error) {
          console.error(
            "[ChatHub] Failed to rejoin room after reconnect:",
            error,
          );
        }
      });
    }

    return this.connection;
  }

  async start() {
    const connection = this.getConnection();

    if (connection.state === HubConnectionState.Connected) {
      return connection;
    }

    if (this.startPromise) {
      return this.startPromise;
    }

    this.startPromise = connection
      .start()
      .then(() => connection)
      .finally(() => {
        this.startPromise = null;
      });

    return this.startPromise;
  }

  async stop() {
    if (!this.connection) {
      return;
    }

    if (this.connection.state !== HubConnectionState.Disconnected) {
      await this.connection.stop();
    }
  }

  isConnected() {
    return this.connection?.state === HubConnectionState.Connected;
  }

  async joinRoom(chatId) {
    await this.start();
    const roomId = Number(chatId);
    await this.connection.invoke("JoinRoom", roomId);
    this.currentRoomId = roomId;
  }

  async leaveRoom(chatId) {
    if (
      !this.connection ||
      this.connection.state !== HubConnectionState.Connected
    ) {
      return;
    }

    const roomId = Number(chatId);
    await this.connection.invoke("LeaveRoom", roomId);

    if (this.currentRoomId === roomId) {
      this.currentRoomId = null;
    }
  }

  async sendMessage(chatId, content) {
    await this.start();
    return this.connection.invoke("SendMessage", Number(chatId), content);
  }

  onReceiveMessage(handler) {
    const connection = this.getConnection();
    connection.on("ReceiveMessage", handler);

    return () => {
      connection.off("ReceiveMessage", handler);
    };
  }

  offReceiveMessage(handler) {
    if (!this.connection) {
      return;
    }

    this.connection.off("ReceiveMessage", handler);
  }
}

const chatHubService = new ChatHubService();

export default chatHubService;
