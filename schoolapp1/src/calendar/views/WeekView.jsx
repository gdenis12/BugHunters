import { useMemo } from "react";

export default function WeekView({
                                     events,
                                     selectedDate,
                                     onDateSelect,
                                     loading,
                                     error,
                                 }) {
    // Отримуємо дні тижня
    const weekDays = useMemo(() => {
        const days = [];
        const currentDay = selectedDate.getDay();
        const diff = selectedDate.getDate() - currentDay + (currentDay === 0 ? -6 : 1); // Коригуємо для того, щоб тиждень починався з понеділка

        // Створюємо нову дату замість мутації оригінальної
        const weekStart = new Date(selectedDate);
        weekStart.setDate(diff);

        for (let i = 0; i < 7; i++) {
            const date = new Date(weekStart);
            date.setDate(weekStart.getDate() + i);
            days.push({
                date,
                events: events.filter(
                    (event) =>
                        new Date(event.dateOfStart).toDateString() ===
                        date.toDateString()
                ),
            });
        }

        return days;
    }, [selectedDate, events]);

    // Створюємо часові слоти для кожної години (з 8:00 до 20:00)
    const timeSlots = Array.from({ length: 13 }, (_, i) => i + 8);

    if (loading) {
        return <div className="text-center py-4">Завантаження...</div>;
    }

    if (error) {
        return <div className="text-center py-4 text-red-500">{error}</div>;
    }

    return (
        <div className="overflow-x-auto">
            <div className="min-w-[800px]">
                {/* Заголовки днів тижня */}
                <div className="grid grid-cols-8 gap-1 mb-1">
                    <div className="w-20" /> {/* Пустий кут для часових міток */}
                    {weekDays.map(({ date }) => (
                        <div
                            key={date.toISOString()}
                            className="text-center py-2 bg-gray-100 font-semibold"
                            onClick={() => onDateSelect(date)}
                        >
                            <div>{["Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Нд"][date.getDay() === 0 ? 6 : date.getDay() - 1]}</div>
                            <div>{date.getDate()}</div>
                        </div>
                    ))}
                </div>

                {/* Часова сітка */}
                {timeSlots.map((hour) => (
                    <div key={hour} className="grid grid-cols-8 gap-1 min-h-[60px]">
                        {/* Часова мітка */}
                        <div className="w-20 text-right pr-2 py-2 text-sm text-gray-500">
                            {`${hour}:00`}
                        </div>

                        {/* Комірки для кожного дня */}
                        {weekDays.map(({ date, events }) => {
                            const hourEvents = events.filter((event) => {
                                const eventHour = new Date(event.dateOfStart).getHours();
                                return eventHour === hour;
                            });

                            return (
                                <div
                                    key={`${date.toISOString()}-${hour}`}
                                    className="border border-gray-200 p-1"
                                >
                                    {hourEvents.map((event) => (
                                        <div
                                            key={event.id}
                                            className="text-xs p-1 mb-1 rounded bg-blue-100 text-blue-800 truncate"
                                            title={event.name}
                                        >
                                            {event.name}
                                        </div>
                                    ))}
                                </div>
                            );
                        })}
                    </div>
                ))}
            </div>
        </div>
    );
}