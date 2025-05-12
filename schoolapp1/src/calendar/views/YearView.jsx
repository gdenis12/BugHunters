import { useMemo } from "react";

export default function YearView({
                                     events,
                                     selectedDate,
                                     onDateSelect,
                                     loading,
                                     error,
                                 }) {
    const months = useMemo(() => {
        const result = [];
        const year = selectedDate.getFullYear();

        for (let month = 0; month < 12; month++) {
            const firstDay = new Date(year, month, 1);
            const lastDay = new Date(year, month + 1, 0);
            const monthDays = [];

            // Отримуємо перший день місяця
            const firstDayOfWeek = firstDay.getDay() || 7; // 7 для неділі

            // Додаємо дні з попереднього місяця
            for (let i = firstDayOfWeek - 1; i >= 0; i--) {
                const date = new Date(year, month, -i);
                monthDays.push({
                    date,
                    isCurrentMonth: false,
                    hasEvents: events.some(
                        (event) =>
                            new Date(event.dateOfStart).toDateString() ===
                            date.toDateString()
                    ),
                });
            }

            // Додаємо дні поточного місяця
            for (let day = 1; day <= lastDay.getDate(); day++) {
                const date = new Date(year, month, day);
                monthDays.push({
                    date,
                    isCurrentMonth: true,
                    hasEvents: events.some(
                        (event) =>
                            new Date(event.dateOfStart).toDateString() ===
                            date.toDateString()
                    ),
                });
            }

            // Додаємо дні наступного місяця
            const remainingDays = 42 - monthDays.length; // 6 рядків по 7 днів
            for (let i = 1; i <= remainingDays; i++) {
                const date = new Date(year, month + 1, i);
                monthDays.push({
                    date,
                    isCurrentMonth: false,
                    hasEvents: events.some(
                        (event) =>
                            new Date(event.dateOfStart).toDateString() ===
                            date.toDateString()
                    ),
                });
            }

            result.push({
                name: new Date(year, month).toLocaleString("uk-UA", {
                    month: "long",
                }),
                days: monthDays,
            });
        }

        return result;
    }, [selectedDate, events]);

    if (loading) {
        return <div className="text-center py-4">Завантаження...</div>;
    }

    if (error) {
        return <div className="text-center py-4 text-red-500">{error}</div>;
    }

    return (
        <div className="grid grid-cols-3 gap-4">
            {months.map((month) => (
                <div key={month.name} className="border rounded p-2">
                    <h3 className="text-center font-semibold mb-2">
                        {month.name}
                    </h3>
                    <div className="grid grid-cols-7 gap-1">
                        {/* Заголовки днів тижня */}
                        {["Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Нд"].map(
                            (day) => (
                                <div
                                    key={day}
                                    className="text-center text-xs font-medium text-gray-500"
                                >
                                    {day}
                                </div>
                            )
                        )}

                        {/* Дні місяця */}
                        {month.days.map(({ date, isCurrentMonth, hasEvents }) => (
                            <div
                                key={date.toISOString()}
                                onClick={() => onDateSelect(date)}
                                className={`text-center text-xs p-1 cursor-pointer rounded ${
                                    isCurrentMonth
                                        ? hasEvents
                                            ? "bg-blue-100 text-blue-800"
                                            : "hover:bg-gray-100"
                                        : "text-gray-400"
                                } ${
                                    date.toDateString() ===
                                    selectedDate.toDateString()
                                        ? "ring-2 ring-blue-500"
                                        : ""
                                }`}
                            >
                                {date.getDate()}
                            </div>
                        ))}
                    </div>
                </div>
            ))}
        </div>
    );
}