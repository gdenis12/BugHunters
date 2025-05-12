import { useNavigate, useLocation } from "react-router-dom";
import CreateEventForm from "./CreateEventForm";

export default function CreateEventPage() {
    const navigate = useNavigate();
    const location = useLocation();
    const initialDate = location.state?.date;

    const handleSuccess = () => {
        navigate("/calendar");
    };

    return (
        <div className="min-h-screen bg-gray-50 py-8 px-4 sm:px-6 lg:px-8">
            <div className="max-w-3xl mx-auto">
                <div className="mb-8">
                    <h2 className="text-2xl font-bold text-gray-900">
                        Створення нової події
                    </h2>
                    <p className="mt-1 text-sm text-gray-500">
                        Заповніть форму для створення нової події в календарі
                    </p>
                </div>

                <div className="bg-white shadow rounded-lg p-6">
                    <CreateEventForm 
                        onSuccess={handleSuccess}
                        initialDate={initialDate}
                    />
                </div>
            </div>
        </div>
    );
} 