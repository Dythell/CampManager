document.addEventListener("DOMContentLoaded", async () => {
    const token = localStorage.getItem("token");
    if (!token) {
        window.location.href = "../Auth/login.html";
        return;
    }

    try {
        const response = await fetch("https://localhost:7060/api/profile", {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            }
        });

        if (!response.ok) {
            throw new Error("Ошибка при загрузке профиля");
        }

        const userData = await response.json();

        document.getElementById("userName").textContent = userData.name || "";
        document.getElementById("userSurname").textContent = userData.surname || "";
        document.getElementById("userPatronymic").textContent = userData.patronymic || "";
        document.getElementById("userPhone").textContent = userData.phoneNumber || "";

        let roleText;
        switch (userData.role) {
            case "Counselor":
                roleText = "Вожатый";
                break;
            case "Admin":
                roleText = "Администратор";
                break;
            case "GAdmin":
                roleText = "Главный администратор";
                break;
            default:
                roleText = userData.role;
        }
        document.getElementById("role").textContent = roleText;

    } catch (error) {
        console.error("Ошибка загрузки профиля:", error);
        alert("Не удалось загрузить профиль. Пожалуйста, попробуйте ещё раз.");
    }

    document.getElementById("logout").addEventListener("click", () => {
        localStorage.removeItem("token");
        window.location.href = "../Auth/login.html";
    });
});
