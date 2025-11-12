document.addEventListener("DOMContentLoaded", function () {
    const searchInput = document.getElementById("search-input");
    const resultsPopup = document.getElementById("search-results-popup");
    let debounceTimer;

    if (!searchInput || !resultsPopup) {
        console.warn("Không tìm thấy ô tìm kiếm hoặc popup.");
        return;
    }

    // Hàm fetch dữ liệu
    async function fetchSearchResults(query) {
        if (query.length < 3) {
            resultsPopup.style.display = "none";
            return;
        }

        try {
            const response = await fetch(`/Products/Search?handler=Suggestions&q=${encodeURIComponent(query)}`);
            if (!response.ok) {
                resultsPopup.style.display = "none";
                return;
            }

            const suggestions = await response.json();

            if (suggestions.length > 0) {
                renderSuggestions(suggestions);
            } else {
                resultsPopup.innerHTML = "<div class='p-3 text-muted small text-center'>Không tìm thấy kết quả.</div>";
                resultsPopup.style.display = "block";
            }
        } catch (error) {
            console.error("Lỗi khi tìm kiếm:", error);
            resultsPopup.style.display = "none";
        }
    }

    // Hàm hiển thị kết quả
    function renderSuggestions(suggestions) {
        let html = suggestions.map(item => `
            <a href="/Products/Details/${item.productID}" class="search-popup-item">
                <img src="${item.imageURL || 'https://placehold.co/40x40/eee/999?text=N/A'}" alt="${item.productName}" />
                <div class="info">
                    <strong>${item.productName}</strong>
                    <span>${(item.price || 0).toLocaleString('vi-VN')} đ</span>
                </div>
            </a>
        `).join('');

        resultsPopup.innerHTML = html;
        resultsPopup.style.display = "block";
    }

    // Sự kiện gõ phím (với debounce 300ms)
    searchInput.addEventListener("input", (e) => {
        clearTimeout(debounceTimer);
        const query = e.target.value;

        debounceTimer = setTimeout(() => {
            fetchSearchResults(query);
        }, 300);
    });

    // Ẩn popup khi click ra ngoài
    document.addEventListener("click", (e) => {
        const searchContainer = e.target.closest('.position-relative');
        // Nếu click ra ngoài .position-relative của thanh search
        if (!searchContainer || !searchContainer.contains(searchInput)) {
            resultsPopup.style.display = "none";
        }
    });
});