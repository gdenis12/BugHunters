import axios from "axios";

const instance = axios.create({
    baseURL: "https://localhost:7014/api",
    headers: {
        "Content-Type": "application/json",
    },
    withCredentials: true
});

instance.interceptors.request.use((config) => {
    const token = localStorage.getItem("token");
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
}, (error) => {
    return Promise.reject(error);
});

// Додаємо обробку помилок
instance.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response?.status === 401) {
            // Якщо отримали 401 - токен недійсний або відсутній
            localStorage.removeItem("token");
            window.location.href = "/login";
        }
        console.error('API Error:', error.response?.data || error.message);
        return Promise.reject(error);
    }
);

export default instance;
