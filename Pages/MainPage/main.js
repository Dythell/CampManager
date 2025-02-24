document.addEventListener("DOMContentLoaded", async () => {
    const token = localStorage.getItem("token");
    const profileLink = document.getElementById("profileLink");
    const logout = document.getElementById("logout");
    const createEventBtn = document.getElementById("createEventBtn");
    const eventsList = document.getElementById("eventsList");

    if (token) {
        profileLink.style.display = "inline";
        logout.style.display = "inline";
    } else {
        window.location.href = "../Auth/login.html";
        return;
    }

    logout.addEventListener("click", () => {
        localStorage.removeItem("token");
        window.location.href = "../Auth/login.html";
    });

    createEventBtn.addEventListener("click", () => {
        window.location.href = "../Event/create-event.html";
    });

    try {
        const response = await fetch("https://localhost:7060/api/events", {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });
        if (!response.ok) throw new Error("Не удалось загрузить мероприятия");
        const events = await response.json();
        eventsList.innerHTML = "";
        events.forEach(ev => {
            const li = document.createElement("li");
            // Отображаем название кастомного мероприятия или название из шаблона уже имеющегося
            li.textContent = ev.customName || ev.eventTemplateName || ev.type + " - " + new Date(ev.dateTime).toLocaleString();
            eventsList.appendChild(li);
        });
    } catch (error) {
        console.error(error);
        eventsList.innerHTML = `<li>Ошибка загрузки мероприятий: ${error.message}</li>`;
    }
});
