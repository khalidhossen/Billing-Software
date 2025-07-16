$(document).ready(function () {
    // Trigger suggestion on typing
    
    $("#Phone").on("input", function () {
        let phone = $(this).val();
        if (phone.length >= 4) {
            $.ajax({
                url: "/Invoice/SearchPhone?phone=" + phone,
                method: "GET",
                success: function (res) {
                    let suggestions = $("#phoneSuggestions");
                    suggestions.empty();

                    if (res.length > 0) {
                        res.forEach(function (item) {
                            suggestions.append(`<a href="#" class="list-group-item list-group-item-action phone-suggestion" data-id="${item.customerId}">${item.phone}</a>`);
                        });
                        suggestions.show();
                    } else {
                        suggestions.hide();
                    }
                }
            });
        }
    });

    // On click suggestion
    $(document).on("click", ".phone-suggestion", function (e) {
        e.preventDefault();
        let customerId = $(this).data("id");
        /*alert("customerId = " + customerId);*/

        $.ajax({
            url: "/Invoice/GetCustomerDetails?customerId=" + customerId,
            method: "GET",
            success: function (data) {
                console.log(data)
                $("#FullName").val(data.fullName);
                $("#Phone").val(data.phone);
                $("#Email").val(data.email);
                $("#Address").val(data.address);
                $("#phoneSuggestions").hide();
            }
        });
    });

    // Hide suggestions on outside click
    $(document).on("click", function (e) {
        if (!$(e.target).closest("#Phone, #phoneSuggestions").length) {
            $("#phoneSuggestions").hide();
        }
    });

    $("#productName").on("input", function () {
        let product = $(this).val();
        if (product.length >= 2) {
            $.ajax({
                url: "/Invoice/SearchProduct?product=" + product,
                method: "GET",
                success: function (res) {
                    let suggestions = $("#productSuggestions");
                    suggestions.empty();

                    if (res.length > 0) {
                        res.forEach(function (item) {
                            suggestions.append(`<a href="#" class="list-group-item list-group-item-action product-suggestion" data-id="${item.productId}">${item.product}</a>`);
                            

                        });
                        suggestions.show();
                    } else {
                        suggestions.hide();
                    }
                }
            });
        }
    });

    // On click suggestion
    $(document).on("click", ".product-suggestion", function (e) {
        e.preventDefault();
        let productId = $(this).data("id");
        /*alert("customerId = " + customerId);*/

        $.ajax({
            url: "/Invoice/GetProductDetails?productId=" + productId,
            method: "GET",
            success: function (data) {
                debugger;

                $("#productName").val(data.product);
                $("#Category").val(data.category);
                $("#productSuggestions").hide();
            }
        });
    });

    // Hide suggestions on outside click
    $(document).on("click", function (e) {
        if (!$(e.target).closest("#productName, #productSuggestions").length) {
            $("#productSuggestions").hide();
        }
    });

    // Auto-calculate total price on input
    function updateTotalPrice() {
        const quantity = parseFloat($("input[name='Unit']").val()) || 0;
        const price = parseFloat($("input[name='Price']").val()) || 0;
        const discount = parseFloat($("input[name='Discount']").val()) || 0;

        const total = (price * quantity) - discount;
        $("input[name='TotalPrice']").val(total.toFixed(2));
    }

    // Listen for input changes to recalculate total price
    $("input[name='Unit'], input[name='Price'], input[name='Discount']").on("input", updateTotalPrice);


    // Calculate full summary
    function calculateSummary() {
        let subtotal = 0;
        let Discount = 0;
        let paid = 0;

        // Iterate over each row in the table
        $("#orderListTable tr").each(function () {
            const rowTotal = parseFloat($(this).find("td:eq(5)").text().replace("Tk", "").trim()) || 0;
            const rowDiscount = parseFloat($(this).find("td:eq(4)").text().replace("Tk", "").trim()) || 0;

            subtotal += rowTotal;
            Discount += rowDiscount;
        });

        const extraDiscount = parseFloat($("#extraDiscount").val()) || 0;
        const totalDiscount = Discount + extraDiscount;

        // 🔧 Fix: subtract both regular discount and extra discount
        const grandTotal = subtotal - extraDiscount;

        const payment = parseFloat($("#payment").val()) || 0;
        let due = 0;
        let exchange = 0;

        if (payment > grandTotal) {
            exchange = payment - grandTotal;
            due = 0;
            paid = grandTotal;
        } else {
            due = grandTotal - payment;
            exchange = 0;
            paid = payment;
        }

        // Update the summary fields
        $("#subtotal").val(subtotal.toFixed(2));
        $("#discount").val(Discount.toFixed(2));
       
        $("#grandTotal").val(grandTotal.toFixed(2));
        $("#totalDiscount").val(totalDiscount.toFixed(2));
        $("#due").val(due.toFixed(2));
        $("#paid").val(paid.toFixed(2));
        $("#exchange").val(exchange.toFixed(2));
    }

    // Recalculate grand total if manual discount is updated
    $("#extraDiscount").on("input", calculateSummary);

    // Recalculate due when payment amount is entered
    $("#payment").on("input", calculateSummary);
    // Add item to table
    /*let currentInvoiceNumber = parseInt($("input[name='InvoiceNumber']").val())|| 10;*/ // Starting from 100

    $(".btn-danger").on("click", function (e) {
        e.preventDefault();

        const productName = $("input[name='ProductName']").val();
        
        const category = $("select[name='Category'] option:selected").text();
        const quantity = parseFloat($("input[name='Unit']").val()) || 0;
        const price = parseFloat($("input[name='Price']").val()) || 0;
        const discount = parseFloat($("input[name='Discount']").val()) || 0;
        const totalPrice = (price * quantity) - discount;

        // Validation
        if (!productName || quantity <= 0 || price <= 0) {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: 'Please fill in valid product name. Quantity, and price are always positive.',
            });
            return;
        }

        // Append row
        const newRow = `
        <tr>
            <td>${productName}</td>
            <td>${category}</td>
            <td>${quantity}</td>
            <td>${price.toFixed(2)}Tk</td>
            <td>${discount.toFixed(2)}Tk</td>
            <td>${totalPrice.toFixed(2)}Tk</td>
            <td>
                <button class="btn btn-sm btn-danger d-flex align-items-center remove-item">
                    <i class="fas fa-trash-alt" style="font-size: 0.8rem;"></i> Remove
                </button>
            </td>
        </tr>
    `;
        $("#orderListTable").append(newRow);

        // Increase invoice number
        //currentInvoiceNumber++;
        //$("input[name='InvoiceNumber']").val(currentInvoiceNumber); // Update input field

        // Clear fields
        $("input[name='ProductName']").val('');
        $("select[name='Category']").val('');
        $("input[name='Unit']").val('1');
        $("input[name='Price']").val('');
        $("input[name='Discount']").val('');
        $("input[name='TotalPrice']").val('');

        calculateSummary();
    });
    // Remove item from table and update subtotal
    $(document).on("click", ".remove-item", function () {
        $(this).closest("tr").remove();
        calculateSummary();
    });

    // Phone number validation (allow only numbers)
    $("#Phone").on("input", function () {
        this.value = this.value.replace(/[^0-9]/g, '').substring(0, 14);
    });
    //positive number
    //$(".payment, #subtotal, #grandTotal, #totalDiscount, #extraDiscount, #paid, #due, #exchange").on("input", function () {
    //    this.value = this.value.replace(/[^0-9.]/g, '');
    //    if (this.value.indexOf('.') !== -1) {
    //        this.value = this.value.substring(0, this.value.indexOf('.') + 3); // Allow only 2 decimal places
    //    }
    //});


    // Save Invoice
    $("#SaveButton").on("click", function (e) {
        e.preventDefault();

        // Collect Customer Info
        let customerFullName = $("#FullName").val().trim();
        let customerPhone = $("#Phone").val().trim();
        let customerEmail = $("#Email").val().trim();
        let customerAddress = $("#Address").val().trim();

        // Basic validation
        if (!customerFullName || !customerPhone || !customerAddress) {
            Swal.fire('Error', 'Name, Phone, and Address are required.', 'error');
            return;
        }

        if ($("#orderListTable tr").length === 0) {
            Swal.fire('Error', 'Please add at least one product before saving.', 'error');
            return;
        }

        let grandTotal = parseFloat($("#grandTotal").val()) || 0;
        if (grandTotal <= 0) {
            Swal.fire('Error', 'Grand Total must be greater than 0.', 'error');
            return;
        }

        // Collect Products
        let products = [];
        $("#orderListTable tr").each(function () {
            let product = {
                ProductName: $(this).find("td:eq(0)").text(),
                Category: $(this).find("td:eq(1)").text(),
                Quantity: parseFloat($(this).find("td:eq(2)").text()) || 0,
                Price: parseFloat($(this).find("td:eq(3)").text().replace('Tk', '').trim()) || 0,
                Discount: parseFloat($(this).find("td:eq(4)").text().replace('Tk', '').trim()) || 0,
                TotalPrice: parseFloat($(this).find("td:eq(5)").text().replace('Tk', '').trim()) || 0
            };
            products.push(product);
        });

        // Summary
        let invoiceNumber = parseInt($("#InvoiceNumber").val()) || 0;
        let subtotal = parseFloat($("#subtotal").val()) || 0;
        let discount = parseFloat($("#discount").val()) || 0;
        let extraDiscount = parseFloat($("#extraDiscount").val()) || 0;
        let totalDiscount = parseFloat($("#totalDiscount").val()) || 0;
        let payAmount = parseFloat($("#payment").val()) || 0;
        let dueAmount = parseFloat($("#due").val()) || 0;
        let exchangeAmount = parseFloat($("#exchange").val()) || 0;
        let paid = parseFloat($("#paid").val()) || 0;
        let paymentMethod = $("input[name='paymentMethod']:checked").val();

        // Full invoice object
        let invoice = {
            CustomerFullName: customerFullName,
            CustomerPhone: customerPhone,
            CustomerEmail: customerEmail,
            CustomerAddress: customerAddress,
            Products: products,
            InvoiceNumber: invoiceNumber,
            Subtotal: subtotal,
            Discount: discount,
            ExtraDiscount: extraDiscount,
            GrandTotal: grandTotal,
            TotalDiscount: totalDiscount,
            PayAmount: payAmount,
            DueAmount: dueAmount,
            ExchangeAmount: exchangeAmount,
            Paid: paid,
            PaymentMethod: paymentMethod
        };

        // 1️⃣ Save the invoice
        $.ajax({
            url: "/Invoice/InvoiceCreate",
            method: "POST",
            contentType: "application/json",
            data: JSON.stringify(invoice),
            success: function (response) {
                if (response.success) {
                    // 2️⃣ Generate PDF after successful save
                    $.ajax({
                        url: "/Invoice/GenerateInvoicePdf",
                        method: "POST",
                        contentType: "application/json",
                        data: JSON.stringify(invoice),
                        xhrFields: {
                            responseType: 'blob'
                        },
                        success: function (blob) {
                            // ✅ Show popup
                            Swal.fire('Success', 'Invoice saved and PDF downloaded.', 'success');

                            // ✅ Download PDF
                            const url = window.URL.createObjectURL(blob);
                            const a = document.createElement('a');
                            a.href = url;
                            a.download = `Invoice_${invoice.InvoiceNumber}.pdf`;
                            document.body.appendChild(a);
                            a.click();
                            a.remove();

                            // ✅ Reset form
                            $("input[name='InvoiceNumber']").val(response.nextInvoiceNumber);
                            $("#FullName, #Phone, #Email, #Address").val('');
                            $("#orderListTable").empty();
                            $("#subtotal, #discount, #extraDiscount, #grandTotal, #totalDiscount, #payment, #due, #exchange, #paid").val('');
                        },
                        error: function () {
                            Swal.fire('Error', 'Invoice saved but PDF generation failed.', 'error');
                        }
                    });
                } else {
                    Swal.fire('Error', response.message, 'error');
                }
            },
            error: function () {
                Swal.fire('Error', 'Something went wrong!', 'error');
            }
        });
    });



    //negative value check
    function validatePositiveInput(selector) {
        $(selector).on("keydown", function (e) {
            // Block minus sign key press
            if (e.key === "-") {
                e.preventDefault();
                Swal.fire({
                    icon: 'error',
                    title: 'Invalid Input',
                    text: 'Negative values are not allowed.',
                    timer: 1000, // Auto-close after 1 seconds
                    showConfirmButton: false
                });
            }
        });

        $(selector).on("input", function () {
            const inputField = $(this);
            const originalValue = inputField.val();

            // If pasted or entered manually a negative sign
            if (originalValue.includes('-')) {
                inputField.val('');
                Swal.fire({
                    icon: 'error',
                    title: 'Invalid Input',
                    text: 'Negative values are not allowed.',
                    timer: 1000, // Auto-close after 3 seconds
                    showConfirmButton: false
                });
                return;
            }

            const value = parseFloat(inputField.val());

            if (isNaN(value)) return;

            // Send to server for backend validation
            $.ajax({
                url: "/Invoice/NegCheck",
                method: "POST",
                contentType: "application/json",
                data: JSON.stringify(value),
                success: function (response) {
                    if (!response.success) {
                        Swal.fire({
                            icon: 'error',
                            title: 'Invalid Input',
                            text: response.message,
                            timer: 1000, // Auto-close after 1 seconds
                            showConfirmButton: false
                        });
                        inputField.val('');
                    }
                },
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'Could not validate input.',
                        timer: 1000, // Auto-close after 1 seconds
                        showConfirmButton: false
                    });
                }
            });
        });
    }



    // Call this function for each input field you want to protect
    validatePositiveInput("input[name='Price']");
    validatePositiveInput("input[name='Discount']");
    validatePositiveInput("input[name='Payment']");
    validatePositiveInput("input[name='ExtraDiscount']");
    validatePositiveInput("input[name='Unit']");

    $(document).on('click', '.delete-btn', function () {
        const button = $(this);
        const invoiceId = button.data('id');

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
                    url: "/Invoice/InvoiceDelete/" + invoiceId,
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

    // Handle pay button click
    $(".pay-btn").on("click", function (e) {
        e.preventDefault();

        const invoiceId = $(this).data("id");
        const invoiceNumber = $(this).data("number");
        const customerName = $(this).data("name");
        const dueAmount = parseFloat($(this).data("due"));
        const payAmount = parseFloat($(this).data("payamount"));
        const grandTotal = parseFloat($(this).data("grandtotal"));
        const paid = parseFloat($(this).data("paid"));

        // Get current local datetime in YYYY-MM-DDTHH:MM format
        //const now = new Date();
        //const pad = (n) => n.toString().padStart(2, '0');
        //const localNow = `${now.getFullYear()}-${pad(now.getMonth() + 1)}-${pad(now.getDate())}T${pad(now.getHours())}:${pad(now.getMinutes())}`;
        //console.log("Local datetime value:", localNow);

        $("#invoiceId").val(invoiceId);
        $("#modalInvoiceNumber").val(invoiceNumber);
        $("#modalCustomerName").val(customerName);
        $("#modalDueAmount").val(dueAmount.toFixed(2));
        $("#modalGrandTotal").val(grandTotal.toFixed(2));
        /*$("#modalPayDate").val(localNow); // set default to today*/
        $("#modalPaidAmount").val(paid.toFixed(2)).data("original", paid);
        $("#modalUpdatedDueAmount").val(dueAmount.toFixed(2));

        $("#payDueModal").modal("show");
    });


// Handle PayAmount input changes
    $('input[name="PayAmount"]').on("input", function () {
        let cleaned = $(this).val().replace(/[^0-9.]/g, '');

        // Allow only one dot
        if ((cleaned.match(/\./g) || []).length > 1) {
            cleaned = cleaned.replace(/\.+$/, '');
        }

        $(this).val(cleaned);

        const inputPay = parseFloat($(this).val()) || 0;
        const grandTotal = parseFloat($("#modalGrandTotal").val()) || 0;
        const alreadyPaid = parseFloat($("#modalPaidAmount").data("original")) || 0;

        const newTotalPaid = alreadyPaid + inputPay;
        let updatedDue = grandTotal - newTotalPaid;
        if (updatedDue < 0) updatedDue = 0;

        $("#modalCurrentPayAmount").val(inputPay.toFixed(2));
        $("#modalPaidAmount").val(newTotalPaid.toFixed(2));
        $("#modalUpdatedDueAmount").val(updatedDue.toFixed(2));
    });

    // Handle form submit and also create pdf
    $("#payDueForm").on("submit", function (e) {
        e.preventDefault();

        const payAmount = parseFloat($("[name='PayAmount']").val());
        if (isNaN(payAmount) || payAmount <= 0) {
            Swal.fire("Error", "Enter a valid amount greater than 0.", "error");
            return;
        }

        const dto = {
            InvoiceId: $("#invoiceId").val(),
            CurrentPay: payAmount,
            CurrentDue: parseFloat($("#modalUpdatedDueAmount").val()),
            PayDate: $("#modalPayDate").val()
        };

        $.ajax({
            url: "/Invoice/PayDue",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(dto),
            success: function (res) {
                if (res.success) {
                    const updated = res.updated;

                    // ✅ Update row
                    const row = $(`tr[data-id='${updated.invoiceId}']`);
                    row.find(".due-amount").fadeOut(150, function () {
                        $(this).text(updated.dueAmount.toFixed(2)).fadeIn(150);
                    });

                    if (updated.dueAmount <= 0) {
                        row.find(".pay-btn")
                            .removeClass("btn-info")
                            .addClass("btn-secondary")
                            .text("Paid")
                            .prop("disabled", true);
                    }

                    // ✅ Show success popup
                    Swal.fire({
                        icon: 'success',
                        title: 'Payment Successful',
                        text: 'Payment has been recorded and the PDF receipt will now be downloaded.',
                        timer: 2000,
                        showConfirmButton: false
                    }).then(() => {
                        // ✅ Download the PDF after the popup
                        downloadPaymentPdf(dto);

                        // ✅ Close modal and clear form
                        $("#payDueModal").modal("hide");
                        $("#payDueForm")[0].reset();
                        $("[name='PayAmount']").val("");
                    });
                } else {
                    Swal.fire("Error", res.message, "error");
                }
            },
            error: function () {
                Swal.fire("Error", "Something went wrong.", "error");
            }
        });
    });

    function downloadPaymentPdf(dto) {
        const pdfData = {
            InvoiceNumber: $("#modalInvoiceNumber").val(),
            CustomerName: $("#modalCustomerName").val(),
            GrandTotal: parseFloat($("#modalGrandTotal").val()),
            PayDate: $("#modalPayDate").val(),
            DueAmount: parseFloat($("#modalDueAmount").val()),
            PaidAmount: parseFloat($("#modalPaidAmount").val()),
            UpdatedDueAmount: parseFloat($("#modalUpdatedDueAmount").val()),
            PayAmount: dto.CurrentPay
        };

        $.ajax({
            url: '/Invoice/DownloadPayDuePdf',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(pdfData),
            xhrFields: { responseType: 'blob' },
            success: function (blob) {
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = `PayDue_${pdfData.InvoiceNumber}.pdf`;
                document.body.appendChild(a);
                a.click();
                a.remove();
            },
            error: function () {
                Swal.fire('Error', 'Failed to generate PDF.', 'error');
            }
        });
    }


    //details posrtion
    // Use event delegation to handle dynamically loaded buttons
    // delegated click handler
    $(document).on('click', '.details-btn', function () {
        const invoiceId = $(this).data('id');

        $.ajax({
            url: `/Invoice/GetInvoiceDetails/${invoiceId}`,
            method: 'GET',
            success: function (response) {
                if (response.success) {
                    const invoice = response.data;

                    $('#detailInvoiceNumber').text(invoice.invoiceNumber);
                    $('#detailCustomerName').text(invoice.customerName);
                    $('#detailCustomerPhone').text(invoice.customerPhone);
                    $('#detailCustomerAddress').text(invoice.customerAddress);
                    $('#detailInvoiceDate').text(formatDate(invoice.invoiceDate));
                    $('#detailGrandTotal').text(invoice.grandTotal.toFixed(2));
                    $('#detailTotalDiscount').text(invoice.totalDiscount.toFixed(2));
                    $('#detailPayAmount').text(invoice.payAmount.toFixed(2));
                    $('#detailExchangeAmount').text(invoice.exchangeAmount.toFixed(2));
                    $('#detailDueAmount').text(invoice.dueAmount.toFixed(2));
                    $('#detailPaid').text(invoice.paid.toFixed(2));

                    const tbody = $('#productTableBody');
                    tbody.empty();
                    invoice.products.forEach(p => {
                        tbody.append(`
                                        <tr>
                                            <td>${p.productName}</td>
                                            <td>${p.category}</td>
                                            <td>${p.quantity}</td>
                                            <td>${p.price}</td>
                                            <td>${p.discount}</td>
                                            <td>${p.totalPrice}</td>
                                        </tr>
                                    `);
                    });

                    $('#invoiceDetailsModal').modal('show');
                } else {
                    alert(response.msg);
                }
            },
            error: function () {
                alert('Failed to load invoice details.');
            }
        });
    });

    function formatDate(dateStr) {
        const date = new Date(dateStr);
        return date.toLocaleDateString();
    }

});