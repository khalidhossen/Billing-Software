using Firo.Application.Models;      
using Firo.Application.ViewModels;
using Firo.Common.Services;
using Firo.Domain.Entities;
using Firo.Domain.Interfaces;
using Firo.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Drawing.Printing;
using iTextSharp.text.pdf.draw;

namespace Firo.Web.Areas.Admin.Controllers
{
    [Area("Admin"), Route("Invoice")]
    public class InvoiceController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICompanyProfileRepository _companyProfileRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly ILookUpRepository _lookUpRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPayDueRepository _payDueRepository;
        public InvoiceController(
            IInvoiceRepository invoiceRepository,
            ICustomerRepository customerRepository,
            ICompanyProfileRepository companyProfileRepository,
            IBranchRepository branchRepository,
            ILookUpRepository lookUpRepository,
            IProductRepository productRepository,
            IPayDueRepository payDueRepository)
        {
            _invoiceRepository = invoiceRepository;
            _customerRepository = customerRepository;
            _companyProfileRepository = companyProfileRepository;
            _branchRepository = branchRepository;
            _lookUpRepository = lookUpRepository;
            _productRepository = productRepository;
            _payDueRepository = payDueRepository;

        }

        private Guid currUserGuid => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userGuid) ? userGuid : Guid.Empty;

        [HttpGet]
        [Route("InvoiceList")]
        public async Task<IActionResult> InvoiceList(int page=1,int pageSize=4)
        {
            var invoices= await _invoiceRepository.GetInvoicesPagedAsync( page, pageSize);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("/Areas/Admin/Views/PartialViews/_InvoiceTablePartial.cshtml", invoices);
            }

            return View(invoices);
        }

        [Route("InvoiceCreate")]
        public async Task<IActionResult> InvoiceCreate()
        {
            var paymentMethods = await _lookUpRepository.GetByDataKeyAsync("PaymentMethod");
            ViewBag.PaymentMethods = paymentMethods;
            var categories = await _lookUpRepository.GetByDataKeyAsync("CategoryType");
            ViewBag.Categories = categories;

            int nextInvoiceNumber = await _invoiceRepository.GetNextInvoiceNumberAsync();
            ViewBag.NextInvoiceNumber = nextInvoiceNumber;

            return View();
        }

        [HttpPost("InvoiceCreate")]
        public async Task<IActionResult> InvoiceCreate([FromBody] InvoiceVM model)
        {
            if (!string.IsNullOrEmpty(model.CustomerEmail))
            {
                var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!emailRegex.IsMatch(model.CustomerEmail))
                {
                    return Json(new { success = false, message = "Invalid email format." });
                }
            }

            // Phone number validation
            if (!string.IsNullOrEmpty(model.CustomerPhone))
            {
                var phoneRegex = new System.Text.RegularExpressions.Regex(@"^\d{1,14}$");
                if (!phoneRegex.IsMatch(model.CustomerPhone))
                {
                    return Json(new { success = false, message = "Invalid phone number. Only numbers allowed and maximum 14 digits." });
                }
            }

            // Products validation
            foreach (var product in model.Products)
            {
                if (product.Price < 0 || product.Discount < 0 || product.TotalPrice < 0 || product.Quantity < 0)
                {
                    return Json(new { success = false, message = "Product price, discount, quantity, and total price must be positive numbers." });
                }
            }

            
            var customerId = Guid.NewGuid();
            var currentDate = DateTime.Now;

            CustomerDto customerDto;

            // Step 2: Check for existing customer
            var existingCustomer = await _customerRepository.GetByEmailOrPhoneAsync(model.CustomerEmail, model.CustomerPhone);

            if (existingCustomer != null)
            {
                // Reuse the existing customerId
                customerDto = new CustomerDto
                {
                    CustomerId = existingCustomer.CustomerId, // Use existing
                    FullName = existingCustomer.FullName,
                    Phone = existingCustomer.Phone,
                    Email = existingCustomer.Email,
                    Address = existingCustomer.Address,
                    CreatedAt = existingCustomer.CreatedAt,
                    UpdatedAt = currentDate
                };
            }
            else
            {
                // New customer, create new customerId
                customerDto = new CustomerDto
                {
                    CustomerId = customerId, // New one only if not exist
                    FullName = model.CustomerFullName,
                    Phone = model.CustomerPhone,
                    Email = model.CustomerEmail,
                    Address = model.CustomerAddress,
                    CreatedAt = currentDate,
                    UpdatedAt = currentDate
                };

                customerDto = await _customerRepository.AddCustomerAsync(customerDto); // Save new customer
            }

            // Step 3: Create and Save Invoice
            var invoiceDto = new InvoiceDto
            {
                CustomerId = customerDto.CustomerId,// Same CustomerId as used above
                InvoiceNumber = model.InvoiceNumber,
                Subtotal = model.Subtotal,
                Discount = model.Discount,
                ExtraDiscount = model.ExtraDiscount,
                TotalDiscount = model.TotalDiscount,
                GrandTotal = model.GrandTotal,
                PayAmount = model.PayAmount,
                DueAmount = model.DueAmount,
                ExchangeAmount = model.ExchangeAmount,
                Paid = model.Paid,
                PaymentMethod = model.PaymentMethod,
                InvoiceDate = currentDate,
                DueDate = currentDate.AddDays(7), // Example
                CreatedAt = currentDate
            };

           var newinvoice =  await _invoiceRepository.AddInvoiceAsync(invoiceDto); // Save the invoice

            // Step 4: Save each product
            foreach (var product in model.Products)
            {
                var productDto = new ProductDto
                {
                    ProductId = Guid.NewGuid(),
                    InvoiceId = newinvoice.InvoiceId, // Reusing the same InvoiceId
                    CustomerId = customerDto.CustomerId, // Reusing the same CustomerId
                    ProductName = product.ProductName,
                    
                    Category = product.Category,
                    Quantity = product.Quantity,
                    Price = product.Price,
                    Discount = product.Discount,
                    TotalPrice = product.TotalPrice,
                    CreatedAt = currentDate
                };

                await _productRepository.AddProductAsync(productDto); // Save the product
            }
            // ✅ Return the next invoice number
            var nextInvoiceNumber = await _invoiceRepository.GetNextInvoiceNumberAsync();

            return Json(new
            {
                success = true,
                message = "Data Saved Successfully.",
                nextInvoiceNumber = nextInvoiceNumber
            });
        }

        [HttpGet("SearchPhone")]
        public async Task<IActionResult> SearchPhone(string phone)
      {
            var matches = await _customerRepository.SearchByPhoneAsync(phone);
            // return minimal array of { customerId, phone }
            var result = matches
              .Select(c => new { customerId = c.CustomerId, phone = c.Phone })
              .ToList();
            return Json(result);
        }

        [HttpPost("NegCheck")]
        public IActionResult NegCheck([FromBody] decimal input)
        {
            if (input < 0)
            {
                return Json(new { success = false, message = "Negative values are not allowed." });
            }
            return Json(new { success = true });
        }

        [HttpGet("GetCustomerDetails")]
        public async Task<IActionResult> GetCustomerDetails(Guid customerId)
        {
            var c = await _customerRepository.GetByIdAsync(customerId);
            if (c == null) return NotFound();
            return Json(new
            {
                customerId = c.CustomerId,
                fullName = c.FullName,
                phone = c.Phone,
                email = c.Email,
                address = c.Address
            });
        }
        [HttpGet("SearchProduct")]
        public async Task<IActionResult> SearchProduct(string product)
        {
            var matches = await _productRepository.SearchByProductAsync(product);
            var result = matches
                .GroupBy(c => c.ProductName)
                .Select(g => new { productId = g.First().ProductId, product = g.Key })
                .ToList();
            return Json(result);
        }

        [HttpGet("GetProductDetails")]
        public async Task<IActionResult> GetProductDetails(Guid productId)
        {
            var c = await _productRepository.GetByIdAsync(productId);
            if (c == null) return NotFound();
            
            return Json(new
            {
                productId = c.ProductId,
                product=c.ProductName,
                category=c.Category
                
            });
        }
        [HttpPost("InvoiceDelete/{invoiceId}")]
        public async Task<IActionResult> InvoiceDelete(Guid invoiceId)
        {
            bool deleted = await _invoiceRepository.DeleteInvoiceAsync(invoiceId);
            if (deleted == false)
            {
                return Json(new { success = false, msg = "Invoice not found!" });
            }
            else
            {
                return Json(new
                {
                    success = deleted,
                    msg = deleted ? "Invoice deleted successfully!" : "Failed to delete Invoice!"
                });
            }

        }

        [HttpGet("GetInvoiceDetails/{invoiceId}")]
        public async Task<IActionResult> GetInvoiceDetails(Guid invoiceId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);

            if (invoice == null)
            {
                return Json(new { success = false, msg = "Invoice not found!" });
            }

            return Json(new
            {
                success = true,
                data = invoice
            });
        }

        [HttpGet]
        [Route("DueList")]
        public async Task<IActionResult> DueList(int page = 1, int pageSize = 4)
        {
            var dueInvoices = await _invoiceRepository.GetDueInvoicesPagedAsync(page, pageSize);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("/Areas/Admin/Views/PartialViews/_DueTablePartial.cshtml", dueInvoices);
            }

            return View(dueInvoices);
        }

        [HttpPost("PayDue")]
        public async Task<IActionResult> PayDue([FromBody] PayDueDto dto)
        {
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(dto.InvoiceId);
            if (invoice == null)
                return Json(new { success = false, message = "Invoice not found." });

            if (dto.CurrentPay <= 0)
                return Json(new { success = false, message = "Pay amount must be positive." });

            if (dto.CurrentPay > invoice.DueAmount)
                return Json(new { success = false, message = "Cannot pay more than due amount." });

            // Update invoice
            invoice.PayAmount += dto.CurrentPay;
            invoice.DueAmount -= dto.CurrentPay;
            invoice.Paid += dto.CurrentPay;
            invoice.UpdatedAt = DateTime.UtcNow;

            await _invoiceRepository.UpdateInvoiceAsync(invoice);

            // Create new PayDue entry
            dto.CustomerId = invoice.CustomerId;
            dto.CreatedBy = Guid.NewGuid(); // Replace with logged-in user if needed
            dto.UpdatedBy = dto.CreatedBy;

            await _payDueRepository.CreateAsync(dto);

            return Json(new
  {
    success = true,
    message = "Payment successful.",
    updated = new
    {
        invoiceId = invoice.InvoiceId,
        dueAmount = invoice.DueAmount,
        paid = invoice.Paid
    }
});
        }

        [HttpGet]
        [Route("PayList")]
        public async Task<IActionResult> PayList(int page = 1, int pageSize = 4)
        {
            var allPays = await _payDueRepository.GetAllAsync(page,pageSize);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("/Areas/Admin/Views/PartialViews/_PayTablePartial.cshtml", allPays);
            }

            return View(allPays);
        }
        [HttpPost("DownloadPayDuePdf")]
        public IActionResult DownloadPayDuePdf([FromBody] PayDueVM model)
        {
            using var stream = new MemoryStream();
            var document = new Document(PageSize.A4, 36, 36, 80, 36);
            PdfWriter.GetInstance(document, stream);
            document.Open();

            // Fonts
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.Black);
            var subTitleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.DarkGray);
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            var smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);

            PdfPCell CreateHeaderCell(string text) => new PdfPCell(new Phrase(text, headerFont))
            {
                BackgroundColor = new BaseColor(230, 230, 250),
                BorderColor = BaseColor.LightGray,
                Padding = 6,
                HorizontalAlignment = Element.ALIGN_LEFT
            };

            PdfPCell CreateDataCell(string text) => new PdfPCell(new Phrase(text ?? "", normalFont))
            {
                BackgroundColor = BaseColor.White,
                BorderColor = BaseColor.LightGray,
                Padding = 6,
                HorizontalAlignment = Element.ALIGN_LEFT
            };

            // -------------------- Company Header --------------------
            PdfPTable headerTable = new PdfPTable(2) { WidthPercentage = 100 };
            headerTable.SetWidths(new float[] { 20, 80 });

            string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", "logo.png");
            if (System.IO.File.Exists(logoPath))
            {
                var logo = iTextSharp.text.Image.GetInstance(logoPath);
                logo.ScaleToFit(60f, 60f);
                headerTable.AddCell(new PdfPCell(logo)
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT
                });
            }
            else
            {
                headerTable.AddCell(new PdfPCell(new Phrase("FIROTECH", titleFont))
                {
                    Border = Rectangle.NO_BORDER,
                    PaddingTop = 15
                });
            }

            headerTable.AddCell(new PdfPCell(new Phrase("FIROTECH\nInnovating Billing Solutions\nEmail: support@firotech.com\nPhone: 0123456789", normalFont))
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            });

            document.Add(headerTable);
            document.Add(new Paragraph("\n"));

            // -------------------- Title --------------------
            var receiptTitle = new Paragraph("PAYMENT RECEIPT", subTitleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 10f
            };
            document.Add(receiptTitle);

            var line = new LineSeparator(1f, 100f, BaseColor.LightGray, Element.ALIGN_CENTER, -2);
            document.Add(new Chunk(line));
            document.Add(new Paragraph("\n"));

            // -------------------- Voucher-style Box --------------------
            PdfPTable borderedTable = new PdfPTable(2) { WidthPercentage = 90, HorizontalAlignment = Element.ALIGN_CENTER };
            borderedTable.SetWidths(new float[] { 40, 60 });
            borderedTable.SpacingBefore = 10f;
            borderedTable.SpacingAfter = 20f;

            void AddRow(string label, string value)
            {
                borderedTable.AddCell(CreateHeaderCell(label));
                borderedTable.AddCell(CreateDataCell(value));
            }

            AddRow("Invoice Number", model.InvoiceNumber.ToString());
            AddRow("Customer Name", model.CustomerName);
            AddRow("Pay Date", model.PayDate.ToString("dd-MM-yyyy hh:mm tt"));
            AddRow("Grand Total", model.GrandTotal.ToString("F2") + " Tk");
            AddRow("Previous Due", model.DueAmount.ToString("F2") + " Tk");
            AddRow("Previously Paid", model.PaidAmount.ToString("F2") + " Tk");
            AddRow("New Payment", model.PayAmount.ToString("F2") + " Tk");
            AddRow("Updated Due", model.UpdatedDueAmount.ToString("F2") + " Tk");

            document.Add(borderedTable);

            // -------------------- Thank You Footer --------------------
            var footer = new PdfPTable(1) { WidthPercentage = 100 };
            var footerCell = new PdfPCell(new Phrase("Thank you for your payment!", smallFont))
            {
                BackgroundColor = new BaseColor(245, 245, 245),
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 10
            };
            footer.AddCell(footerCell);
            document.Add(footer);

            document.Close();
            return File(stream.ToArray(), "application/pdf", $"PayDue_{model.InvoiceNumber}.pdf");
        }




        [HttpPost("GenerateInvoicePdf")]
        public IActionResult GenerateInvoicePdf([FromBody] InvoiceVM model)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 36, 36, 80, 36);
                PdfWriter writer = PdfWriter.GetInstance(document, stream);
                document.Open();

                // Fonts
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                var subTitleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                var smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);

                // Reusable cell creators
                PdfPCell CreateHeaderCell(string text) => new PdfPCell(new Phrase(text, headerFont))
                {
                    BackgroundColor = new BaseColor(240, 240, 240),
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5
                };

                PdfPCell CreateDataCell(string text) => new PdfPCell(new Phrase(text ?? "", normalFont))
                {
                    Padding = 5,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };

                // -------------------- Company Header --------------------
                PdfPTable headerTable = new PdfPTable(2) { WidthPercentage = 100 };
                headerTable.SetWidths(new float[] { 20, 80 });

                string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", "logo.png");
                if (System.IO.File.Exists(logoPath))
                {
                    iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                    logo.ScaleToFit(60f, 60f);
                    PdfPCell logoCell = new PdfPCell(logo)
                    {
                        Border = Rectangle.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    headerTable.AddCell(logoCell);
                }
                else
                {
                    headerTable.AddCell(new PdfPCell(new Phrase("FIROTECH", titleFont))
                    {
                        Border = Rectangle.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        PaddingTop = 15
                    });
                }

                headerTable.AddCell(new PdfPCell(new Phrase("FIROTECH\nInnovating Billing Solutions\nEmail: support@firotech.com\nPhone: 0123456789", normalFont))
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });

                document.Add(headerTable);
                document.Add(new Paragraph("\n"));

                // -------------------- Customer Info --------------------
                document.Add(new Paragraph("Customer Information", subTitleFont));
                PdfPTable customerTable = new PdfPTable(2) { WidthPercentage = 100 };
                customerTable.SpacingBefore = 10f;
                customerTable.SetWidths(new float[] { 30, 70 });

                customerTable.AddCell(CreateHeaderCell("Name"));
                customerTable.AddCell(CreateDataCell(model.CustomerFullName));
                customerTable.AddCell(CreateHeaderCell("Phone"));
                customerTable.AddCell(CreateDataCell(model.CustomerPhone));
                customerTable.AddCell(CreateHeaderCell("Email"));
                customerTable.AddCell(CreateDataCell(model.CustomerEmail));
                customerTable.AddCell(CreateHeaderCell("Address"));
                customerTable.AddCell(CreateDataCell(model.CustomerAddress));

                document.Add(customerTable);
                document.Add(new Paragraph("\n"));

                // -------------------- Invoice Info --------------------
                document.Add(new Paragraph("Invoice Information", subTitleFont));
                PdfPTable invoiceTable = new PdfPTable(2) { WidthPercentage = 100 };
                invoiceTable.SpacingBefore = 10f;
                invoiceTable.SetWidths(new float[] { 30, 70 });

                invoiceTable.AddCell(CreateHeaderCell("Invoice Number"));
                invoiceTable.AddCell(CreateDataCell(model.InvoiceNumber.ToString()));
                invoiceTable.AddCell(CreateHeaderCell("Payment Method"));
                invoiceTable.AddCell(CreateDataCell(model.PaymentMethod));

                document.Add(invoiceTable);
                document.Add(new Paragraph("\n"));

                // -------------------- Payment Summary --------------------
                document.Add(new Paragraph("Payment Summary", subTitleFont));
                PdfPTable paymentTable = new PdfPTable(2)
                {
                    WidthPercentage = 60,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    SpacingBefore = 10f
                };
                paymentTable.SetWidths(new float[] { 50, 50 });

                void AddSummaryRow(string label, decimal value)
                {
                    paymentTable.AddCell(new PdfPCell(new Phrase(label, headerFont)) { Border = Rectangle.NO_BORDER });
                    paymentTable.AddCell(new PdfPCell(new Phrase(value.ToString("F2"), normalFont))
                    {
                        Border = Rectangle.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    });
                }

                AddSummaryRow("Subtotal", model.Subtotal);
                AddSummaryRow("Discount", model.Discount);
                AddSummaryRow("Extra Discount", model.ExtraDiscount);
                AddSummaryRow("Total Discount", model.TotalDiscount);
                AddSummaryRow("Grand Total", model.GrandTotal);
                AddSummaryRow("Paid", model.Paid);
                AddSummaryRow("Due Amount", model.DueAmount);
                AddSummaryRow("Exchange Amount", model.ExchangeAmount);

                document.Add(paymentTable);

                // -------------------- Footer --------------------
                document.Add(new Paragraph("\n\nThank you for your business!", smallFont)
                {
                    Alignment = Element.ALIGN_CENTER
                });

                document.Close();
                byte[] pdfBytes = stream.ToArray();
                return File(pdfBytes, "application/pdf", $"Invoice_{model.InvoiceNumber}.pdf");
            }
        }




    }
}