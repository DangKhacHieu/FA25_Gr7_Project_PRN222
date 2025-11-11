using Microsoft.AspNetCore.SignalR;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Hubs
{
    public class DataSignalR : Hub
    {
        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
        public async Task SendCartUpdate()
        {
            // Gửi thông điệp "ReceiveCartUpdate" đến TẤT CẢ các client đang kết nối
            await Clients.All.SendAsync("ReceiveCartUpdate");
        }
    }
}
