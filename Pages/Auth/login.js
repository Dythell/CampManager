document.addEventListener("DOMContentLoaded", () => {
    const loginForm = document.getElementById("loginForm");
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
});
