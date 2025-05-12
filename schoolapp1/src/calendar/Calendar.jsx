import { useState, useEffect } from "react";
import axios from "../api/axios";
import DayView from "./views/DayView";
import WeekView from "./views/WeekView";
import MonthView from "./views/MonthView";
import YearView from "./views/YearView";
import { useAuthStore } from "../store/authStore";
import { useNavigate } from "react-router-dom";

export default function Calendar({ viewMode = "month" }) {
    const [events, setEvents] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [selectedDate, setSelectedDate] = useState(new Date());
    const user = useAuthStore((state) => state.user);
    const navigate = useNavigate();

    useEffect(() => {
        loadEvents();
    }, [selectedDate, viewMode]);

    const loadEvents = async () => {
        setLoading(true);
        setError(null);
        try {
            // Визначаємо діапазон дат для запиту залежно від режиму перегляду
            let startDate, endDate;
            switch (viewMode) {
                case "day":
                    startDate = new Date(selectedDate);
                    endDate = new Date(selectedDate);
                    break;
                case "week":
                    startDate = new Date(selectedDate);
                    startDate.setDate(startDate.getDate() - startDate.getDay());
                    endDate = new Date(startDate);
                    endDate.setDate(endDate.getDate() + 6);
                    break;
                case "month":
                    startDate = new Date(selectedDate.getFullYear(), selectedDate.getMonth(), 1);
                    endDate = new Date(selectedDate.getFullYear(), selectedDate.getMonth() + 1, 0);
                    break;
                case "year":
                    startDate = new Date(selectedDate.getFullYear(), 0, 1);
                    endDate = new Date(selectedDate.getFullYear(), 11, 31);
                    break;
            }

            const response = await axios.get("/events", {
                params: {
                    startDate: startDate.toISOString(),
                    endDate: endDate.toISOString(),
                    userId: user.id,
                },
            });

            const filteredEvents = response.data.map(event => ({
                ...event,
                name: event.participants?.includes(user.id) || user.role === 'teacher'
                    ? event.name
                    : 'Зайнято',
                content: event.participants?.includes(user.id) || user.role === 'teacher'
                    ? event.content
                    : null
            }));

            setEvents(filteredEvents);
        } catch (err) {
            setError("Помилка завантаження подій");
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const renderCalendarView = () => {
        const props = {
            events,
            selectedDate,
            onDateSelect: setSelectedDate,
            loading,
            error,
        };

        switch (viewMode) {
            case "day":
                return <DayView {...props} />;
            case "week":
                return <WeekView {...props} />;
            case "month":
                return <MonthView {...props} />;
            case "year":
                return <YearView {...props} />;
            default:
                return <MonthView {...props} />;
        }
    };

    const handleCreateEvent = () => {
        navigate("/events/create", { state: { date: selectedDate } });
    };

    return (
        <div className="min-h-screen bg-gray-50 py-8 px-4 sm:px-6 lg:px-8">
            <div className="max-w-7xl mx-auto">
                <div className="sm:flex sm:items-center sm:justify-between mb-8">
                    <div>
                        <h2 className="text-2xl font-bold text-gray-900">Календар</h2>
                        <p className="mt-1 text-sm text-gray-500">
                            {selectedDate.toLocaleDateString("uk-UA", {
                                year: "numeric",
                                month: "long"
                            })}
                        </p>
                    </div>
                    <div className="mt-4 sm:mt-0 sm:ml-4">
                        <button
                            onClick={handleCreateEvent}
                            className="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                        >
                            Створити подію
                        </button>
                    </div>
                </div>
                <div className="bg-white rounded-lg shadow p-4">
                    <div className="flex justify-between items-center mb-4">
                        <button
                            onClick={() => {
                                const newDate = new Date(selectedDate);
                                switch (viewMode) {
                                    case "day":
                                        newDate.setDate(newDate.getDate() - 1);
                                        break;
                                    case "week":
                                        newDate.setDate(newDate.getDate() - 7);
                                        break;
                                    case "month":
                                        newDate.setMonth(newDate.getMonth() - 1);
                                        break;
                                    case "year":
                                        newDate.setFullYear(newDate.getFullYear() - 1);
                                        break;
                                }
                                setSelectedDate(newDate);
                            }}
                            className="px-4 py-2 bg-gray-200 rounded hover:bg-gray-300"
                        >
                            &lt; Попередній
                        </button>

                        <h2 className="text-xl font-semibold">
                            {selectedDate.toLocaleDateString("uk-UA", {
                                day: viewMode === "day" ? "numeric" : undefined,
                                month: viewMode !== "year" ? "long" : undefined,
                                year: "numeric"
                            })}
                        </h2>

                        <button
                            onClick={() => {
                                const newDate = new Date(selectedDate);
                                switch (viewMode) {
                                    case "day":
                                        newDate.setDate(newDate.getDate() + 1);
                                        break;
                                    case "week":
                                        newDate.setDate(newDate.getDate() + 7);
                                        break;
                                    case "month":
                                        newDate.setMonth(newDate.getMonth() + 1);
                                        break;
                                    case "year":
                                        newDate.setFullYear(newDate.getFullYear() + 1);
                                        break;
                                }
                                setSelectedDate(newDate);
                            }}
                            className="px-4 py-2 bg-gray-200 rounded hover:bg-gray-300"
                        >
                            Наступний &gt;
                        </button>
                    </div>

                    {renderCalendarView()}
                </div>
            </div>
        </div>
    );
}