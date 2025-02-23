document.addEventListener("DOMContentLoaded", () => {
    const token = localStorage.getItem("token");
    const profileLink = document.getElementById("profileLink");
    const logout = document.getElementById("logout");

    if (token) {
        profileLink.style.display = "inline";
        logout.style.display = "inline";
    }

    logout.addEventListener("click", () => {
        localStorage.removeItem("token");
        window.location.reload();
    });

    if (!token && window.location.pathname !== "/Auth/login.html" && window.location.pathname !== "/Auth/register.html") {
        window.location.href = "../Auth/login.html";
    }
});
