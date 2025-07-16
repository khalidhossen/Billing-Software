$(document).ready(function () {

    // Remove spaces from DataKey input
    $('input[name="DataKey"]').on('input', function () {
        this.value = this.value.replace(/\s+/g, '');
    });

    // Save LookUp (Add or Update)
    $('#saveLookUp').click(function () {
        Swal.fire({
            title: 'Saving...',
            allowOutsideClick: false,
            didOpen: () => Swal.showLoading()
        });

        var formData = new FormData($('#LookUpForm')[0]);
        let id = $('#LookUpId').val();
        let url = id && id !== '00000000-0000-0000-0000-000000000000'
            ? '/LookUp/UpdateLookUp'
            : '/LookUp/CreateLookUp';

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
                        text: response.msg
                    });

                    // Build row data
                    const dataKey = $('input[name="DataKey"]').val();
                    const displayText = $('input[name="DisplayText"]').val();
                    const dataValue = $('input[name="DataValue"]').val();
                    const dataOrder = $('input[name="DataOrder"]').val();
                    const isActive = $('select[name="IsActive"]').val() === "true" ? "Yes" : "No";

                    // Use the ID from the response if new
                    const lookUpId = response.lookUpId || id;

                    // Create new row
                    const newRow = `
                        <tr>
                            <td>${dataKey}</td>
                            <td>${displayText}</td>
                            <td>${dataValue}</td>
                            <td>${dataOrder}</td>
                            <td>${isActive}</td>
                            <td>
                                <button class="btn btn-primary btn-sm edit-btn"
                                        data-id="${lookUpId}"
                                        data-key="${dataKey}"
                                        data-text="${displayText}"
                                        data-value="${dataValue}"
                                        data-order="${dataOrder}"
                                        data-active="${$('select[name="IsActive"]').val()}">
                                    Edit
                                </button>
                                <button class="btn btn-danger btn-sm delete-btn" data-id="${lookUpId}">Delete</button>
                            </td>
                        </tr>
                    `;

                    // Replace existing row or append new one
                    const existingRow = $(`#lookUpBody .edit-btn[data-id="${lookUpId}"]`).closest('tr');
                    if (existingRow.length > 0) {
                        existingRow.replaceWith(newRow);
                    } else {
                        $('#lookUpBody').append(newRow);
                    }

                    // Reset form and button
                    $('#LookUpForm')[0].reset();
                    $('select[name="IsActive"]').val("true");
                    $('#LookUpId').val('00000000-0000-0000-0000-000000000000');
                    $('#saveLookUp').text('Add Item');
                } else {
                    Swal.fire({
                        icon: "error",
                        text: response.msg
                    });
                }
            },
            error: function () {
                Swal.close();
                Swal.fire({
                    icon: "error",
                    text: "Something went wrong!"
                });
            }
        });
    });

    // Delete handler
    $(document).on('click', '.delete-btn', function () {
        alert("hit the button");
        const button = $(this);
        const lookUpId = button.data('id');
        Swal.fire({
            title: "Are you sure?",
            text: "You won't be able to revert this!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#d33",
            cancelButtonColor: "#3085d6",
            confirmButtonText: "Yes, delete it!"
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `/LookUp/Delete/${lookUpId}`,
                    type: 'POST',
                    success: function (response) {
                        if (response.success) {
                            button.closest('tr').fadeOut(500, function () {
                                $(this).remove();
                            });
                            Swal.fire("Deleted!", response.msg, "success");
                        } else {
                            Swal.fire("Error!", response.msg, "error");
                        }
                    },
                    error: function () {
                        Swal.fire("Error!", "Something went wrong!", "error");
                    }
                });
            }
        });
    });

    // Edit handler
    $(document).on('click', '.edit-btn', function () {
        const button = $(this);

        // Get values from button attributes
        const lookUpId = button.data('id');
        const dataKey = button.data('key');
        const displayText = button.data('text');
        const dataValue = button.data('value');
        const dataOrder = button.data('order');
        const isActive = button.data('active');

        // Fill form inputs
        $('#LookUpId').val(lookUpId);
        $('input[name="DataKey"]').val(dataKey);
        $('input[name="DisplayText"]').val(displayText);
        $('input[name="DataValue"]').val(dataValue);
        $('input[name="DataOrder"]').val(dataOrder);
        $('select[name="IsActive"]').val(isActive.toString());

        // Change button text to "Update"
        $('#saveLookUp').text('Update');
    });

});
