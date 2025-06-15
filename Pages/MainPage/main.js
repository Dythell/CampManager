document.addEventListener("DOMContentLoaded", async () => {
  const token = localStorage.getItem("token");
  if (!token) return window.location.href = "../Auth/login.html";

  const profileLink       = document.getElementById("profileLink");
  const logout            = document.getElementById("logout");
  const createSessionBtn  = document.getElementById("createSessionBtn");
  const createChildBtn    = document.getElementById("createChildBtn");
  const manageGroupsBtn   = document.getElementById("manageGroupsBtn");
  const sessionDetailsBtn = document.getElementById("sessionDetailsBtn");
  const createEventBtn    = document.getElementById("createEventBtn");
  const templatesBtn      = document.getElementById("templatesBtn");
  const eventsList        = document.getElementById("eventsList");

  profileLink.style.display = "inline";
  logout.style.display      = "inline";

  let role = "";
  try {
    const pr = await fetch("https://localhost:7060/api/profile", {
      headers: { "Authorization": `Bearer ${token}` }
    });
    if (!pr.ok) throw "";
    const data = await pr.json();
    role = data.role;
    if (role === "Admin" || role === "GAdmin") {
      createSessionBtn.style.display = "inline-block";
      createChildBtn.style.display   = "inline-block";
      manageGroupsBtn.style.display  = "inline-block";
    }
  } catch {
    createSessionBtn.style.display =
    createChildBtn.style.display   =
    manageGroupsBtn.style.display  = "none";
  }

  sessionDetailsBtn.onclick = () => window.location.href = "../SessionDetails/session-details.html";
  logout.onclick           = () => { localStorage.removeItem("token"); window.location.href = "../Auth/login.html"; };
  createEventBtn.onclick   = () => window.location.href = "../Event/create-event.html";
  templatesBtn.onclick     = () => window.location.href = "../EventTemplates/event-templates.html";
  createSessionBtn.onclick = () => window.location.href = "../Sessions/sessions.html";
  createChildBtn.onclick   = () => window.location.href = "../CreateChild/create-child.html";
  manageGroupsBtn.onclick  = () => window.location.href = "../Groups/groups.html";

  let currentEditId = null;
  const modal        = document.getElementById("editModal");
  const closeModal   = document.getElementById("closeModal");
  const editForm     = document.getElementById("editForm");
  const editName     = document.getElementById("editName");
  const editDateTime = document.getElementById("editDateTime");
  const editStatus   = document.getElementById("editStatus");
  const editCounselor= document.getElementById("editCounselor");

  closeModal.onclick = () => modal.style.display = "none";
  window.onclick     = e => { if (e.target === modal) modal.style.display = "none"; };

  let counselorsList = [];
  try {
    const resp = await fetch("https://localhost:7060/api/counselors", {
      headers: { "Authorization": `Bearer ${token}` }
    });
    if (!resp.ok) throw new Error("Не удалось загрузить список вожатых");
    counselorsList = await resp.json();
  } catch (err) {
    console.warn(err);
  }

  editForm.onsubmit = async e => {
    e.preventDefault();
    const payload = {};
    if (editName.value.trim())       payload.CustomName  = editName.value.trim();
    if (editDateTime.value)          payload.DateTime    = new Date(editDateTime.value).toISOString();
    if (editStatus.value)            payload.Status      = editStatus.value;

    const sel = editCounselor.value;
    if (sel === "__KEEP__") {
    } else if (sel === "") {
      payload.CounselorId = null;
    } else {
      payload.CounselorId = parseInt(sel);
    }

    const res = await fetch(`https://localhost:7060/api/events/${currentEditId}`, {
      method: "PUT",
      headers: {
        "Authorization": `Bearer ${token}`,
        "Content-Type": "application/json"
      },
      body: JSON.stringify(payload)
    });
    if (res.ok) {
      const card = document.querySelector(`.event-card[data-id="${currentEditId}"]`);
      if (payload.CustomName) card.querySelector("h4").textContent = payload.CustomName;
      if (payload.DateTime)   card.querySelector("p").textContent  = new Date(payload.DateTime).toLocaleString();
      modal.style.display = "none";
    } else {
      alert("Ошибка при сохранении");
    }
  };

  try {
    const res = await fetch("https://localhost:7060/api/events", {
      headers: { "Authorization": `Bearer ${token}` }
    });
    if (!res.ok) throw new Error("Не удалось загрузить мероприятия");
    const events = await res.json();
    eventsList.innerHTML = "";

    events.forEach(ev => {
      const card = document.createElement("div");
      card.className = "event-card";
      card.dataset.id = ev.event_Id;

      const title = document.createElement("h4");
      title.textContent = ev.eventName || "Без названия";
      const time = document.createElement("p");
      time.textContent = new Date(ev.dateTime).toLocaleString();

      const btnContainer = document.createElement("div");
      btnContainer.className = "card-buttons";

      const viewBtn = document.createElement("button");
      viewBtn.textContent = "Открыть";
      viewBtn.onclick = () => window.location.href = `../Event/event-details.html?eventId=${ev.event_Id}`;
      btnContainer.appendChild(viewBtn);

      if (role === "Admin" || role === "GAdmin") {
        const editBtn = document.createElement("button");
        editBtn.textContent = "Ред.";
        editBtn.onclick = () => {
          currentEditId = ev.event_Id;

          editName.value = ev.eventName || "";
          const dt       = new Date(ev.dateTime);
          const tzOffset = dt.getTimezoneOffset() * 60000;
          editDateTime.value = new Date(dt - tzOffset).toISOString().slice(0,16);
          editStatus.value   = ev.status;

          editCounselor.innerHTML = "";
          const keepOpt = document.createElement("option");
          keepOpt.value = "__KEEP__";
          keepOpt.textContent = "Оставить текущего";
          editCounselor.appendChild(keepOpt);

          const noneOpt = document.createElement("option");
          noneOpt.value = "";
          noneOpt.textContent = "Не назначен";
          editCounselor.appendChild(noneOpt);

          counselorsList.forEach(c => {
            const opt = document.createElement("option");
            opt.value = c.counselor_Id;
            opt.textContent = `${c.surname} ${c.name}${c.patronymic ? ' ' + c.patronymic : ''}`;
            editCounselor.appendChild(opt);
          });

          // выбратт по умолчанию если был назначен keep иначе отправлем none
          editCounselor.value = ev.counselorId != null
            ? "__KEEP__"
            : "";

          modal.style.display = "block";
        };
        btnContainer.appendChild(editBtn);

        const delBtn = document.createElement("button");
        delBtn.textContent = "Удал.";
        delBtn.onclick = async () => {
          if (!confirm("Удалить это мероприятие?")) return;
          const dres = await fetch(`https://localhost:7060/api/events/${ev.event_Id}`, {
            method: "DELETE",
            headers: { "Authorization": `Bearer ${token}` }
          });
          if (dres.ok) card.remove();
          else alert("Не удалось удалить");
        };
        btnContainer.appendChild(delBtn);
      }

      card.append(title, time, btnContainer);
      eventsList.appendChild(card);
    });
  } catch (err) {
    eventsList.innerHTML = `<p class="error">Ошибка: ${err.message}</p>`;
  }
});
