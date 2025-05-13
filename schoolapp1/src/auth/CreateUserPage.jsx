import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useState } from "react";
import axios from "../api/axios";

const userSchema = z.object({
    name: z.string().min(2, "Ім'я має містити щонайменше 2 символи"),
    surname: z.string().min(2, "Прізвище має містити щонайменше 2 символи"),
    phone: z.string().min(6, "Номер телефону має бути валідним"),
    email: z.string().email("Невалідний email"),
    password: z.string().min(6, "Пароль має бути щонайменше 6 символів"),
    role: z.enum(["teacher", "student", "parent"]),
    birthDay: z.string().optional(),
    groupId: z.number().optional(),
    parentTypeId: z.number().optional(),
});

export default function CreateUserPage() {
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
    const [success, setSuccess] = useState(false);

    const {
        register,
        handleSubmit,
        watch,
        reset,
        formState: { errors },
    } = useForm({
        resolver: zodResolver(userSchema),
    });

    const selectedRole = watch("role");

    const onSubmit = async (data) => {
        setLoading(true);
        setError(null);
        setSuccess(false);

        try {
            const response = await axios.post("/auth/create-user", data);
            setSuccess(true);
            reset(); // Очищаємо форму після успішного створення
        } catch (err) {
            setError(err.response?.data?.message || "Помилка при створенні користувача");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="max-w-md mx-auto mt-10 p-6 bg-white rounded-lg shadow-md">
            <h2 className="text-2xl font-bold mb-6">Створення нового користувача</h2>

            {success && (
                <div className="mb-4 p-2 bg-green-100 text-green-700 rounded">
                    Користувача успішно створено!
                </div>
            )}

            {error && (
                <div className="mb-4 p-2 bg-red-100 text-red-700 rounded">
                    {error}
                </div>
            )}

            <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
                <div>
                    <label className="block mb-1">Ім'я</label>
                    <input
                        {...register("name")}
                        className="w-full border rounded p-2"
                    />
                    {errors.name && (
                        <p className="text-red-500 text-sm">{errors.name.message}</p>
                    )}
                </div>

                <div>
                    <label className="block mb-1">Прізвище</label>
                    <input
                        {...register("surname")}
                        className="w-full border rounded p-2"
                    />
                    {errors.surname && (
                        <p className="text-red-500 text-sm">{errors.surname.message}</p>
                    )}
                </div>

                <div>
                    <label className="block mb-1">Телефон</label>
                    <input
                        {...register("phone")}
                        className="w-full border rounded p-2"
                    />
                    {errors.phone && (
                        <p className="text-red-500 text-sm">{errors.phone.message}</p>
                    )}
                </div>

                <div>
                    <label className="block mb-1">Email</label>
                    <input
                        {...register("email")}
                        type="email"
                        className="w-full border rounded p-2"
                    />
                    {errors.email && (
                        <p className="text-red-500 text-sm">{errors.email.message}</p>
                    )}
                </div>

                <div>
                    <label className="block mb-1">Пароль</label>
                    <input
                        {...register("password")}
                        type="password"
                        className="w-full border rounded p-2"
                    />
                    {errors.password && (
                        <p className="text-red-500 text-sm">{errors.password.message}</p>
                    )}
                </div>

                <div>
                    <label className="block mb-1">Роль</label>
                    <select
                        {...register("role")}
                        className="w-full border rounded p-2"
                    >
                        <option value="">-- Оберіть роль --</option>
                        <option value="teacher">Викладач</option>
                        <option value="student">Учень</option>
                        <option value="parent">Батьки</option>
                    </select>
                    {errors.role && (
                        <p className="text-red-500 text-sm">{errors.role.message}</p>
                    )}
                </div>

                {selectedRole === "student" && (
                    <>
                        <div>
                            <label className="block mb-1">Дата народження</label>
                            <input
                                {...register("birthDay")}
                                type="date"
                                className="w-full border rounded p-2"
                            />
                        </div>
                        <div>
                            <label className="block mb-1">Група</label>
                            <input
                                {...register("groupId")}
                                type="number"
                                className="w-full border rounded p-2"
                            />
                        </div>
                    </>
                )}

                {selectedRole === "parent" && (
                    <div>
                        <label className="block mb-1">Тип батьківства</label>
                        <select
                            {...register("parentTypeId")}
                            className="w-full border rounded p-2"
                        >
                            <option value="">-- Оберіть тип --</option>
                            <option value="1">Мати</option>
                            <option value="2">Батько</option>
                            <option value="3">Опікун</option>
                        </select>
                    </div>
                )}

                <button
                    type="submit"
                    disabled={loading}
                    className="w-full bg-blue-500 text-white py-2 px-4 rounded hover:bg-blue-600 disabled:bg-blue-300"
                >
                    {loading ? "Створення..." : "Створити користувача"}
                </button>
            </form>
        </div>
    );
}