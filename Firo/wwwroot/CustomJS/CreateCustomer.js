$(document).ready(function () {
    $('#saveCustomer').click(function () {

        // Manually set the IsActive value to 'true' or 'false' based on checkbox state
        if ($('#IsActive').is(':checked')) {
            // If checked, set IsActive to 'true'
            $('input[name="IsActive"]').val('true');
        } else {
            // If unchecked, set IsActive to 'false'
            $('input[name="IsActive"]').val('false');
        }

        // SweetAlert loading open
        Swal.fire({
            title: 'Saving...',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });

        var formData = new FormData($('#customerForm')[0]);
        let id = $('#CustomerId').val();
        let url = '';

        if (id !== '00000000-0000-0000-0000-000000000000') {
            url = '/Customer/UpdateCustomer';
        } else {
            url = '/Customer/CreateCustomer';
        }

        $.ajax({
            url: url,
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                Swal.close();
                if (response.success) {
                    Swal.fire({
                        icon: "success",
                        title: "Customer saved successfully!",
                        timer: 1500,
                        showConfirmButton: false
                    }).then(() => {
                        // Redirect to the List page
                        window.location.href = "/Customer/List";
                    });
                } else {
                    Swal.fire({
                        icon: "error",
                        title: "Error!",
                        text: response.msg || "Something went wrong. Please try again."
                    });
                }
            },
            error: function () {
                Swal.close();
                Swal.fire({
                    icon: "error",
                    title: "Request Failed",
                    text: "An error occurred while saving the customer."
                });
            }
        });
    });

    function loadCustomerPage(pageNumber) {
        $.ajax({
            url: '@Url.Action("List", "Customer")',
            type: 'GET',
            data: { page: pageNumber },
            success: function (result) {
                $('#customerTableContainer').html(result);
            },
            error: function () {
                alert('Failed to load page.');
            }
        });
    }
});
