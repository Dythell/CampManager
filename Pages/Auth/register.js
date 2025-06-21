document.addEventListener("DOMContentLoaded", () => {
  const registerForm      = document.getElementById("registerForm");
  const loader            = document.getElementById("loader");
  const registerError     = document.getElementById("registerError");
  const registerRole      = document.getElementById("registerRole");
  const counselorFields   = document.getElementById("counselorFields");
  const surnameField      = document.getElementById("registerSurname");
  const nameField         = document.getElementById("registerName");
  const patronymicField   = document.getElementById("registerPatronymic");
  const phoneField        = document.getElementById("registerPhoneNumber");

  const showLoader = () => loader.style.display = "block";
  const hideLoader = () => loader.style.display = "none";

  const cyrillicRegex = /^[А-ЯЁа-яё]+$/;
  const phoneRegex    = /^(?:\+7|8)\d{10}$/;

  const toggleCounselorFields = () => {
    const isCounselor = registerRole.value === "Counselor";
    counselorFields.style.display = isCounselor ? "block" : "none";

    [surnameField, nameField, patronymicField, phoneField].forEach(f => {
      f.required = isCounselor;
      if (!isCounselor) {
        f.setCustomValidity("");
      }
    });
  };

  registerRole.addEventListener("change", toggleCounselorFields);
  toggleCounselorFields();

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
      phoneField.setCustomValidity("Неверный формат. Например: +71234567890 или 81234567890");
    } else {
      phoneField.setCustomValidity("");
    }
    phoneField.reportValidity();
  });

  registerForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    registerError.innerText = "";

    if (registerRole.value === "Counselor") {
      for (let f of [surnameField, nameField, patronymicField]) {
        if (!cyrillicRegex.test(f.value)) {
          registerError.innerText = f.placeholder + ": только кириллица";
          return;
        }
      }
      if (!phoneRegex.test(phoneField.value)) {
        registerError.innerText = "Номер телефона в формате +7XXXXXXXXXX или 8XXXXXXXXXX";
        return;
      }
    }

    showLoader();

    const payload = {
      username: document.getElementById("registerUsername").value,
      password: document.getElementById("registerPassword").value,
      role:     registerRole.value
    };

    if (registerRole.value === "Counselor") {
      payload.surname      = surnameField.value;
      payload.name         = nameField.value;
      payload.patronymic   = patronymicField.value;
      payload.phoneNumber  = phoneField.value;
    }

    try {
      const response = await fetch("https://localhost:7060/api/auth/register", {
        method: "POST",
        headers: {
          "Content-Type": "application/json; charset=UTF-8",
          "Accept-Charset": "UTF-8"
        },
        body: JSON.stringify(payload)
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || "Ошибка регистрации");
      }

      window.location.href = "login.html";
    } catch (error) {
      registerError.innerText = error.message;
    } finally {
      hideLoader();
    }
  });

  const togglePassword = document.getElementById("togglePassword");
  const passwordField  = document.getElementById("registerPassword");
  if (togglePassword && passwordField) {
    togglePassword.addEventListener("click", () => {
      const type = passwordField.getAttribute("type") === "password" ? "text" : "password";
      passwordField.setAttribute("type", type);
      togglePassword.classList.toggle("fa-eye");
      togglePassword.classList.toggle("fa-eye-slash");
    });
  }
});
