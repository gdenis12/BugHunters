import { useMemo } from "react";

export default function DayView({
                                    events,
                                    selectedDate,
                                    loading,
                                    error,
                                }) {
    // Створюємо часові слоти для кожної години (з 8:00 до 20:00)
    const timeSlots = Array.from({ length: 13 }, (_, i) => i + 8);

    // Фільтруємо події для вибраного дня
    const dayEvents = useMemo(() => {
        return events.filter(
            (event) =>
                new Date(event.dateOfStart).toDateString() ===
                selectedDate.toDateString()
        );
    }, [events, selectedDate]);

    if (loading) {
        return <div className="text-center py-4">Завантаження...</div>;
    }

    if (error) {
        return <div className="text-center py-4 text-red-500">{error}</div>;
    }

    return (
        <div className="space-y-2">
            {/* Часова сітка */}
            {timeSlots.map((hour) => {
                const hourEvents = dayEvents.filter(
                    (event) => new Date(event.dateOfStart).getHours() === hour
                );

                return (
                    <div
                        key={hour}
                        className="grid grid-cols-12 gap-4 min-h-[60px] border-b border-gray-200"
                    >
                        {/* Часова мітка */}
                        <div className="col-span-1 text-right pr-4 py-2 text-sm text-gray-500">
                            {`${hour}:00`}
                        </div>

                        {/* Події */}
                        <div className="col-span-11 py-1">
                            {hourEvents.map((event) => (
                                <div
                                    key={event.id}
                                    className="p-2 mb-1 rounded bg-blue-100 text-blue-800"
                                >
                                    <div className="font-medium">
                                        {event.name}
                                    </div>
                                    {event.content && (
                                        <div className="text-sm mt-1">
                                            {event.content}
                                        </div>
                                    )}
                                    <div className="text-xs text-blue-600 mt-1">
                                        {new Date(
                                            event.dateOfStart
                                        ).toLocaleTimeString("uk-UA", {
                                            hour: "2-digit",
                                            minute: "2-digit",
                                        })}
                                        {" - "}
                                        {new Date(
                                            new Date(event.dateOfStart).getTime() +
                                            event.duration * 60000
                                        ).toLocaleTimeString("uk-UA", {
                                            hour: "2-digit",
                                            minute: "2-digit",
                                        })}
                                    </div>
                                </div>
                            ))}
                        </div>
                    </div>
                );
            })}

            {/* Якщо немає подій */}
            {dayEvents.length === 0 && (
                <div className="text-center py-8 text-gray-500">
                    На цей день подій немає
                </div>
            )}
        </div>
    );
}