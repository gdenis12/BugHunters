import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import { useEffect } from "react";
import { useAuthStore } from "./store/authStore";
import LoginPage from "./auth/LoginPage";
import RegisterPage from "./auth/RegisterPage";
import Dashboard from "./components/Dashboard";
import CalendarView from "./calendar/CalendarView";
import EventsPage from "./events/EventsPage";
import TasksPage from "./tasks/TasksPage";
import ProfilePage from "./profile/ProfilePage";
import ProtectedRoute from "./components/ProtectedRoute";
import CreateUserPage from "./auth/CreateUserPage";
import CreateEventPage from "./events/CreateEventPage";

export default function App() {
    const user = useAuthStore((state) => state.user);
    const checkAuth = useAuthStore((state) => state.checkAuth);

    useEffect(() => {
        checkAuth();
    }, []); // ⬅️ Порожній масив залежностей = лише 1 раз при завантаженні

    return (
        <Router>
            <Routes>
                {/* Публічні роути */}
                <Route
                    path="/"
                    element={user ? <Navigate to="/dashboard" /> : <LoginPage />}
                />
                <Route
                    path="/register"
                    element={user ? <Navigate to="/dashboard" /> : <RegisterPage />}
                />

                {/* Захищені роути */}
                <Route
                    path="/dashboard"
                    element={<ProtectedRoute element={<Dashboard />} />}
                />
                <Route
                    path="/calendar"
                    element={<ProtectedRoute element={<CalendarView />} />}
                />
                <Route
                    path="/events"
                    element={<ProtectedRoute element={<EventsPage />} />}
                />
                <Route
                    path="/tasks"
                    element={<ProtectedRoute element={<TasksPage />} />}
                />
                <Route
                    path="/profile/:id"
                    element={<ProtectedRoute element={<ProfilePage />} />}
                />

                {/* Роути тільки для вчителів */}
                <Route
                    path="/create-user"
                    element={
                        <ProtectedRoute
                            element={<CreateUserPage />}
                            allowedRoles={["teacher"]}
                        />
                    }
                />

                {/* Редірект для неіснуючих роутів */}
                <Route path="*" element={<Navigate to="/" replace />} />

                {/* Додаємо новий маршрут для CreateEventPage */}
                <Route path="/events/create" element={<CreateEventPage />} />
            </Routes>
        </Router>
    );
}
