document.addEventListener("DOMContentLoaded", async () => {
    const token = localStorage.getItem("token");
    const templateList = document.getElementById("templateList");
    const form = document.getElementById("templateForm");

    if (!token) {
        window.location.href = "../Auth/login.html";
        return;
    }

    async function loadTemplates() {
        try {
            const response = await fetch("https://localhost:7060/api/eventtemplates", {
                headers: { "Authorization": `Bearer ${token}` }
            });

            if (!response.ok) throw new Error("Ошибка загрузки шаблонов");

            const templates = await response.json();
            templateList.innerHTML = "";
            templates.forEach(template => {
                const li = document.createElement("li");
                li.textContent = `${template.name} (${template.type}) - ${template.defaultDescription || "Нет описания"}`;
                templateList.appendChild(li);
            });
        } catch (error) {
            console.error("Ошибка:", error);
            templateList.innerHTML = `<li>Ошибка загрузки шаблонов</li>`;
        }
    }

    form.addEventListener("submit", async (event) => {
        event.preventDefault();

        const name = document.getElementById("name").value.trim();
        const type = document.getElementById("type").value;
        const description = document.getElementById("description").value.trim();

        if (!name || !type) {
            alert("Название и тип обязательны");
            return;
        }

        try {
            const response = await fetch("https://localhost:7060/api/eventtemplates", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify({ name, type, defaultDescription: description })
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || "Ошибка создания шаблона");
            }

            alert("Шаблон добавлен успешно!");
            form.reset();
            loadTemplates();
        } catch (error) {
            console.error("Ошибка при создании шаблона:", error);
            alert("Ошибка при создании шаблона: " + error.message);
        }
    });

    loadTemplates();
    document.getElementById("logout").addEventListener("click", () => {
        localStorage.removeItem("token");
        window.location.href = "../Auth/login.html";
    });
});
