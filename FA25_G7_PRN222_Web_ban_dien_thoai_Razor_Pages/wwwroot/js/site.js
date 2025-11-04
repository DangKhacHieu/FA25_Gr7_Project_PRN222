var connection = new signalR.HubConnectionBuilder()
    .withUrl("/DataSignalRChanel")
    .build();

//setup lắng nghe message “load” thì thực hiện
connection.on("load", function () {
    //location.reload(); // Tải lại trang để cập nhật dữ liệu
    location.href = '/Data/Index'
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});