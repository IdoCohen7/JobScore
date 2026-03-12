// Token management utilities

const TOKEN_KEY = "jwt_token";

export const tokenService = {
  // Get token from localStorage
  getToken: () => {
    return localStorage.getItem(TOKEN_KEY);
  },

  // Set token in localStorage
  setToken: (token) => {
    localStorage.setItem(TOKEN_KEY, token);
  },

  // Remove token from localStorage
  removeToken: () => {
    localStorage.removeItem(TOKEN_KEY);
  },

  // Check if user is authenticated (has token)
  isAuthenticated: () => {
    return !!localStorage.getItem(TOKEN_KEY);
  },

  // Decode JWT token to get payload (without verification)
  // Note: This doesn't verify the token, just decodes it
  decodeToken: (token) => {
    try {
      const base64Url = token.split(".")[1];
      const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split("")
          .map((c) => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
          .join(""),
      );
      return JSON.parse(jsonPayload);
    } catch (error) {
      console.error("Error decoding token:", error);
      return null;
    }
  },

  // Check if token is expired
  isTokenExpired: (token) => {
    const decoded = tokenService.decodeToken(token);
    if (!decoded || !decoded.exp) return true;

    const currentTime = Date.now() / 1000;
    return decoded.exp < currentTime;
  },
};

export default tokenService;
