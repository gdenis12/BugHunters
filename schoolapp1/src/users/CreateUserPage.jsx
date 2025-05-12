import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import axios from "../api/axios";
import { useAuthStore } from "../store/authStore";

const schema = z.object({
    email: z.string().email("Введіть коректний email"),
    name: z.string().min(2, "Ім'я має бути не менше 2 символів"),
    surname: z.string().min(2, "Прізвище має бути не менше 2 символів"),
    role: z.enum(["teacher", "student", "parent"], {
        required_error: "Оберіть роль"
    }),
    password: z.string().min(6, "Пароль має бути не менше 6 символів"),
});

export default function CreateUserPage() {
    const navigate = useNavigate();
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);
    const { user } = useAuthStore();

    // Перевіряємо, чи користувач є вчителем
    if (!user || user.role !== "teacher") {
        return (
            <div className="min-h-screen flex items-center justify-center">
                <div className="text-red-500">
                    Доступ заборонено. Тільки вчителі можуть створювати нових користувачів.
                </div>
            </div>
        );
    }

    const {
        register,
        handleSubmit,
        reset,
        formState: { errors },
    } = useForm({
        resolver: zodResolver(schema),
    });

    const onSubmit = async (data) => {
        try {
            setError("");
            setLoading(true);
            await axios.post("/users/create", data);
            reset(); // Очищаємо форму після успішного створення
            alert("Користувача успішно створено!");
        } catch (err) {
            setError(err.response?.data?.message || "Помилка створення користувача");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
            <div className="max-w-md mx-auto">
                <div>
                    <h2 className="mt-6 text-center text-3xl font-extrabold text-gray-900">
                        Створення нового користувача
                    </h2>
                    <p className="mt-2 text-center text-sm text-gray-600">
                        Заповніть форму для створення нового користувача
                    </p>
                </div>

                <form className="mt-8 space-y-6" onSubmit={handleSubmit(onSubmit)}>
                    <div className="rounded-md shadow-sm -space-y-px">
                        <div className="grid grid-cols-2 gap-4 mb-4">
                            <div>
                                <label htmlFor="name" className="block text-sm font-medium text-gray-700">
                                    Ім'я
                                </label>
                                <input
                                    {...register("name")}
                                    type="text"
                                    className="mt-1 block w-full border rounded-md shadow-sm py-2 px-3"
                                />
                                {errors.name && (
                                    <p className="mt-1 text-sm text-red-500">{errors.name.message}</p>
                                )}
                            </div>
                            <div>
                                <label htmlFor="surname" className="block text-sm font-medium text-gray-700">
                                    Прізвище
                                </label>
                                <input
                                    {...register("surname")}
                                    type="text"
                                    className="mt-1 block w-full border rounded-md shadow-sm py-2 px-3"
                                />
                                {errors.surname && (
                                    <p className="mt-1 text-sm text-red-500">{errors.surname.message}</p>
                                )}
                            </div>
                        </div>

                        <div className="mb-4">
                            <label htmlFor="email" className="block text-sm font-medium text-gray-700">
                                Email
                            </label>
                            <input
                                {...register("email")}
                                type="email"
                                className="mt-1 block w-full border rounded-md shadow-sm py-2 px-3"
                            />
                            {errors.email && (
                                <p className="mt-1 text-sm text-red-500">{errors.email.message}</p>
                            )}
                        </div>

                        <div className="mb-4">
                            <label htmlFor="password" className="block text-sm font-medium text-gray-700">
                                Пароль
                            </label>
                            <input
                                {...register("password")}
                                type="password"
                                className="mt-1 block w-full border rounded-md shadow-sm py-2 px-3"
                            />
                            {errors.password && (
                                <p className="mt-1 text-sm text-red-500">{errors.password.message}</p>
                            )}
                        </div>

                        <div className="mb-4">
                            <label htmlFor="role" className="block text-sm font-medium text-gray-700">
                                Роль
                            </label>
                            <select
                                {...register("role")}
                                className="mt-1 block w-full border rounded-md shadow-sm py-2 px-3"
                            >
                                <option value="">Оберіть роль</option>
                                <option value="teacher">Вчитель</option>
                                <option value="student">Учень</option>
                                <option value="parent">Батько/Мати</option>
                            </select>
                            {errors.role && (
                                <p className="mt-1 text-sm text-red-500">{errors.role.message}</p>
                            )}
                        </div>
                    </div>

                    {error && (
                        <div className="text-red-500 text-sm text-center">
                            {error}
                        </div>
                    )}

                    <div className="flex items-center justify-between">
                        <button
                            type="button"
                            onClick={() => navigate("/users")}
                            className="text-sm text-blue-600 hover:text-blue-500"
                        >
                            Повернутися до списку користувачів
                        </button>
                        <button
                            type="submit"
                            disabled={loading}
                            className={`group relative flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 ${
                                loading ? "opacity-50 cursor-not-allowed" : ""
                            }`}
                        >
                            {loading ? "Створення..." : "Створити користувача"}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
} 