document.addEventListener("DOMContentLoaded", () => {
  const token = localStorage.getItem("token");
  if (!token) {
    window.location.href = "../Auth/login.html";
    return;
  }

  const logout = document.getElementById("logout");
  logout.onclick = () => {
    localStorage.removeItem("token");
    window.location.href = "../Auth/login.html";
  };

  const form = document.getElementById("sessionForm");
  const errorEl = document.getElementById("sessionError");
  const tableBody = document.getElementById("sessionsBody");

  // Функция для загрузки и отрисовки списка смен
  async function loadSessions() {
    try {
      const res = await fetch("https://localhost:7060/api/sessions", {
        headers: { "Authorization": `Bearer ${token}` }
      });
      if (!res.ok) throw new Error("Не удалось загрузить список смен");
      const sessions = await res.json();

      // Очистим таблицу
      tableBody.innerHTML = "";

      // Вставим строки
      sessions.forEach(s => {
        const tr = document.createElement("tr");
        tr.innerHTML = `
          <td>${s.session_Id}</td>
          <td>${s.number}</td>
          <td>${s.type}</td>
          <td>${s.year}</td>
          <td>${s.season}</td>
        `;
        tableBody.appendChild(tr);
      });
    } catch (err) {
      console.error(err);
      tableBody.innerHTML = `<tr><td colspan="5" class="error">Ошибка: ${err.message}</td></tr>`;
    }
  }

  // Обработчик создания новой смены
  form.addEventListener("submit", async e => {
    e.preventDefault();
    errorEl.textContent = "";

    const payload = {
      number: parseInt(document.getElementById("number").value, 10),
      type: document.getElementById("type").value,
      year: parseInt(document.getElementById("year").value, 10),
      season: document.getElementById("season").value
    };

    try {
      const res = await fetch("https://localhost:7060/api/sessions", {
        method: "POST",
        headers: {
          "Authorization": `Bearer ${token}`,
          "Content-Type": "application/json"
        },
        body: JSON.stringify(payload)
      });
      if (!res.ok) {
        const errorData = await res.json();
        throw new Error(errorData.message || "Ошибка создания смены");
      }
      // После успешного создания – перезагрузим таблицу
      form.reset();
      await loadSessions();
    } catch (err) {
      console.error(err);
      errorEl.textContent = err.message;
    }
  });

  // Изначальная загрузка
  loadSessions();
});
