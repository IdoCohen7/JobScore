import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import authService from "../../services/authService";
import tokenService from "../../utils/tokenService";

const initialState = {
  user: null,
  isAuthenticated: false,
  authInitialized: false,
  loading: false,
  error: null,
};

export const initializeAuth = createAsyncThunk(
  "user/initializeAuth",
  async (_, { rejectWithValue }) => {
    const token = tokenService.getToken();

    if (!token || tokenService.isTokenExpired(token)) {
      tokenService.removeToken();
      return null;
    }

    try {
      const response = await authService.Me();
      return response.data;
    } catch {
      tokenService.removeToken();
      return rejectWithValue("Could not restore session.");
    }
  },
);

const userSlice = createSlice({
  name: "user",
  initialState,
  reducers: {
    setUser: (state, action) => {
      state.user = action.payload;
      state.isAuthenticated = true;
      state.authInitialized = true;
      state.error = null;
    },
    clearUser: (state) => {
      state.user = null;
      state.isAuthenticated = false;
      state.authInitialized = true;
      state.error = null;
    },
    setLoading: (state, action) => {
      state.loading = action.payload;
    },
    setError: (state, action) => {
      state.error = action.payload;
      state.loading = false;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(initializeAuth.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(initializeAuth.fulfilled, (state, action) => {
        state.loading = false;
        state.authInitialized = true;
        state.error = null;

        if (action.payload) {
          state.user = action.payload;
          state.isAuthenticated = true;
        } else {
          state.user = null;
          state.isAuthenticated = false;
        }
      })
      .addCase(initializeAuth.rejected, (state, action) => {
        state.loading = false;
        state.authInitialized = true;
        state.user = null;
        state.isAuthenticated = false;
        state.error = action.payload || null;
      });
  },
});

export const { setUser, clearUser, setLoading, setError } = userSlice.actions;

export default userSlice.reducer;
