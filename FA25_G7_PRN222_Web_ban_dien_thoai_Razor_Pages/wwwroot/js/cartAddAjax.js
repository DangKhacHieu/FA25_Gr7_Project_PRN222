// --- HÀM TOAST TOÀN CỤC ---
function showGlobalToast(message, type = 'info') {
    const toastContainer = document.querySelector('.toast-container');
    if (!toastContainer) return;

    const toastId = 'toast-' + Math.random().toString(36).substr(2, 9);
    const toastTypeClass = type === 'success' ? 'text-bg-success' : 'text-bg-danger';

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
    const toast = new bootstrap.Toast(toastElement, { delay: 3000 });
    toast.show();

    // Xóa element khỏi DOM sau khi toast đã ẩn
    toastElement.addEventListener('hidden.bs.toast', () => {
        toastElement.remove();
    });
}

async function handleAjaxAddToCart(form) {
    try {
        const formData = new FormData(form);
        const token = formData.get('__RequestVerificationToken');

        const response = await fetch('/Carts/Add?handler=Ajax', {
            method: 'POST',
            body: new URLSearchParams(formData),
            headers: {
                'RequestVerificationToken': token
            }
        });

        if (!response.ok) {
            throw new Error('Lỗi máy chủ: ' + response.status);
        }

        const result = await response.json();
        showGlobalToast(result.message, result.success ? 'success' : 'danger');

    } catch (error) {
        console.error('Lỗi khi thêm vào giỏ:', error);
        showGlobalToast('Lỗi kết nối. Vui lòng thử lại.', 'danger');
    }
}

// Gắn sự kiện khi trang tải xong
document.addEventListener('DOMContentLoaded', () => {
    // 1. Gắn sự kiện cho các form "Thêm vào giỏ" (Home, Search)
    document.querySelectorAll('.form-add-to-cart-ajax').forEach(form => {
        form.addEventListener('submit', function (event) {
            event.preventDefault(); // Ngăn form submit
            handleAjaxAddToCart(this);
        });
    });

    // 2. Gắn sự kiện cho nút "Thêm vào giỏ" (trang Details)
    const btnAjax = document.getElementById('btn-add-to-cart-ajax');
    if (btnAjax) {
        btnAjax.addEventListener('click', function () {
            const form = this.closest('form'); // Tìm form cha
            if (form) {
                handleAjaxAddToCart(form);
            }
        });
    }
});
