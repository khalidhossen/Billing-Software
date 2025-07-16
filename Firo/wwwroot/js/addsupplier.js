
    document.addEventListener('DOMContentLoaded', function () {
        const addContactBtn = document.getElementById('addContact');
        const inputForm = document.getElementById('inputForm');
        const addedContacts = document.getElementById('addedContacts');

        // Array to store contact data
        const contactsArray = [];

        addContactBtn.addEventListener('click', function () {
            // Get all input values
            const inputs = inputForm.querySelectorAll('input');
            const formData = {};
            let hasValue = false;

            inputs.forEach(input => {
                formData[input.name] = input.value.trim();
                if (formData[input.name] !== '') {
                    hasValue = true;
                }
            });

            // Validate that at least one field is filled
            if (!hasValue) {
                alert('Please fill at least one field before adding a contact.');
                return;
            }

            // Add formData to contactsArray
            contactsArray.push({ ...formData });

            // Log updated array
            console.log('Contact added:', formData);
            console.log('Updated contactsArray:', contactsArray);

            // Create new contact display
            const contactDiv = document.createElement('div');
            contactDiv.className = 'mb-4 border rounded p-3 position-relative';
            contactDiv.style.paddingTop = '2.5rem'; // Add padding at top to prevent overlap with delete button

            let contactHtml = '<div class="row g-3">';

            // Row 1
            contactHtml += `
                <div class="col-md-4">
                    <div class="form-group">
                        <label class="form-label">Name</label>
                        <input type="text" class="form-control" value="${formData.Name}" readonly>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="form-group">
                        <label class="form-label">Primary Email</label>
                        <input type="email" class="form-control" value="${formData.Email}" readonly>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="form-group">
                        <label class="form-label">Secondary Email</label>
                        <input type="email" class="form-control" value="${formData.Email2}" readonly>
                    </div>
                </div>
            `;

            // Row 2
            contactHtml += `
                <div class="col-md-4">
                    <div class="form-group">
                        <label class="form-label">Primary Phone</label>
                        <input type="tel" class="form-control" value="${formData.Phone1}" readonly>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="form-group">
                        <label class="form-label">Secondary Phone</label>
                        <input type="tel" class="form-control" value="${formData.Phone2}" readonly>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="form-group">
                        <label class="form-label">Remarks</label>
                        <input type="text" class="form-control" value="${formData.Remarks}" readonly>
                    </div>
                </div>
            </div>`;

            // Delete button with fixed positioning and z-index
            contactHtml += `
                <button type="button" class="btn btn-danger btn-sm delete-contact" style="position: absolute; top: 10px; right: 10px; z-index: 100;">
                    <i class="fa fa-trash"></i>
                </button>
            `;

            contactDiv.innerHTML = contactHtml;

            // Add delete functionality
            contactDiv.querySelector('.delete-contact').addEventListener('click', function () {
                // Find and remove the corresponding object in contactsArray
                const index = contactsArray.findIndex(
                    contact =>
                        contact.Name === formData.Name &&
                        contact.Email === formData.Email &&
                        contact.Email2 === formData.Email2 &&
                        contact.Phone1 === formData.Phone1 &&
                        contact.Phone2 === formData.Phone2 &&
                        contact.Remarks === formData.Remarks
                );

                if (index !== -1) {
                    contactsArray.splice(index, 1);
                }

                console.log('Contact deleted:', formData);
                console.log('Updated contactsArray after deletion:', contactsArray);

                contactDiv.remove();
            });

            // Add to bottom
            addedContacts.insertBefore(contactDiv, addedContacts.firstChild);

            // Clear original form
            inputs.forEach(input => {
                input.value = '';
            });
        });

        // Save Button Handler
                $("#saveButton").click(function (event) {
        event.preventDefault();

        // Perform custom validation for required fields
        var supplierName = $('#SupplierName').val().trim();
        var address = $('#Address').val().trim();

        if (!supplierName || !address) {
            Swal.fire("Error", "Please fill in all required fields: Supplier Name and Address.", "error");
            return;
        }

        Swal.fire({
            title: 'Saving...',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });

        var data = {
            Supplier: {
                SupplierName: supplierName,
                Address: address
            },
            Contacts: contactsArray
        };

        console.log('Data being sent:', data);

        $.ajax({
            url: '/Suppliers/Create',
            type: 'POST',
            data: JSON.stringify(data),
            contentType: 'application/json',
            success: function (response) {
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Saved Successfully!",
                    timer: 1500,
                    showConfirmButton: false,
                    customClass: {
                        popup: 'small-alert'
                    }
                });
                $('#supplierForm')[0].reset();
                $('#addedContacts').empty();
                contactsArray.length = 0;
            },
            error: function () {
                Swal.fire({
                    position: "top-end",
                    icon: "error",
                    title: "Failed",
                    timer: 1500,
                    showConfirmButton: false,
                    customClass: {
                        popup: 'small-alert'
                    }
                });
            }
        });
    });

    });
