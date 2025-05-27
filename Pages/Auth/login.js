document.addEventListener("DOMContentLoaded", () => {
    const loginForm = document.getElementById("loginForm");
    const loader = document.getElementById("loader");
    const loginError = document.getElementById("loginError");

    function showLoader() {
        loader.style.display = "block";
    }

    function hideLoader() {
        loader.style.display = "none";
    }

    if (loginForm) {
        loginForm.addEventListener("submit", async (event) => {
            event.preventDefault();
            loginError.innerText = "";
            showLoader();

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
                window.location.href = "../MainPage/main.html";
            } catch (error) {
                loginError.innerText = error.message;
            } finally {
                hideLoader();
            }
        });
    }
    
});
