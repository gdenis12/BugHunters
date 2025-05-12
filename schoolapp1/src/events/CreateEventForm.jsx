import { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import axios from "../api/axios";
import { useAuthStore } from "../store/authStore";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

const schema = z.object({
    title: z.string().min(3, "Назва має бути не менше 3 символів"),
    description: z.string().min(10, "Опис має бути не менше 10 символів"),
    eventType: z.enum(["lesson", "meeting", "personal"], {
        required_error: "Оберіть тип події"
    }),
    startDate: z.date({
        required_error: "Оберіть дату початку"
    }),
    endDate: z.date({
        required_error: "Оберіть дату завершення"
    }),
    participants: z.array(z.string()).optional(),
    tasks: z.array(z.object({
        title: z.string().min(3, "Назва завдання має бути не менше 3 символів"),
        description: z.string().optional()
    })).optional()
}).refine(data => data.endDate >= data.startDate, {
    message: "Дата завершення має бути пізніше дати початку",
    path: ["endDate"]
});

export default function CreateEventForm({ onSuccess, initialDate }) {
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");
    const [users, setUsers] = useState([]);
    const [tasks, setTasks] = useState([]);
    const { user } = useAuthStore();

    const {
        register,
        handleSubmit,
        setValue,
        watch,
        reset,
        formState: { errors }
    } = useForm({
        resolver: zodResolver(schema),
        defaultValues: {
            startDate: initialDate || new Date(),
            endDate: initialDate || new Date(),
            eventType: "lesson",
            participants: [],
            tasks: []
        }
    });

    // Спостерігаємо за типом події для умовного рендерингу
    const eventType = watch("eventType");
    const startDate = watch("startDate");
    const endDate = watch("endDate");

    useEffect(() => {
        // Завантажуємо список користувачів для вибору учасників
        const fetchUsers = async () => {
            try {
                const response = await axios.get("/users");
                setUsers(response.data);
            } catch (err) {
                setError("Помилка завантаження користувачів");
            }
        };
        fetchUsers();
    }, []);

    const onSubmit = async (data) => {
        try {
            setLoading(true);
            setError("");
            
            // Додаємо завдання до даних події
            const eventData = {
                ...data,
                tasks: tasks.length > 0 ? tasks : undefined,
                createdBy: user.id
            };

            await axios.post("/events", eventData);
            reset();
            setTasks([]);
            if (onSuccess) onSuccess();
        } catch (err) {
            setError(err.response?.data?.message || "Помилка створення події");
        } finally {
            setLoading(false);
        }
    };

    const addTask = () => {
        setTasks([...tasks, { title: "", description: "" }]);
    };

    const removeTask = (index) => {
        setTasks(tasks.filter((_, i) => i !== index));
    };

    const updateTask = (index, field, value) => {
        const newTasks = [...tasks];
        newTasks[index] = { ...newTasks[index], [field]: value };
        setTasks(newTasks);
    };

    return (
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
            <div>
                <label className="block text-sm font-medium text-gray-700">
                    Назва події
                </label>
                <input
                    type="text"
                    {...register("title")}
                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                />
                {errors.title && (
                    <p className="mt-1 text-sm text-red-500">{errors.title.message}</p>
                )}
            </div>

            <div>
                <label className="block text-sm font-medium text-gray-700">
                    Опис
                </label>
                <textarea
                    {...register("description")}
                    rows={3}
                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                />
                {errors.description && (
                    <p className="mt-1 text-sm text-red-500">{errors.description.message}</p>
                )}
            </div>

            <div>
                <label className="block text-sm font-medium text-gray-700">
                    Тип події
                </label>
                <select
                    {...register("eventType")}
                    className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                >
                    <option value="lesson">Урок</option>
                    <option value="meeting">Зустріч</option>
                    <option value="personal">Особиста подія</option>
                </select>
                {errors.eventType && (
                    <p className="mt-1 text-sm text-red-500">{errors.eventType.message}</p>
                )}
            </div>

            <div className="grid grid-cols-2 gap-4">
                <div>
                    <label className="block text-sm font-medium text-gray-700">
                        Дата початку
                    </label>
                    <DatePicker
                        selected={startDate}
                        onChange={(date) => setValue("startDate", date)}
                        showTimeSelect
                        dateFormat="Pp"
                        className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                    />
                    {errors.startDate && (
                        <p className="mt-1 text-sm text-red-500">{errors.startDate.message}</p>
                    )}
                </div>

                <div>
                    <label className="block text-sm font-medium text-gray-700">
                        Дата завершення
                    </label>
                    <DatePicker
                        selected={endDate}
                        onChange={(date) => setValue("endDate", date)}
                        showTimeSelect
                        dateFormat="Pp"
                        className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                    />
                    {errors.endDate && (
                        <p className="mt-1 text-sm text-red-500">{errors.endDate.message}</p>
                    )}
                </div>
            </div>

            {eventType !== "personal" && (
                <div>
                    <label className="block text-sm font-medium text-gray-700">
                        Учасники
                    </label>
                    <select
                        multiple
                        {...register("participants")}
                        className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                    >
                        {users.map((user) => (
                            <option key={user.id} value={user.id}>
                                {user.name} {user.surname} ({user.role})
                            </option>
                        ))}
                    </select>
                    {errors.participants && (
                        <p className="mt-1 text-sm text-red-500">{errors.participants.message}</p>
                    )}
                </div>
            )}

            {(eventType === "lesson" || eventType === "meeting") && (
                <div>
                    <div className="flex justify-between items-center mb-2">
                        <label className="block text-sm font-medium text-gray-700">
                            Завдання
                        </label>
                        <button
                            type="button"
                            onClick={addTask}
                            className="inline-flex items-center px-3 py-1 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700"
                        >
                            Додати завдання
                        </button>
                    </div>
                    {tasks.map((task, index) => (
                        <div key={index} className="mb-4 p-4 border rounded-md">
                            <div className="flex justify-between mb-2">
                                <input
                                    type="text"
                                    value={task.title}
                                    onChange={(e) => updateTask(index, "title", e.target.value)}
                                    placeholder="Назва завдання"
                                    className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                                />
                                <button
                                    type="button"
                                    onClick={() => removeTask(index)}
                                    className="ml-2 text-red-600 hover:text-red-900"
                                >
                                    Видалити
                                </button>
                            </div>
                            <textarea
                                value={task.description}
                                onChange={(e) => updateTask(index, "description", e.target.value)}
                                placeholder="Опис завдання"
                                rows={2}
                                className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                            />
                        </div>
                    ))}
                </div>
            )}

            {error && (
                <div className="text-red-500 text-sm text-center">
                    {error}
                </div>
            )}

            <div className="flex justify-end">
                <button
                    type="submit"
                    disabled={loading}
                    className={`inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 ${
                        loading ? "opacity-50 cursor-not-allowed" : ""
                    }`}
                >
                    {loading ? "Створення..." : "Створити подію"}
                </button>
            </div>
        </form>
    );
} 