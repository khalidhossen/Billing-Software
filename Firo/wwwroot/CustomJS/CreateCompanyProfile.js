const imageInput = document.getElementById('Logo');
const preview = document.getElementById('ImagePreview');
const removeButton = document.getElementById('removeLogo');

if (preview.src && preview.src !== window.location.href + '#') {
    preview.style.display = 'block';
}

$(document).ready(function () {
    $('#saveProfile').click(function () {

        //sweeet alert loading open
        Swal.fire({
            title: 'Saving...',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });

        var formData = new FormData($('#companyProfileForm')[0]);

        let id = $('#CompanyProfileId').val();

        let url = '';

        if ( id != '00000000-0000-0000-0000-000000000000' ) {
            url = '/ProfileSetup/UpdateCompany';
        }
        else {
            url = '/ProfileSetup/CreateCompany';
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
                console.log('An error occurred while saving the profile.');
            }
        });
    });
});
