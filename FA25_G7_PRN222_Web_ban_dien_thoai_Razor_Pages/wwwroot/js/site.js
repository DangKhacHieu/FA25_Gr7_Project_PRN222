// wwwroot/js/site.js

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/DataSignalRChanel")
    .build();

connection.on("ReceiveCartNotification", (message, type) => {
    showGlobalToast(message, type);
});

connection.on("ReceiveCartUpdate", (data) => {
    showGlobalToast(data.message, data.newQuantity === 0 ? "info" : "success");

    const cartItemId = data.cartItemId;
    const inputEl = document.querySelector(`input[data-id="${cartItemId}"]`);

    if (inputEl && data.newQuantity > 0) {
        const row = inputEl.closest(".row");
        const subtotalDisplay = row.querySelector(".subtotal");
        if (subtotalDisplay) {
            subtotalDisplay.textContent = data.subtotal.toLocaleString("vi-VN") + " đ";
        }
        inputEl.dataset.quantity = data.newQuantity;
        inputEl.value = data.newQuantity;

    } else if (inputEl && data.newQuantity === 0) {
        const row = inputEl.closest(".row");
        row.remove();
    }

    const totalEl = document.getElementById("cart-total");
    if (totalEl) {
        totalEl.textContent = data.total.toLocaleString("vi-VN") + " đ";
    }

    if (data.total === 0 && document.querySelectorAll('.card-body .row').length === 0) {
        // nếu cart bị xóa không còn cart items nào thì reload lại để hiện trang empty
        location.reload(); 
    }
});

connection.start().catch(function (err) {
    console.error("SignalR Connection Error: " + err.toString());
});


function showGlobalToast(message, type = 'info') {
    const toastContainer = document.querySelector('.toast-container');
    if (!toastContainer) {
        console.warn("Không tìm thấy .toast-container để hiển thị thông báo.");
        return;
    }

    const toastId = 'toast-' + Math.random().toString(36).substr(2, 9);

    const toastTypeClass = type === 'success' ? 'text-bg-success'
        : (type === 'danger' || type === 'warning' ? 'text-bg-danger' : 'text-bg-info');

    const toastHTML = `
        <div id="${toastId}" class="toast align-items-center ${toastTypeClass} border-0" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `;

    toastContainer.insertAdjacentHTML('beforeend', toastHTML);

    const toastElement = document.getElementById(toastId);
    const toast = new bootstrap.Toast(toastElement, { delay: 1500 });
    toast.show();

    toastElement.addEventListener('hidden.bs.toast', () => {
        toastElement.remove();
    });
}