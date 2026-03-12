import axios from "axios";
import tokenService from "../utils/tokenService";

export const BASE_URL = import.meta.env.VITE_API_BASE_URL;

const api = axios.create({
  baseURL: BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

// Request interceptor to add JWT token to headers
api.interceptors.request.use(
  (config) => {
    const token = tokenService.getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  },
);

// Response interceptor to handle token expiration
api.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    if (error.response?.status === 401) {
      // Clear token on 401 Unauthorized
      tokenService.removeToken();

      // Only redirect to login if not already on auth pages
      const currentPath = window.location.pathname;
      if (currentPath !== "/login" && currentPath !== "/register") {
        window.location.href = "/login";
      }
    }
    return Promise.reject(error);
  },
);

export default api;
