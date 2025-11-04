var connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7237/ProductSignalRChanel") // Host Hub
    .build();

connection.on("load", function () {
    location.href = '/Products/Index'; // hoặc location.reload();});

    connection.start()
        .then(() => console.log("SignalR connected from MVC"))
        .catch(err => console.error("Lỗi kết nối SignalR:", err.toString()));
