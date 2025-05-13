import { useState } from "react";
import { useAuthStore } from "../store/authStore";
import Calendar from "../calendar/Calendar";
import TasksList from "../tasks/TasksList";

export default function Dashboard() {
    const user = useAuthStore((state) => state.user);
    const [viewMode, setViewMode] = useState("month"); // month, week, day, year

    return (
        <div className="container mx-auto px-4 py-8">
            <div className="flex justify-between items-center mb-6">
                <h1 className="text-2xl font-bold">
                    Вітаємо, {user?.name} {user?.surname}!
                </h1>
                <div className="space-x-2">
                    <button
                        onClick={() => setViewMode("day")}
                        className={`px-4 py-2 rounded ${
                            viewMode === "day"
                                ? "bg-blue-500 text-white"
                                : "bg-gray-200"
                        }`}
                    >
                        День
                    </button>
                    <button
                        onClick={() => setViewMode("week")}
                        className={`px-4 py-2 rounded ${
                            viewMode === "week"
                                ? "bg-blue-500 text-white"
                                : "bg-gray-200"
                        }`}
                    >
                        Тиждень
                    </button>
                    <button
                        onClick={() => setViewMode("month")}
                        className={`px-4 py-2 rounded ${
                            viewMode === "month"
                                ? "bg-blue-500 text-white"
                                : "bg-gray-200"
                        }`}
                    >
                        Місяць
                    </button>
                    <button
                        onClick={() => setViewMode("year")}
                        className={`px-4 py-2 rounded ${
                            viewMode === "year"
                                ? "bg-blue-500 text-white"
                                : "bg-gray-200"
                        }`}
                    >
                        Рік
                    </button>
                </div>
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-4 gap-6">
                <div className="lg:col-span-3">
                    <Calendar viewMode={viewMode} />
                </div>
                <div className="lg:col-span-1">
                    <div className="bg-white rounded-lg shadow p-4">
                        <h2 className="text-xl font-semibold mb-4">
                            Завдання на сьогодні
                        </h2>
                        <TasksList />
                    </div>
                </div>
            </div>
        </div>
    );
}
