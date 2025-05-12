import { useState, useEffect } from "react";
import axios from "../api/axios";

export default function TasksList() {
    const [tasks, setTasks] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        loadTasks();
    }, []);

    const loadTasks = async () => {
        try {
            setLoading(true);
            const today = new Date();
            today.setHours(0, 0, 0, 0);

            const response = await axios.get("/tasks", {
                params: {
                    date: today.toISOString(),
                },
            });

            setTasks(response.data);
        } catch (err) {
            setError("Помилка завантаження завдань");
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const toggleTaskCompletion = async (taskId, isCompleted) => {
        try {
            await axios.post(`/tasks/${taskId}/completion`, {
                isCompleted,
            });

            // Оновлюємо стан локально
            setTasks(tasks.map(task =>
                task.id === taskId
                    ? { ...task, isCompleted, completedAt: isCompleted ? new Date().toISOString() : null }
                    : task
            ));
        } catch (err) {
            console.error("Помилка при оновленні статусу завдання:", err);
        }
    };

    if (loading) {
        return <div className="text-center py-4">Завантаження...</div>;
    }

    if (error) {
        return <div className="text-center py-4 text-red-500">{error}</div>;
    }

    if (tasks.length === 0) {
        return (
            <div className="text-center py-4 text-gray-500">
                На сьогодні завдань немає
            </div>
        );
    }

    return (
        <div className="space-y-2">
            {tasks.map((task) => (
                <div
                    key={task.id}
                    className={`p-3 rounded border ${
                        task.isCompleted
                            ? "bg-green-50 border-green-200"
                            : "bg-white border-gray-200"
                    }`}
                >
                    <div className="flex items-start gap-2">
                        <input
                            type="checkbox"
                            checked={task.isCompleted}
                            onChange={(e) =>
                                toggleTaskCompletion(task.id, e.target.checked)
                            }
                            className="mt-1"
                        />
                        <div className="flex-1">
                            <h3
                                className={`font-medium ${
                                    task.isCompleted
                                        ? "text-green-800 line-through"
                                        : "text-gray-800"
                                }`}
                            >
                                {task.name}
                            </h3>
                            {task.content && (
                                <p
                                    className={`text-sm mt-1 ${
                                        task.isCompleted
                                            ? "text-green-600"
                                            : "text-gray-600"
                                    }`}
                                >
                                    {task.content}
                                </p>
                            )}
                            <div className="text-xs text-gray-500 mt-1">
                                Дедлайн:{" "}
                                {new Date(
                                    task.dateOfEnding
                                ).toLocaleDateString("uk-UA", {
                                    day: "numeric",
                                    month: "long",
                                    hour: "2-digit",
                                    minute: "2-digit",
                                })}
                            </div>
                        </div>
                    </div>
                </div>
            ))}
        </div>
    );
} 