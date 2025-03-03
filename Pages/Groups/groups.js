document.addEventListener("DOMContentLoaded", async () => {
    const token = localStorage.getItem("token");
    const groupForm = document.getElementById("groupForm");
    const sessionSelect = document.getElementById("sessionSelect");
    const counselorSelect = document.getElementById("counselorSelect");
    const groupError = document.getElementById("groupError");
    const groupsBody = document.getElementById("groupsBody");

    if (!token) {
        window.location.href = "../Auth/login.html";
        return;
    }

    async function loadSessions() {
        try {
            const response = await fetch("https://localhost:7060/api/sessions", {
                headers: { "Authorization": `Bearer ${token}` }
            });
            if (!response.ok) throw new Error("Не удалось загрузить список смен");
            const sessions = await response.json();
            sessionSelect.innerHTML = '<option value="">-- Выберите смену --</option>';
            sessions.forEach(session => {
                const option = document.createElement("option");
                option.value = session.session_Id;
                option.textContent = `Смена №${session.number} (${session.year} - ${session.season}, ${session.type})`;
                sessionSelect.appendChild(option);
            });
        } catch (error) {
            console.error("Ошибка загрузки смен:", error);
            groupError.textContent = error.message;
        }
    }

    async function loadCounselors() {
        try {
            const response = await fetch("https://localhost:7060/api/counselors", {
                headers: { "Authorization": `Bearer ${token}` }
            });
            if (!response.ok) throw new Error("Не удалось загрузить список вожатых");
            const counselors = await response.json();
            counselorSelect.innerHTML = '<option value="">-- Выберите вожатого --</option>';
            counselors.forEach(counselor => {
                const option = document.createElement("option");
                option.value = counselor.counselor_Id;
                option.textContent = `${counselor.surname} ${counselor.name} ${counselor.patronymic}`;
                counselorSelect.appendChild(option);
            });
        } catch (error) {
            console.error("Ошибка загрузки вожатых:", error);
            groupError.textContent = error.message;
        }
    }

    async function loadGroups() {
        try {
            const response = await fetch("https://localhost:7060/api/groups", {
                headers: { "Authorization": `Bearer ${token}` }
            });
            if (!response.ok) {
                throw new Error("Не удалось загрузить список отрядов");
            }
            const groups = await response.json();

            groupsBody.innerHTML = "";
            groups.forEach(group => {
                const tr = document.createElement("tr");

                const groupId = group.groupId || group.group_Id;
                const groupName = group.name;
                const groupNumber = group.number;
                
                const sessionStr = group.session
                    ? `Смена №${group.session.number} (${group.session.year} - ${group.session.season}, ${group.session.type})`
                    : "Нет смены";

                const counselorStr = group.counselor
                    ? `${group.counselor.surname} ${group.counselor.name} ${group.counselor.patronymic}`
                    : "Нет вожатого";

                tr.innerHTML = `
                    <td>${groupId}</td>
                    <td>${groupName}</td>
                    <td>${groupNumber}</td>
                    <td>${sessionStr}</td>
                    <td>${counselorStr}</td>
                `;
                groupsBody.appendChild(tr);
            });
        } catch (error) {
            console.error("Ошибка загрузки отрядов:", error);
            groupError.textContent = error.message;
        }
    }

    await loadSessions();
    await loadCounselors();
    await loadGroups();

    groupForm.addEventListener("submit", async (e) => {
        e.preventDefault();
        groupError.textContent = "";

        const groupName = document.getElementById("groupName").value.trim();
        const groupNumber = parseInt(document.getElementById("groupNumber").value);
        const sessionId = parseInt(sessionSelect.value);
        const counselorId = parseInt(counselorSelect.value);

        if (!groupName || isNaN(groupNumber) || isNaN(sessionId) || isNaN(counselorId)) {
            groupError.textContent = "Все поля обязательны.";
            return;
        }

        const payload = {
            name: groupName,
            number: groupNumber,
            counselorId: counselorId,
            sessionId: sessionId
        };

        try {
            const response = await fetch("https://localhost:7060/api/groups", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify(payload)
            });
            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || "Ошибка создания отряда");
            }
            alert("Отряд создан успешно!");
            groupForm.reset();
            await loadGroups();
        } catch (error) {
            console.error("Ошибка при создании отряда:", error);
            groupError.textContent = error.message;
        }
    });

    document.getElementById("logout").addEventListener("click", () => {
        localStorage.removeItem("token");
        window.location.href = "../Auth/login.html";
    });
});
