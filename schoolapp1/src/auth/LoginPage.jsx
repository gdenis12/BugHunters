import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { useAuthStore } from "../store/authStore";

const schema = z.object({
    email: z.string().email(),
    password: z.string().min(6),
});

export default function LoginPage() {
    const login = useAuthStore((state) => state.login);
    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm({
        resolver: zodResolver(schema),
    });

    const onSubmit = async (data) => {
        try {
            await login(data.email, data.password);
        } catch (err) {
            console.error("Помилка авторизації:", err);
            alert("Помилка авторизації: " + (err.response?.data?.message || err.message || "Невідома помилка"));
        }
    };

    return (
        <div className="flex items-center justify-center min-h-screen bg-gray-100">
            <form
                onSubmit={handleSubmit(onSubmit)}
                className="bg-white p-6 rounded shadow-md w-full max-w-sm"
            >
                <h2 className="text-2xl font-bold mb-4 text-center">Вхід</h2>
                <div className="mb-4">
                    <label className="block mb-1">Email</label>
                    <input
                        {...register("email")}
                        type="email"
                        className="w-full border px-3 py-2 rounded"
                    />
                    {errors.email && <p className="text-red-500 text-sm">{errors.email.message}</p>}
                </div>
                <div className="mb-4">
                    <label className="block mb-1">Пароль</label>
                    <input
                        {...register("password")}
                        type="password"
                        className="w-full border px-3 py-2 rounded"
                    />
                    {errors.password && <p className="text-red-500 text-sm">{errors.password.message}</p>}
                </div>
                <button
                    type="submit"
                    className="w-full bg-blue-500 text-white py-2 rounded hover:bg-blue-600"
                >
                    Увійти
                </button>
            </form>
        </div>
    );
}