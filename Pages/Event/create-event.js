document.addEventListener("DOMContentLoaded", async () => {
    const createEventForm = document.getElementById("createEventForm");
    const sessionSelect = document.getElementById("sessionSelect");
    const counselorSelect = document.getElementById("counselorSelect");
    const templateSelect = document.getElementById("eventTemplateSelect");
    const typeSelect = document.getElementById("type");
    const typeContainer = document.getElementById("typeContainer");
    const customNameContainer = document.getElementById("customNameContainer");
    const eventError = document.getElementById("eventError");

    const token = localStorage.getItem("token");
    if (!token) {
        window.location.href = "../Auth/login.html";
        return;
    }

    let eventTemplates = [];

    try {
        const response = await fetch("https://localhost:7060/api/sessions", {
            headers: { "Authorization": `Bearer ${token}` }
        });
        if (!response.ok) throw new Error("Не удалось загрузить список смен");
        const sessions = await response.json();
        sessions.forEach(session => {
            const option = document.createElement("option");
            option.value = session.session_Id;
            option.textContent = `Смена №${session.number} (${session.year} - ${session.season}, ${session.type})`;
            sessionSelect.appendChild(option);
        });
    } catch (error) {
        console.error(error);
        eventError.textContent = error.message;
    }

    try {
        const response = await fetch("https://localhost:7060/api/counselors", {
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });
        if (!response.ok) throw new Error("Не удалось загрузить список вожатых");
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

    try {
        const response = await fetch("https://localhost:7060/api/eventtemplates", {
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });
        if (!response.ok) throw new Error("Не удалось загрузить список шаблонов мероприятий");
        eventTemplates = await response.json();
        eventTemplates.forEach(template => {
            const option = document.createElement("option");
            option.value = template.eventTemplate_Id;
            option.textContent = `${template.name} (${template.type})`;
            templateSelect.appendChild(option);
        });
    } catch (error) {
        console.error(error);
        eventError.textContent = error.message;
    }

    // Обработчик изменения выбора шаблона мероприятия 
    templateSelect.addEventListener("change", () => {
        if (templateSelect.value) {
            const selectedTemplate = eventTemplates.find(t => t.eventTemplate_Id == templateSelect.value);
            if (selectedTemplate) {
                typeSelect.value = selectedTemplate.type;
            }
            // Если шаблон выбран, скрываем контейнеры и исключаем из валидации
            typeContainer.style.display = "none";
            typeSelect.disabled = true;
            customNameContainer.style.display = "none";
        } else {
            // Если шаблон не выбран, показываем поля и включаем валидацию
            typeContainer.style.display = "block";
            typeSelect.disabled = false;
            customNameContainer.style.display = "block";
        }
    });

    createEventForm.addEventListener("submit", async (e) => {
        e.preventDefault();

        const sessionId = parseInt(sessionSelect.value);

        const templateIdVal = templateSelect.value;
        const eventTemplateId = templateIdVal ? parseInt(templateIdVal) : null;
        const customName = templateSelect.value ? null : document.getElementById("customName").value;
        // Если шаблон выбран, событие не кастомное и наоборот
        const isCustomEvent = templateSelect.value ? false : true;
        // Если шаблон выбран, тип берется автоматически из таблицы шаблнов, если нет то вручную вводится
        const type = typeSelect.value;
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

    document.getElementById("logout").addEventListener("click", () => {
        localStorage.removeItem("token");
        window.location.href = "../Auth/login.html";
    });
});
