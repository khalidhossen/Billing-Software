$(document).ready(function () {
    $('#saveBranch').click(function () {

        //sweeet alert loading open
        Swal.fire({
            title: 'Saving...',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });

        var formData = new FormData($('#BranchForm')[0]);

        let id = $('#BranchId').val();

        let url = '';

        if ( id != '00000000-0000-0000-0000-000000000000' ) {
            url = '/Branch/UpdateBranch';
        }
        else {
            url = '/Branch/CreateBranch';
        }

        console.log(formData);
 

        $.ajax({
            url: url,
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.success) {

                    Swal.close();

                    Swal.fire({
                        //title: obj.msg,
                        icon: "success",
                        draggable: true
                    });

                }
            },
            error: function () {
                console.log('An error occurred while saving the branch.');
            }
        });
    });
});
