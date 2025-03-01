document.addEventListener("DOMContentLoaded", async () => {
    const token = localStorage.getItem("token");
    const profileLink = document.getElementById("profileLink");
    const logout = document.getElementById("logout");
    const createEventBtn = document.getElementById("createEventBtn");
    const eventsList = document.getElementById("eventsList");
    const templatesBtn = document.getElementById("templatesBtn");
    const createSessionBtn = document.getElementById("createSessionBtn");

    if (token) {
        profileLink.style.display = "inline";
        logout.style.display = "inline";
    } else {
        window.location.href = "../Auth/login.html";
        return;
    }

    try {
        const profileResponse = await fetch("https://localhost:7060/api/profile", {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            }
        });
        if (!profileResponse.ok) {
            throw new Error("Ошибка при загрузке профиля");
        }
        const profileData = await profileResponse.json();
        if (profileData.role === "Admin") {
            createSessionBtn.style.display = "inline-block";
        } else {
            createSessionBtn.style.display = "none";
        }
    } catch (error) {
        console.error("Ошибка загрузки профиля:", error);
        createSessionBtn.style.display = "none";
    }

    logout.addEventListener("click", () => {
        localStorage.removeItem("token");
        window.location.href = "../Auth/login.html";
    });

    createEventBtn.addEventListener("click", () => {
        window.location.href = "../Event/create-event.html";
    });

    templatesBtn.addEventListener("click", () => {
        window.location.href = "../EventTemplates/event-templates.html";
    });

    createSessionBtn.addEventListener("click", () => {
        window.location.href = "../Sessions/sessions.html";
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
});
