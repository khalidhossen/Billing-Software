function getCurrentPage() {
    return parseInt($('#pagination-numbers').data('current-page')) || 1;
}
$('#pagination-numbers').on('click', '.page-number-btn', function () {
    var page = $(this).data('page');
    loadData(page);
});

$('#pagination-numbers').on('click', '#previous-page-btn', function () {
    var currentPage = getCurrentPage();
    if (currentPage > 1) {
        loadData(currentPage - 1);
    }
});

$('#pagination-numbers').on('click', '#next-page-btn', function () {
    var currentPage = getCurrentPage();
    var totalPages = $('#pagination-numbers').data('total-pages');
    if (currentPage < totalPages) {
        loadData(currentPage + 1);
    }
});
$('#searchButton').click(function () {
    loadData(1);
});

function updatePagination(paginationData) {
    var currentPage = paginationData.currentPage;
    var totalPages = paginationData.totalPages;

    var pages = [];
    var paginationNumbers = $('#pagination-numbers');
    paginationNumbers.empty().data('current-page', currentPage).data('total-pages', totalPages);

    pages.push(1);

    if (currentPage > 4) {
        pages.push("...");
    }

    for (let i = Math.max(2, currentPage - 1); i <= Math.min(totalPages - 1, currentPage + 1); i++) {
        pages.push(i);
    }

    if (totalPages > 1 && currentPage < totalPages - 3) {
        pages.push("...");
    }

    if (totalPages > 1) {
        pages.push(totalPages);
    }

    pages = pages.filter((page, index, self) => self.indexOf(page) === index || page === "...");

    pages.forEach(page => {
        if (page === "...") {
            paginationNumbers.append('<span class="dots">...</span>');
        } else {
            let btn = $('<button>')
                .addClass('btn btn-light page-number-btn btn-sm')
                .text(page)
                .attr('data-page', page);
            if (page === currentPage) {
                btn.addClass('btn-primary active');
            }
            paginationNumbers.append(btn);
        }
    });

    var previousDisabled = currentPage === 1 ? 'disabled' : '';
    var nextDisabled = currentPage === totalPages ? 'disabled' : '';

    paginationNumbers.prepend(`
        <button class="btn btn-secondary btn-sm" id="previous-page-btn" ${previousDisabled}>Previous</button>
    `);

    paginationNumbers.append(`
        <button class="btn btn-secondary btn-sm" id="next-page-btn" ${nextDisabled}>Next</button>
    `);
}