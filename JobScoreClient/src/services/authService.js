import api from "../api/axios";

const authService = {
  Me: () => {
    return api.get("/api/Auth/me");
  },
  Register: (firstName, lastName, email, password, passwordConfirm) => {
    return api.post("/api/Auth/register", {
      firstName,
      lastName,
      email,
      password,
      passwordConfirm,
    });
  },
  Login: (email, password) => {
    return api.post("/api/Auth/login", {
      email,
      password,
    });
  },
};

export default authService;
