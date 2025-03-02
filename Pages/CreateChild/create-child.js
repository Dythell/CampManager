document.addEventListener("DOMContentLoaded", async () => {
    const token = localStorage.getItem("token");
    const childForm = document.getElementById("childForm");
    const groupSelect = document.getElementById("groupSelect");
    const childError = document.getElementById("childError");

    if (!token) {
        window.location.href = "../Auth/login.html";
        return;
    }

    async function loadGroups() {
        try {
            const response = await fetch("https://localhost:7060/api/groups", {
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            });
            if (!response.ok) throw new Error("Не удалось загрузить список отрядов");

            const groups = await response.json();
            groupSelect.innerHTML = '<option value="">-- Выберите отряд --</option>';
            groups.forEach(group => {
                const option = document.createElement("option");
                option.value = group.group_Id;
                option.textContent = `Отряд "${group.name}" (Номер: ${group.number})`;
                groupSelect.appendChild(option);
            });
        } catch (error) {
            console.error("Ошибка загрузки отрядов:", error);
            childError.textContent = error.message;
        }
    }

    await loadGroups();

    childForm.addEventListener("submit", async (e) => {
        e.preventDefault();
        childError.textContent = "";

        const surname = document.getElementById("surname").value.trim();
        const name = document.getElementById("name").value.trim();
        const patronymic = document.getElementById("patronymic").value.trim();
        const birthYearStr = document.getElementById("birthYear").value;
        const parentNumber = document.getElementById("parentNumber").value.trim();
        const groupId = parseInt(groupSelect.value);

        if (!surname || !name || !patronymic || !birthYearStr || !parentNumber || isNaN(groupId)) {
            childError.textContent = "Все поля обязательны.";
            return;
        }

        const birthYear = new Date(birthYearStr);
        const birthYearUtc = new Date(Date.UTC(
            birthYear.getFullYear(),
            birthYear.getMonth(),
            birthYear.getDate()
        )).toISOString();

        const payload = {
            surname,
            name,
            patronymic,
            birthYear: birthYearUtc,
            parentNumber,
            groupId
        };


        try {
            const response = await fetch("https://localhost:7060/api/children", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify(payload)
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || "Ошибка создания ребенка");
            }

            alert("Ребенок успешно добавлен!");
            childForm.reset();
        } catch (error) {
            console.error("Ошибка при создании ребенка:", error);
            childError.textContent = error.message;
        }
    });

    document.getElementById("logout").addEventListener("click", () => {
        localStorage.removeItem("token");
        window.location.href = "../Auth/login.html";
    });
});
