$(function () {
    // --- Autocomplete ---
    $("#searchBox").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Customers/SuggestCustomer',
                data: { term: request.term },
                success: function (data) {
                    response(data);
                }
            });
        },
        minLength: 1,
        select: function (event, ui) {
            $.ajax({
                url: '/Customers/Index',
                data: { searchString: ui.item.value },
                success: function (html) {
                    $("#customerTableBody").html($(html).find("#customerTableBody").html());
                }
            });
        }
    });

    // hiện thông báo 
    // --- SweetAlert thông báo success/error ---
    if (typeof successMessage !== "undefined" && successMessage && successMessage !== "null") {
        Swal.fire({
            icon: 'success',
            title: 'Thành công',
            text: successMessage,
            confirmButtonColor: '#3085d6',
            timer: 2000,
            showConfirmButton: false
        });
    }

    if (typeof errorMessage !== "undefined" && errorMessage && errorMessage !== "null") {
        Swal.fire({
            icon: 'error',
            title: 'Lỗi',
            text: errorMessage,
            confirmButtonColor: '#d33',
            timer: 2500,
            showConfirmButton: false
        });
    }

    // --- Popup confirm khi Update/Delete ---
    $("#customerTableBody").on("click", ".btn-group a", function (e) {
        e.preventDefault();
        var url = $(this).attr("href");
        var actionTitle = $(this).attr("title");

        Swal.fire({
            title: 'Bạn có chắc không?',
            text: `Bạn muốn ${actionTitle.toLowerCase()} khách hàng này?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Có, thực hiện!',
            cancelButtonText: 'Hủy'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: url,
                    type: 'POST', // nếu action là POST
                    success: function (response) {
                        // Reload table body từ server
                        $("#customerTableBody").html($(response).find("#customerTableBody").html());
                        Swal.fire('Thành công!', `${actionTitle} thành công.`, 'success');
                    },
                    error: function () {
                        Swal.fire('Lỗi!', 'Không thể thực hiện hành động.', 'error');
                    }
                });
            }
        });
    });
});
