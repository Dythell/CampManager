document.addEventListener("DOMContentLoaded", async () => {
  const token = localStorage.getItem("token");
  const logoutBtn = document.getElementById("logout");
  const tbody = document.getElementById("pendingAdminsBody");

  if (!token) {
    window.location.href = "../Auth/login.html";
    return;
  }

  logoutBtn.addEventListener("click", () => {
    localStorage.removeItem("token");
    window.location.href = "../Auth/login.html";
  });

  try {
    const res = await fetch("https://localhost:7060/api/users/pending-admins", {
      headers: { Authorization: `Bearer ${token}` }
    });

    if (!res.ok) throw new Error("Ошибка при загрузке админов");

    const admins = await res.json();

    if (admins.length === 0) {
      tbody.innerHTML = "<tr><td colspan='3'>Нет ожидающих подтверждения админов</td></tr>";
      return;
    }

    tbody.innerHTML = "";
    admins.forEach(admin => {
      const tr = document.createElement("tr");

      tr.innerHTML = `
        <td>${admin.user_Id}</td>
        <td>${admin.username}</td>
        <td><button data-id="${admin.user_Id}">Подтвердить</button></td>
      `;

      tr.querySelector("button").addEventListener("click", async () => {
        const id = admin.user_Id;
        const confirmRes = await fetch(`https://localhost:7060/api/users/${id}/confirm-admin`, {
          method: "PUT",
          headers: { Authorization: `Bearer ${token}` }
        });

        if (confirmRes.ok) {
          tr.remove();
        } else {
          alert("Ошибка при подтверждении");
        }
      });

      tbody.appendChild(tr);
    });
  } catch (err) {
    console.error(err);
    tbody.innerHTML = "<tr><td colspan='3'>Ошибка загрузки</td></tr>";
  }
});
