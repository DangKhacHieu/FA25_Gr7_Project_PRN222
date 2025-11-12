// wwwroot/js/cartAddAjax.js

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

        if (!result.success && result.message.includes("Vui lòng đăng nhập")) {
            window.location.href = "/Login";
        }

    } catch (error) {
        console.error('Lỗi khi thêm vào giỏ:', error);
        showGlobalToast('Lỗi kết nối. Vui lòng thử lại.', 'danger');
    }
}

document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.form-add-to-cart-ajax').forEach(form => {
        form.addEventListener('submit', function (event) {
            event.preventDefault();
            handleAjaxAddToCart(this);
        });
    });

    const btnAjax = document.getElementById('btn-add-to-cart-ajax');
    if (btnAjax) {
        btnAjax.addEventListener('click', function () {
            const form = this.closest('form');
            if (form) {
                handleAjaxAddToCart(form);
            }
        });
    }
});