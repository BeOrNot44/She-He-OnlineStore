using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using She___He_Store.Models;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace She___He_Store.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ModelContext _context;
        private readonly IHttpContextAccessor context1;
        private readonly IWebHostEnvironment _webHostEnviroment; //declare Variable
                                                                 //Update the constructer
        public OrderDetailsController(IConfiguration configuration,ModelContext context, IWebHostEnvironment webHostEnviroment, IHttpContextAccessor httpContextAccessor/*, IEmailSender emailSender*/) //declare IWebHostEnvironment parameter
        {
            //this._emailSender = emailSender;
            context1 = httpContextAccessor;
            _context = context;
            _webHostEnviroment = webHostEnviroment; //dependency injection
            _configuration = configuration;
        }

        // GET: OrderDetails
        public async Task<IActionResult> Index()
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            var modelContext = _context.OrderDetails.Include(o => o.Customer).Include(o => o.Product);

            var allBankcards = await _context.Bankcards.ToListAsync();
            ViewBag.Bankcards = allBankcards;

            return View(await modelContext.ToListAsync());
        }
        public IActionResult ShowHistory()
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            var orderDetails = _context.OrderDetails
            .Include(od => od.Customer) // Include the Customer navigation property
            .Include(od => od.Product)  // Include the Product navigation property
            .Where(od => od.Status != "OnCart").ToList();

            //var distinctDates = _context.OrderDetails
            //    .Select(od => od.OrderDate) // Retrieve the entire DateTime
            //    .Distinct()
            //    .ToList();

            //int distinctDateCount = distinctDates.Count;
            //ViewBag.DistinctDateCount = distinctDateCount;

            return View(orderDetails);
        }

        [HttpPost]
        public IActionResult ProcessPayment(decimal cardId, decimal? totalAmount)
        {
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");

            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");
            var bankcard = _context.Bankcards.Find(cardId);
            if (bankcard == null)
            {
                return RedirectToAction("Index");
            }
            if (totalAmount.HasValue && bankcard.Balance >= totalAmount.Value)
            {
                bankcard.Balance -= totalAmount.Value;
                _context.Update(bankcard);

                decimal? customerId = context1.HttpContext.Session.GetInt32("id_customer"); // Corrected variable name
                if (customerId.HasValue)
                {
                    var user = _context.UserLogins.FirstOrDefault(u => u.CustomerId == customerId);
                    if (user != null)
                    {
                        
                        // Send an email to the user
                        string subject = "Your Order Confirmation";
                        string message = "Dear " + ViewBag.CustomerName + ",\n\n"
                                       + "Thank you for placing your order with She & He Store. Your order has been successfully processed and will be on its way to you shortly.\n\n"
                                       + "Order Details:\n"
                                       + "Order Date: " + DateTime.Now.ToString("MMMM dd, yyyy") + "\n"
                                       + "Total Amount: $" + totalAmount.Value.ToString("0.00") + "\n\n"
                                       + "If you have any questions or need further assistance, please don't hesitate to contact our support team.\n\n"
                                       + "Support Team Contact Information:\n"
                                       + "Email: support@sheandhestore.com\n"
                                       + "Phone: +962 776-516-707.\n\n"
                                       + "We value your business and look forward to serving you again in the future.\n\n"
                                       + "Sincerely,\n"
                                       + "She & He Store Team";

                        SendEmail(user.UserName, subject, message);
                    }
                    //add the code to change item.Status to "pending"
                    var orderDetails = _context.OrderDetails
                        .Where(o => o.CustomerId == customerId && o.Status == "OnCart")
                        .ToList();

                    foreach (var orderDetail in orderDetails)
                    {
                        orderDetail.Status = "Pending";
                        orderDetail.OrderDate = DateTime.Now; // Update OrderDate to the current date and time
                        _context.Update(orderDetail);
                    }

                }
                _context.SaveChanges();
                return RedirectToRoute(new { controller = "Home", action = "Index" });
            }

            return RedirectToAction("Index");
        }

            [HttpPost]
            public async Task<IActionResult> SendEmail(string? to, string subject, string message)
            {
                try
                {
                    string smtpServer = _configuration["EmailSettings:SmtpServer"];
                    int smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                    string smtpUsername = _configuration["EmailSettings:SmtpUsername"];
                    string smtpPassword = _configuration["EmailSettings:SmtpPassword"];

                    using (var smtpClient = new SmtpClient(smtpServer, smtpPort))
                    {
                        smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                        smtpClient.EnableSsl = true;

                        using (var mailMessage = new MailMessage(smtpUsername, to)
                        {
                            Subject = subject,
                            Body = message,
                            IsBodyHtml = true
                        })
                        {
                            await smtpClient.SendMailAsync(mailMessage);
                        }
                    }

                    ViewBag.EmailSent = true;
                }
                catch (Exception ex)
                {
                    ViewBag.EmailSent = false;
                    // Handle and log the error
                    Console.WriteLine("Email sending error: " + ex.Message);
                }

                return View("Index"); // Assuming you want to return to the Index view
            }


    //[HttpPost]
    //public async Task<IActionResult> SendEmail(string to, string subject, string message)
    //{
    //    try
    //    {
    //        string apiKey = "Q92SsJRCg4jTm3FK";
    //        using (var httpClient = new HttpClient())
    //        {
    //            // Set the SendinBlue API endpoint
    //            var apiUrl = "https://api.sendinblue.com/v3/smtp/email";

    //            // Construct the email data
    //            var emailData = new
    //            {
    //                sender = new { name = "Admin", email = "raghadr4444r@gmail.com" },
    //                to = new[] { new { email = to } },
    //                subject = subject,
    //                htmlContent = message,
    //            };

    //            // Serialize the email data to JSON
    //            var emailJson = Newtonsoft.Json.JsonConvert.SerializeObject(emailData);

    //            // Create an HTTP request
    //            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
    //            request.Headers.Add("api-key", apiKey);
    //            request.Content = new StringContent(emailJson, Encoding.UTF8, "application/json");

    //            // Send the email
    //            var response = await httpClient.SendAsync(request);

    //            if (response.IsSuccessStatusCode)
    //            {
    //                ViewBag.EmailSent = true;
    //            }
    //            else
    //            {
    //                ViewBag.EmailSent = false;
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        ViewBag.EmailSent = false;
    //        Console.WriteLine("Email sending error: " + ex.Message);
    //    }

    //    // Redirect to your view where you can display the result
    //    return View("Index");
    //}







    // GET: OrderDetails/Details/5
    public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.OrderDetails == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(o => o.Customer)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // GET: OrderDetails/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
            return View();
        }

        // POST: OrderDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CustomerId,ProductId,Quantity,OrderDate,TotalAmount,Status,DeliveryDate")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", orderDetail.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", orderDetail.ProductId);
            return View(orderDetail);
        }

        // GET: OrderDetails/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.OrderDetails == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", orderDetail.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", orderDetail.ProductId);
            return View(orderDetail);
        }

        // POST: OrderDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,CustomerId,ProductId,Quantity,OrderDate,TotalAmount,Status,DeliveryDate")] OrderDetail orderDetail)
        {
            if (id != orderDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderDetailExists(orderDetail.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", orderDetail.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", orderDetail.ProductId);
            return View(orderDetail);
        }

        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.OrderDetails == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(o => o.Customer)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.OrderDetails == null)
            {
                return Problem("Entity set 'ModelContext.OrderDetails'  is null.");
            }
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail != null)
            {
                _context.OrderDetails.Remove(orderDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderDetailExists(decimal id)
        {
            return (_context.OrderDetails?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
