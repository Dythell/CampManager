document.addEventListener("DOMContentLoaded", async () => {
  const token = localStorage.getItem("token");
  const childForm = document.getElementById("childForm");
  const groupSelect = document.getElementById("groupSelect");
  const childError = document.getElementById("childError");

  const surnameField    = document.getElementById("surname");
  const nameField       = document.getElementById("name");
  const patronymicField = document.getElementById("patronymic");
  const phoneField      = document.getElementById("parentNumber");

  if (!token) {
    window.location.href = "../Auth/login.html";
    return;
  }

  const cyrillicRegex = /^[А-ЯЁа-яё]+$/;
  const phoneRegex    = /^(?:\+7|8)\d{10}$/;

  [surnameField, nameField, patronymicField].forEach(field => {
    field.addEventListener("input", () => {
      if (!field.value) {
        field.setCustomValidity("");
      } else if (!cyrillicRegex.test(field.value)) {
        field.setCustomValidity("Только русские буквы без пробелов и цифр");
      } else {
        field.setCustomValidity("");
      }
      field.reportValidity();
    });
  });

  phoneField.addEventListener("input", () => {
    if (!phoneField.value) {
      phoneField.setCustomValidity("");
    } else if (!phoneRegex.test(phoneField.value)) {
      phoneField.setCustomValidity("Неправильный формат. +71234567890 или 81234567890");
    } else {
      phoneField.setCustomValidity("");
    }
    phoneField.reportValidity();
  });

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

    for (let field of [surnameField, nameField, patronymicField]) {
      if (!field.value || !cyrillicRegex.test(field.value)) {
        childError.textContent = "Фамилия, имя и отчество — только русские буквы";
        return;
      }
    }
    if (!phoneRegex.test(phoneField.value)) {
      childError.textContent = "Телефон в формате +71234567890 или 81234567890";
      return;
    }

    const surname      = surnameField.value.trim();
    const name         = nameField.value.trim();
    const patronymic   = patronymicField.value.trim();
    const birthYearStr = document.getElementById("birthYear").value;
    const parentNumber = phoneField.value.trim();
    const groupId      = parseInt(groupSelect.value);

    if (!birthYearStr || isNaN(groupId)) {
      childError.textContent = "Заполните дату рождения и выберите отряд";
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
      groupSelect.value = "";
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
