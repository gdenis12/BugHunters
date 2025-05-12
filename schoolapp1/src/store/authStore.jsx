import { create } from "zustand";
import axios from "../api/axios";

export const useAuthStore = create((set) => ({
    user: null,
    token: null,

    login: async (email, password) => {
        const res = await axios.post("/auth/login", { email, password });
        const { user, token } = res.data;
        localStorage.setItem("token", token);
        set({ user, token });
    },

    logout: () => {
        localStorage.removeItem("token");
        set({ user: null, token: null });
    },

    checkAuth: async () => {
        const token = localStorage.getItem("token");
        if (!token) return;
        try {
            const res = await axios.get("/auth/me", {
                headers: { Authorization: `Bearer ${token}` },
            });
            set({ user: res.data, token });
        } catch {
            localStorage.removeItem("token");
        }
    },
}));