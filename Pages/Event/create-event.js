document.addEventListener("DOMContentLoaded", async () => {
    const createEventForm = document.getElementById("createEventForm");
    const counselorSelect = document.getElementById("counselorSelect");
    const eventError = document.getElementById("eventError");

    const token = localStorage.getItem("token");
    if (!token) {
        window.location.href = "../Auth/login.html";
        return;
    }

    try {
        const response = await fetch("https://localhost:7060/api/counselors", {
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });
        if (!response.ok) {
            throw new Error("Не удалось загрузить список вожатых");
        }
        const counselors = await response.json();
        counselors.forEach(counselor => {
            const option = document.createElement("option");
            option.value = counselor.counselor_Id;
            option.textContent = `${counselor.surname} ${counselor.name} ${counselor.patronymic}`;
            counselorSelect.appendChild(option);
        });
    } catch (error) {
        console.error(error);
        eventError.textContent = error.message;
    }

    createEventForm.addEventListener("submit", async (e) => {
        e.preventDefault();

        const sessionId = parseInt(document.getElementById("sessionId").value);
        const templateIdVal = document.getElementById("templateId").value;
        const eventTemplateId = templateIdVal ? parseInt(templateIdVal) : null;
        const customName = document.getElementById("customName").value;
        const isCustomEvent = document.getElementById("isCustomEvent").value === "true";
        const type = document.getElementById("type").value;
        const dateTime = document.getElementById("dateTime").value;
        const status = document.getElementById("status").value;
        const counselorId = parseInt(counselorSelect.value);

        const payload = {
            SessionId: sessionId,
            EventTemplateId: eventTemplateId,
            CustomName: customName,
            IsCustomEvent: isCustomEvent,
            Type: type,
            DateTime: new Date(dateTime),
            Status: status,
            CounselorId: counselorId
        };

        try {
            const response = await fetch("https://localhost:7060/api/events", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json; charset=UTF-8",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify(payload)
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || "Ошибка создания мероприятия");
            }

            alert("Мероприятие создано успешно!");
            window.location.href = "../MainPage/main.html";
        } catch (error) {
            eventError.textContent = error.message;
        }
    });
});
