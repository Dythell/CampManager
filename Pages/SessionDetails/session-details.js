document.addEventListener("DOMContentLoaded", async () => {
    const token = localStorage.getItem("token");
    const sessionDropdown = document.getElementById("sessionDropdown");
    const loadSessionBtn = document.getElementById("loadSessionBtn");
    const sessionDetailsSection = document.getElementById("sessionDetails");
  
    if (!token) {
        window.location.href = "../Auth/login.html";
        return;
    }
  
    async function loadSessions() {
        try {
            const response = await fetch("https://localhost:7060/api/sessions", {
                headers: { "Authorization": `Bearer ${token}` }
            });
            if (!response.ok) throw new Error("Ошибка загрузки смен");
            const sessions = await response.json();
            sessionDropdown.innerHTML = '<option value="">-- Выберите смену --</option>';
            sessions.forEach(session => {
                const option = document.createElement("option");
                option.value = session.sessionId || session.session_Id;
                option.textContent = `Смена №${session.number} (${session.year} - ${session.season}, ${session.type})`;
                sessionDropdown.appendChild(option);
            });
        } catch (error) {
            console.error("Ошибка загрузки смен:", error);
            sessionDetailsSection.innerHTML = `<p class="error">${error.message}</p>`;
        }
    }
  
    await loadSessions();
  
    loadSessionBtn.addEventListener("click", async () => {
        const sessionId = sessionDropdown.value;
        if (!sessionId) {
            alert("Выберите смену");
            return;
        }
        try {
            const response = await fetch(`https://localhost:7060/api/sessions/${sessionId}/details`, {
                headers: { "Authorization": `Bearer ${token}` }
            });
            if (!response.ok) throw new Error("Ошибка загрузки деталей смены");
            const details = await response.json();
            renderSessionDetails(details);
        } catch (error) {
            console.error("Ошибка загрузки деталей смены:", error);
            sessionDetailsSection.innerHTML = `<p class="error">${error.message}</p>`;
        }
    });
  
    function renderSessionDetails(details) {
        const session = details.session || details.Session;
        if (!session) {
            sessionDetailsSection.innerHTML = `<p class="error">Данные о смене не получены</p>`;
            return;
        }
  
        let html = `<h2>Смена №${session.number} (${session.year} - ${session.season}, ${session.type})</h2>`;
  
        if (details.events && details.events.length > 0) {
            html += `<h3>Мероприятия:</h3><ul>`;
            details.events.forEach(ev => {
                let eventName = ev.eventName || ev.customName || "Без названия";
                let eventTime = new Date(ev.dateTime).toLocaleString();
                html += `<li>${eventName} - ${eventTime} (Статус: ${ev.status})</li>`;
            });
            html += `</ul>`;
        } else {
            html += `<p>Мероприятия не найдены.</p>`;
        }
  
        if (details.groups && details.groups.length > 0) {
            html += `<h3>Отряды:</h3>`;
            details.groups.forEach(group => {
                html += `<div class="group">
                           <h4>Отряд: ${group.name} (Номер: ${group.number})</h4>`;
                if (group.counselor) {
                    html += `<p>Ответственный вожатый: ${group.counselor.surname} ${group.counselor.name} ${group.counselor.patronymic}</p>`;
                } else {
                    html += `<p>Ответственный вожатый: Не указан</p>`;
                }
                if (group.children && group.children.length > 0) {
                    html += `<p>Дети:</p><ul>`;
                    group.children.forEach(child => {
                        const birthDate = new Date(child.birthYear).toLocaleDateString();
                        html += `<li>${child.surname} ${child.name} ${child.patronymic} (Дата рождения: ${birthDate})</li>`;
                    });
                    html += `</ul>`;
                } else {
                    html += `<p>Дети отсутствуют</p>`;
                }
                html += `</div>`;
            });
        } else {
            html += `<p>Отряды не найдены.</p>`;
        }
  
        sessionDetailsSection.innerHTML = html;
    }
  
    document.getElementById("logout").addEventListener("click", () => {
        localStorage.removeItem("token");
        window.location.href = "../Auth/login.html";
    });
});
