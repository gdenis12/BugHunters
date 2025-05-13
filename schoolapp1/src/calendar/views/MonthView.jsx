import { useMemo } from "react";

export default function MonthView({
                                      events,
                                      selectedDate,
                                      onDateSelect,
                                      loading,
                                      error,
                                  }) {
    // Отримуємо дні місяця
    const calendarDays = useMemo(() => {
        const days = [];
        const year = selectedDate.getFullYear();
        const month = selectedDate.getMonth();

        // Отримуємо перший день місяця
        const firstDay = new Date(year, month, 1);
        // Отримуємо останній день місяця
        const lastDay = new Date(year, month + 1, 0);

        // Додаємо дні з попереднього місяця
        const firstDayOfWeek = firstDay.getDay() || 7; // 7 для неділі
        for (let i = firstDayOfWeek - 1; i >= 0; i--) {
            const date = new Date(year, month, -i);
            days.push({
                date,
                isCurrentMonth: false,
                events: events.filter(
                    (event) =>
                        new Date(event.dateOfStart).toDateString() ===
                        date.toDateString()
                ),
            });
        }

        // Додаємо дні поточного місяця
        for (let i = 1; i <= lastDay.getDate(); i++) {
            const date = new Date(year, month, i);
            days.push({
                date,
                isCurrentMonth: true,
                events: events.filter(
                    (event) =>
                        new Date(event.dateOfStart).toDateString() ===
                        date.toDateString()
                ),
            });
        }

        // Додаємо дні наступного місяця
        const remainingDays = 42 - days.length; // 6 рядків по 7 днів
        for (let i = 1; i <= remainingDays; i++) {
            const date = new Date(year, month + 1, i);
            days.push({
                date,
                isCurrentMonth: false,
                events: events.filter(
                    (event) =>
                        new Date(event.dateOfStart).toDateString() ===
                        date.toDateString()
                ),
            });
        }

        return days;
    }, [selectedDate, events]);

    if (loading) {
        return <div className="text-center py-4">Завантаження...</div>;
    }

    if (error) {
        return <div className="text-center py-4 text-red-500">{error}</div>;
    }

    return (
        <div>
            {/* Заголовки днів тижня */}
            <div className="grid grid-cols-7 gap-1 mb-1">
                {["Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Нд"].map((day) => (
                    <div
                        key={day}
                        className="text-center py-2 bg-gray-100 font-semibold"
                    >
                        {day}
                    </div>
                ))}
            </div>

            {/* Дні місяця */}
            <div className="grid grid-cols-7 gap-1">
                {calendarDays.map(({ date, isCurrentMonth, events }) => (
                    <div
                        key={date.toISOString()}
                        onClick={() => onDateSelect(date)}
                        className={`min-h-[100px] p-2 border rounded cursor-pointer ${
                            isCurrentMonth
                                ? "bg-white"
                                : "bg-gray-50 text-gray-400"
                        } ${
                            date.toDateString() ===
                            selectedDate.toDateString()
                                ? "border-blue-500"
                                : "border-gray-200"
                        } hover:border-blue-300`}
                    >
                        <div className="font-semibold mb-1">
                            {date.getDate()}
                        </div>
                        <div className="space-y-1">
                            {events.slice(0, 3).map((event) => (
                                <div
                                    key={event.id}
                                    className="text-xs p-1 rounded bg-blue-100 text-blue-800 truncate"
                                    title={event.name}
                                >
                                    {event.name}
                                </div>
                            ))}
                            {events.length > 3 && (
                                <div className="text-xs text-gray-500">
                                    +{events.length - 3} ще
                                </div>
                            )}
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}