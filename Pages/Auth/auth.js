document.addEventListener("DOMContentLoaded", () => {
    const loginForm = document.getElementById("loginForm");
    const registerForm = document.getElementById("registerForm");

    if (loginForm) {
        loginForm.addEventListener("submit", async (event) => {
            event.preventDefault();
            const username = document.getElementById("loginUsername").value;
            const password = document.getElementById("loginPassword").value;

            try {
                const response = await fetch("https://localhost:7060/api/auth/login", {
                    method: "POST",
                    headers: { "Content-Type": "application/json; charset=UTF-8" },
                    body: JSON.stringify({ username, password })
                });

                if (!response.ok) {
                    const errorData = await response.json();
                    throw new Error(errorData.message || "Ошибка входа");
                }

                const data = await response.json();
                localStorage.setItem("token", data.token);
                alert("Вход выполнен!");
                window.location.href = "../MainPage/main.html";
            } catch (error) {
                document.getElementById("loginError").innerText = error.message;
            }
        });
    }

    if (registerForm) {
        registerForm.addEventListener("submit", async (event) => {
            event.preventDefault();
            const username = document.getElementById("registerUsername").value;
            const password = document.getElementById("registerPassword").value;
            const role = document.getElementById("registerRole").value;

            const usernameRegex = /^[a-zA-Z0-9_]+$/;
            if (!usernameRegex.test(username)) {
                document.getElementById("registerError").innerText = "Логин может содержать только латинские буквы, цифры и _";
                return;
            }

            try {
                const response = await fetch("https://localhost:7060/api/auth/register", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json; charset=UTF-8",
                        "Accept-Charset": "UTF-8"
                    },
                    body: JSON.stringify({ username, password, role })
                });

                if (!response.ok) {
                    const errorData = await response.json();
                    throw new Error(errorData.message || "Ошибка регистрации");
                }

                alert("Регистрация успешна!");
                window.location.href = "login.html";
            } catch (error) {
                document.getElementById("registerError").innerText = error.message;
            }
        });
    }
});
