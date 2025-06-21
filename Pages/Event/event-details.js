document.addEventListener("DOMContentLoaded", async () => {
    function getQueryParam(param) {
        return new URLSearchParams(window.location.search).get(param);
    }

    const eventId = parseInt(getQueryParam("eventId"), 10);
    if (!eventId) {
        alert("Мероприятие не определено");
        return;
    }

    const token = localStorage.getItem("token");
    if (!token) {
        window.location.href = "../Auth/login.html";
        return;
    }

    try {
        const evRes = await fetch(`https://localhost:7060/api/events/${eventId}`, {
            headers: { "Authorization": `Bearer ${token}` }
        });
        if (!evRes.ok) throw new Error("Не удалось загрузить мероприятие");
        const ev = await evRes.json();

        document.getElementById("eventTitle").textContent =
            ev.eventName || `Мероприятие №${eventId}`;
        document.getElementById("eventDateTime").textContent =
            `Дата и время: ${new Date(ev.dateTime).toLocaleString()}`;

        const sessionEl = document.getElementById("eventSession");
        if (ev.sessionId) {
            try {
                const allSRes = await fetch("https://localhost:7060/api/sessions", {
                    headers: { "Authorization": `Bearer ${token}` }
                });
                if (!allSRes.ok) throw new Error();
                const allSessions = await allSRes.json();
                const s = allSessions.find(s => s.session_Id === ev.sessionId);
                if (s) {
                    sessionEl.textContent = `Смена: №${s.number}, ${s.season} ${s.year}`;
                } else {
                    sessionEl.textContent = "Смена: не найдена";
                }
            } catch {
                sessionEl.textContent = "Смена: не удалось загрузить";
            }
        } else {
            sessionEl.textContent = "Смена: не указана";
        }

        const counEl = document.getElementById("eventCounselor");
        if (ev.counselorId) {
            try {
                const allCRes = await fetch("https://localhost:7060/api/counselors", {
                    headers: { "Authorization": `Bearer ${token}` }
                });
                if (!allCRes.ok) throw new Error();
                const allCounselors = await allCRes.json();
                const c = allCounselors.find(c => c.counselor_Id === ev.counselorId);
                if (c) {
                    counEl.textContent =
                        `Ответственный вожатый: ${c.surname} ${c.name}` +
                        (c.patronymic ? ` ${c.patronymic}` : "");
                } else {
                    counEl.textContent = "Ответственный вожатый: не найден";
                }
            } catch {
                counEl.textContent = "Ответственный вожатый: не удалось загрузить";
            }
        } else {
            counEl.textContent = "Ответственный вожатый: не назначен";
        }

        const cmRes = await fetch(
            `https://localhost:7060/api/comments?event_Id=${eventId}`,
            { headers: { "Authorization": `Bearer ${token}` } }
        );
        if (!cmRes.ok) throw new Error("Не удалось загрузить комментарии");
        const comments = await cmRes.json();
        const list = document.getElementById("commentsList");
        list.innerHTML = "";
        comments.forEach(c => {
            const li = document.createElement("li");
            li.textContent =
                `${c.displayName} (${new Date(c.createdAt).toLocaleString()}): ${c.message}`;
            list.appendChild(li);
        });
    } catch (err) {
        console.error("Ошибка загрузки данных мероприятия:", err);
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7060/commentHub", {
            accessTokenFactory: () => token
        })
        .build();

    connection.on("ReceiveComment", (eid, name, msg, at) => {
        if (eid === eventId) {
            const li = document.createElement("li");
            li.textContent = `${name} (${new Date(at).toLocaleString()}): ${msg}`;
            document.getElementById("commentsList").appendChild(li);
        }
    });

    connection.start()
        .then(() => connection.invoke("JoinEventGroup", eventId))
        .catch(err => console.error("SignalR ошибка:", err));

    document.getElementById("sendCommentBtn").addEventListener("click", async () => {
        const msg = document.getElementById("commentMessage").value.trim();
        if (!msg) return;
        try {
            await connection.invoke("SendComment", eventId, msg);
            document.getElementById("commentMessage").value = "";
        } catch (e) {
            console.error("Ошибка отправки комментария:", e);
        }
    });

    document.getElementById("logout").addEventListener("click", () => {
        localStorage.removeItem("token");
        window.location.href = "../Auth/login.html";
    });
});
