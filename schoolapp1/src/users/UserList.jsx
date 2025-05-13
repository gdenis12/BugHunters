import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import axios from "../api/axios";
import { useAuthStore } from "../store/authStore";

export default function UserList() {
    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [searchTerm, setSearchTerm] = useState("");
    const [roleFilter, setRoleFilter] = useState("all");
    const navigate = useNavigate();
    const { user } = useAuthStore();

    // Перевіряємо, чи користувач є вчителем
    if (!user || user.role !== "teacher") {
        return (
            <div className="min-h-screen flex items-center justify-center">
                <div className="text-red-500">
                    Доступ заборонено. Тільки вчителі мають доступ до списку користувачів.
                </div>
            </div>
        );
    }

    useEffect(() => {
        fetchUsers();
    }, []);

    const fetchUsers = async () => {
        try {
            setLoading(true);
            const response = await axios.get("/users");
            setUsers(response.data);
            setError("");
        } catch (err) {
            setError(err.response?.data?.message || "Помилка завантаження користувачів");
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (userId) => {
        if (!window.confirm("Ви впевнені, що хочете видалити цього користувача?")) {
            return;
        }

        try {
            await axios.delete(`/users/${userId}`);
            setUsers(users.filter(user => user.id !== userId));
        } catch (err) {
            setError(err.response?.data?.message || "Помилка видалення користувача");
        }
    };

    const filteredUsers = users.filter(user => {
        const matchesSearch = (
            user.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
            user.surname.toLowerCase().includes(searchTerm.toLowerCase()) ||
            user.email.toLowerCase().includes(searchTerm.toLowerCase())
        );
        const matchesRole = roleFilter === "all" || user.role === roleFilter;
        return matchesSearch && matchesRole;
    });

    if (loading) {
        return (
            <div className="min-h-screen flex items-center justify-center">
                <div className="text-gray-600">Завантаження...</div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gray-50 py-8 px-4 sm:px-6 lg:px-8">
            <div className="max-w-7xl mx-auto">
                <div className="sm:flex sm:items-center sm:justify-between mb-8">
                    <div>
                        <h2 className="text-2xl font-bold text-gray-900">Користувачі</h2>
                        <p className="mt-1 text-sm text-gray-500">
                            Список всіх користувачів системи
                        </p>
                    </div>
                    <button
                        onClick={() => navigate("/users/create")}
                        className="mt-4 sm:mt-0 inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                    >
                        Створити користувача
                    </button>
                </div>

                <div className="mb-6 sm:flex sm:items-center sm:justify-between">
                    <div className="sm:flex-1 min-w-0">
                        <div className="max-w-xs">
                            <input
                                type="text"
                                placeholder="Пошук користувачів..."
                                value={searchTerm}
                                onChange={(e) => setSearchTerm(e.target.value)}
                                className="shadow-sm focus:ring-blue-500 focus:border-blue-500 block w-full sm:text-sm border-gray-300 rounded-md"
                            />
                        </div>
                    </div>
                    <div className="mt-4 sm:mt-0 sm:ml-4">
                        <select
                            value={roleFilter}
                            onChange={(e) => setRoleFilter(e.target.value)}
                            className="block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm rounded-md"
                        >
                            <option value="all">Всі ролі</option>
                            <option value="teacher">Вчителі</option>
                            <option value="student">Учні</option>
                            <option value="parent">Батьки</option>
                        </select>
                    </div>
                </div>

                {error && (
                    <div className="mb-4 text-red-500 text-center">
                        {error}
                    </div>
                )}

                <div className="bg-white shadow overflow-hidden sm:rounded-md">
                    <ul className="divide-y divide-gray-200">
                        {filteredUsers.map((user) => (
                            <li key={user.id}>
                                <div className="px-4 py-4 sm:px-6">
                                    <div className="flex items-center justify-between">
                                        <div className="flex-1 min-w-0">
                                            <div className="flex items-center">
                                                <div className="flex-shrink-0 h-10 w-10">
                                                    <div className="h-10 w-10 rounded-full bg-gray-200 flex items-center justify-center">
                                                        <span className="text-gray-500 font-medium">
                                                            {user.name[0]}{user.surname[0]}
                                                        </span>
                                                    </div>
                                                </div>
                                                <div className="ml-4">
                                                    <h3 className="text-sm font-medium text-gray-900">
                                                        {user.name} {user.surname}
                                                    </h3>
                                                    <p className="text-sm text-gray-500">{user.email}</p>
                                                </div>
                                            </div>
                                        </div>
                                        <div className="flex items-center space-x-4">
                                            <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                                                user.role === "teacher" ? "bg-green-100 text-green-800" :
                                                user.role === "student" ? "bg-blue-100 text-blue-800" :
                                                "bg-yellow-100 text-yellow-800"
                                            }`}>
                                                {user.role === "teacher" ? "Вчитель" :
                                                 user.role === "student" ? "Учень" : "Батько/Мати"}
                                            </span>
                                            <button
                                                onClick={() => handleDelete(user.id)}
                                                className="text-red-600 hover:text-red-900"
                                            >
                                                Видалити
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </li>
                        ))}
                        {filteredUsers.length === 0 && (
                            <li className="px-4 py-4 sm:px-6 text-center text-gray-500">
                                Користувачів не знайдено
                            </li>
                        )}
                    </ul>
                </div>
            </div>
        </div>
    );
} 