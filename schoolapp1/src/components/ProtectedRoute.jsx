import { Navigate } from "react-router-dom";
import { useAuthStore } from "../store/authStore";

export default function ProtectedRoute({
                                           element,
                                           allowedRoles = [],
                                           redirectTo = "/login"
                                       }) {
    const user = useAuthStore((state) => state.user);

    // Якщо користувач не авторизований
    if (!user) {
        return <Navigate to={redirectTo} replace />;
    }

    // Якщо є обмеження по ролях і роль користувача не входить до дозволених
    if (allowedRoles.length > 0 && !allowedRoles.includes(user.role)) {
        return <Navigate to="/dashboard" replace />;
    }

    // Якщо все ок, рендеримо компонент
    return element;
}