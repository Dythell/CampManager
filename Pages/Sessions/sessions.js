document.addEventListener("DOMContentLoaded", async () => {
    const token = localStorage.getItem("token");
    const sessionForm = document.getElementById("sessionForm");
    const sessionsList = document.getElementById("sessionsList");
    const sessionError = document.getElementById("sessionError");

    if (!token) {
        window.location.href = "../Auth/login.html";
        return;
    }

    document.getElementById("logout").addEventListener("click", () => {
        localStorage.removeItem("token");
        window.location.href = "../Auth/login.html";
    });

    async function loadSessions() {
        try {
            const response = await fetch("https://localhost:7060/api/sessions", {
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            });
            if (!response.ok) throw new Error("Ошибка загрузки смен");
            const sessions = await response.json();
            sessionsList.innerHTML = "";
            sessions.forEach(session => {
                const li = document.createElement("li");
                li.textContent = `Смена №${session.number} (${session.type}), ${session.year} - ${session.season}`;
                sessionsList.appendChild(li);
            });
        } catch (error) {
            console.error("Ошибка загрузки смен:", error);
            sessionsList.innerHTML = `<li>${error.message}</li>`;
        }
    }

    sessionForm.addEventListener("submit", async (e) => {
        e.preventDefault();
        sessionError.textContent = "";

        const number = parseInt(document.getElementById("number").value);
        const type = document.getElementById("type").value;
        const year = parseInt(document.getElementById("year").value);
        const season = document.getElementById("season").value;

        if (!number || !type || !year || !season) {
            sessionError.textContent = "Все поля обязательны";
            return;
        }

        try {
            const response = await fetch("https://localhost:7060/api/sessions", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify({ number, type, year, season })
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || "Ошибка создания смены");
            }

            alert("Смена создана успешно!");
            sessionForm.reset();
            loadSessions();
        } catch (error) {
            console.error("Ошибка при создании смены:", error);
            sessionError.textContent = error.message;
        }
    });

    loadSessions();
});
