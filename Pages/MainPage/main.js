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

            let eventTitle = ev.eventName || "Без названия";
            const eventTime = new Date(ev.dateTime).toLocaleString();

            const link = document.createElement("a");
            link.href = `../Event/event-details.html?eventId=${ev.event_Id}`;
            link.textContent = `${eventTitle} - ${eventTime}`;
            li.appendChild(link);
            eventsList.appendChild(li);
        });
    } catch (error) {
        console.error(error);
        eventsList.innerHTML = `<li>Ошибка загрузки мероприятий: ${error.message}</li>`;
    }

    document.getElementById("templatesBtn").addEventListener("click", () => {
        window.location.href = "../EventTemplates/event-templates.html";
    });
});
