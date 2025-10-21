document.addEventListener("DOMContentLoaded", function () {
    const toggleButton = document.getElementById("sidebarToggle");
    const sidebar = document.getElementById("sidebar");
    const content = document.querySelector(".content");

    if (toggleButton && sidebar && content) {
        toggleButton.addEventListener("click", function () {
            sidebar.classList.toggle("active");
            content.classList.toggle("sidebar-active");
        });
    }
});

function logout() {
    fetch('/LogOutStaffAndAdminController', { method: 'POST' })
        .then(response => {
            if (response.ok) {
                window.location.href = '/LoginOfDashboard';
            } else {
                alert('Logout Failed!');
            }
        })
        .catch(error => console.error('Logout Error:', error));
}
