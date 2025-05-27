document.addEventListener("DOMContentLoaded", () => {
    const registerForm = document.getElementById("registerForm");
    const loader = document.getElementById("loader");
    const registerError = document.getElementById("registerError");

    function showLoader() {
        loader.style.display = "block";
    }

    function hideLoader() {
        loader.style.display = "none";
    }

    if (registerForm) {
        const registerRole = document.getElementById("registerRole");
        const counselorFields = document.getElementById("counselorFields");

        const toggleCounselorFields = () => {
            if (registerRole.value === "Counselor") {
                counselorFields.style.display = "block";
                document.getElementById("registerSurname").required = true;
                document.getElementById("registerName").required = true;
                document.getElementById("registerPatronymic").required = true;
                document.getElementById("registerPhoneNumber").required = true;
            } else {
                counselorFields.style.display = "none";
                document.getElementById("registerSurname").required = false;
                document.getElementById("registerName").required = false;
                document.getElementById("registerPatronymic").required = false;
                document.getElementById("registerPhoneNumber").required = false;
            }
        };

        toggleCounselorFields();
        registerRole.addEventListener("change", toggleCounselorFields);

        registerForm.addEventListener("submit", async (event) => {
            event.preventDefault();
            registerError.innerText = "";
            showLoader();

            const username = document.getElementById("registerUsername").value;
            const password = document.getElementById("registerPassword").value;
            const role = registerRole.value;

            const usernameRegex = /^[a-zA-Z0-9_]+$/;
            if (!usernameRegex.test(username)) {
                registerError.innerText = "Логин может содержать только латинские буквы, цифры и _";
                hideLoader();
                return;
            }

            const payload = { username, password, role };

            if (role === "Counselor") {
                payload.surname = document.getElementById("registerSurname").value;
                payload.name = document.getElementById("registerName").value;
                payload.patronymic = document.getElementById("registerPatronymic").value;
                payload.phoneNumber = document.getElementById("registerPhoneNumber").value;
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
    }

    const togglePassword = document.getElementById("togglePassword");
    const passwordField = document.getElementById("registerPassword");

    if (togglePassword && passwordField) {
        togglePassword.addEventListener("click", () => {
            const type = passwordField.getAttribute("type") === "password" ? "text" : "password";
            passwordField.setAttribute("type", type);
            togglePassword.classList.toggle("fa-eye");
            togglePassword.classList.toggle("fa-eye-slash");
        });
    }
    
});