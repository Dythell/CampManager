document.addEventListener("DOMContentLoaded", async () => {
    function getQueryParam(param) {
        const urlParams = new URLSearchParams(window.location.search);
        return urlParams.get(param);
    }

    const eventId = parseInt(getQueryParam("eventId"));
    if (!eventId) {
        alert("Мероприятие не определено");
        return;
    }

    document.getElementById("eventTitle").textContent = `Мероприятие №${eventId}`;
    document.getElementById("eventDateTime").textContent = `Дата и время: ${new Date().toLocaleString()}`;

    const token = localStorage.getItem("token");
    if (!token) {
        window.location.href = "../Auth/login.html";
        return;
    }

    try {
        const commentsResponse = await fetch(`https://localhost:7060/api/comments?event_Id=${eventId}`, {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });
        if (!commentsResponse.ok) throw new Error("Не удалось загрузить комментарии");
        const comments = await commentsResponse.json();
        const commentsList = document.getElementById("commentsList");
        commentsList.innerHTML = "";
        comments.forEach(comment => {
            const li = document.createElement("li");
            li.textContent = `${comment.displayName} (${new Date(comment.createdAt).toLocaleString()}): ${comment.message}`;
            commentsList.appendChild(li);
        });

    } catch (error) {
        console.error("Ошибка загрузки комментариев:", error);
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7060/commentHub", {
            accessTokenFactory: () => token
        })
        .build();

    connection.on("ReceiveComment", (receivedEventId, displayName, message, createdAt) => {
        if (receivedEventId === eventId) {
            const li = document.createElement("li");
            li.textContent = `${displayName} (${new Date(createdAt).toLocaleString()}): ${message}`;
            document.getElementById("commentsList").appendChild(li);
        }
    });

    connection.start()
        .then(() => {
            console.log("Соединение с CommentHub установлено.");
            connection.invoke("JoinEventGroup", eventId)
                .catch(err => console.error("Ошибка при присоединении к группе:", err.toString()));
        })
        .catch(err => console.error("Ошибка подключения к CommentHub:", err));

    document.getElementById("sendCommentBtn").addEventListener("click", async () => {
        const message = document.getElementById("commentMessage").value;
        if (!message.trim()) return;
        try {
            await connection.invoke("SendComment", eventId, message);
            document.getElementById("commentMessage").value = "";
        } catch (err) {
            console.error("Ошибка отправки комментария:", err);
        }
    });

    document.getElementById("logout").addEventListener("click", () => {
        localStorage.removeItem("token");
        window.location.href = "../Auth/login.html";
    });
});
