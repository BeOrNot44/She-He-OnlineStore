using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Controller;
using She___He_Store.Models;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using System.Text.Json;
using Newtonsoft.Json;
using NuGet.Configuration;

namespace She___He_Store.Controllers
{
    public class AdminController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ModelContext _context;
        private readonly IHttpContextAccessor context1;

        private readonly IWebHostEnvironment _webHostEnviroment; //declare Variable
        //Update the constructer
        public AdminController(ModelContext context, IWebHostEnvironment webHostEnviroment, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) //declare IWebHostEnvironment parameter
        {
            context1 = httpContextAccessor;
            _context = context;
            _webHostEnviroment = webHostEnviroment; //dependency injection
            _configuration = configuration;
        }
        //public IActionResult Index()
        //{
        //    var categories = _context.Customers.ToList(); //
        //    return View(categories); //
        //}

        [HttpGet]
        public IActionResult Dashboard()
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            ViewBag.TotalSale= _context.OrderDetails.Sum(o => o.TotalAmount ?? 0);
            DateTime today = DateTime.Today;
            ViewBag.TodaySale = _context.OrderDetails
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Date == today)
                .Sum(o => o.TotalAmount ?? 0);
            ViewBag.NumberofCustomer = _context.Customers.Count();
            ViewBag.NumberofTestimonials = _context.Testimonials.Count();
            ViewBag.Numberoforders = _context.OrderDetails.Count();

            ViewBag.Registrations = _context.Customers.ToList();
            ViewBag.Testimonials = _context.Testimonials.ToList();

            ViewBag.RegistrationDatesList = _context.Customers.Select(c => c.RegistrationDate).ToList();
            ViewBag.TestimonialDatesList = _context.Testimonials.Select(c => c.TestimonialDate).ToList();



            var customer = _context.Customers.ToList();
                var procustomers =_context.OrderDetails.ToList();
                var products = _context.Products.ToList();
                var categoreys = _context.Categories.ToList();
                var multiTable = from c in customer
                                 join pc in procustomers on c.Id equals pc.CustomerId
                                 join p in products on pc.ProductId equals p.Id
                                 join cat in categoreys on p.CategoryId equals cat.Id
                                 select new JoinTables
                                 {
                                     Product = p,
                                     Customer = c,
                                     OrderDetail = pc,
                                     Category = cat
                                 };
            var modelContext =_context.OrderDetails.Include(p =>p.Customer).Include(p => p.Product).ToList();
            ViewBag.TotalQuantity = modelContext.Sum(x =>x.Quantity);
            ViewBag.TotalPrice = modelContext.Sum(x =>x.Product.Price * x.Quantity);
            var model3 = Tuple.Create<IEnumerable<JoinTables>,IEnumerable<OrderDetail>>(multiTable,modelContext);
            return View(model3);
        }
        [HttpPost]
        public async Task<IActionResult> Dashboard(DateTime? OrderDate, DateTime? DeliveryDate)
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            var customer = _context.Customers.ToList();
            var procustomers =_context.OrderDetails.ToList();
            var products = _context.Products.ToList();
            var categoreys = _context.Categories.ToList();
            var multiTable = from c in customer
                             join pc in procustomers on c.Id equals pc.CustomerId
                             join p in products on pc.ProductId equals p.Id
                             join cat in categoreys on p.CategoryId equals cat.Id
                             select new JoinTables
                             {
                                 Product = p,
                                 Customer = c,
                                 OrderDetail = pc,
                                 Category = cat
                             };
            var modelContext = _context.OrderDetails.Include(p =>p.Customer).Include(p => p.Product);
            if (OrderDate == null && DeliveryDate == null)
            {
                ViewBag.TotalQuantity = modelContext.Sum(x =>x.Quantity);
                ViewBag.TotalPrice = modelContext.Sum(x =>x.Product.Price * x.Quantity);
                var model3 = Tuple.Create<IEnumerable<JoinTables>,IEnumerable<OrderDetail>>(multiTable, await modelContext.ToListAsync());
                return View(model3);
            }
            else if (OrderDate == null && DeliveryDate != null)
            {
                ViewBag.TotalQuantity = modelContext.Where(x =>x.OrderDate.Value.Date == DeliveryDate).Sum(x =>x.Quantity);
                ViewBag.TotalPrice = modelContext.Where(x =>x.OrderDate.Value.Date == DeliveryDate).Sum(x =>x.Product.Price * x.Quantity);
                var model3 = Tuple.Create<IEnumerable<JoinTables>,IEnumerable<OrderDetail>>(multiTable, await modelContext.Where(x => x.OrderDate.Value.Date == DeliveryDate).ToListAsync());
                return View(model3);
            }
            else if (OrderDate != null && DeliveryDate == null)
            {
                ViewBag.TotalQuantity = modelContext.Where(x =>x.OrderDate.Value.Date == OrderDate).Sum(x =>x.Quantity);
                ViewBag.TotalPrice = modelContext.Where(x =>x.OrderDate.Value.Date == OrderDate).Sum(x =>x.Product.Price * x.Quantity);
                var model3 = Tuple.Create<IEnumerable<JoinTables>,IEnumerable<OrderDetail>>(multiTable, await modelContext.Where(x => x.OrderDate.Value.Date == OrderDate).ToListAsync());
                return View(model3);
            }
            else
            {
                ViewBag.TotalQuantity = modelContext.Where(x =>x.OrderDate >= OrderDate && x.OrderDate <= DeliveryDate).Sum(x => x.Quantity);
                ViewBag.TotalPrice = modelContext.Where(x =>x.OrderDate >= OrderDate && x.OrderDate <= DeliveryDate).Sum(x => x.Product.Price * x.Quantity);
                var model3 = Tuple.Create<IEnumerable<JoinTables>,IEnumerable<OrderDetail>>(multiTable, await modelContext.Where(x => x.OrderDate >= OrderDate && x.OrderDate <= DeliveryDate).ToListAsync());
                return View(model3);
            }


        }

        [HttpGet]
        public IActionResult Search()
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            var modelContext =_context.OrderDetails.Include(p =>p.Customer).Include(p => p.Product).ToList();
            return View(modelContext);
        }
        [HttpPost]
        public async Task<IActionResult> Search(DateTime? OrderDate, DateTime? DeliveryDate)
        {
            var modelContext =
            _context.OrderDetails.Include(p =>
            p.Customer).Include(p => p.Product);
            if (OrderDate == null && DeliveryDate == null)
            {
                return View(modelContext);
            }
            else if (OrderDate == null && DeliveryDate != null)
            {
                var result = await modelContext.Where(x =>x.OrderDate.Value.Date == DeliveryDate).ToListAsync();
                return View(result);
            }
            else if (OrderDate != null && DeliveryDate == null)
            {
                var result = await modelContext.Where(x =>x.OrderDate.Value.Date == OrderDate).ToListAsync();
                return View(result);
            }
            else
            {
                var result = await modelContext.Where(x => x.OrderDate >= OrderDate && x.OrderDate <= DeliveryDate).ToListAsync();
                return View(result);
            }
        }



        public IActionResult ManageTestimonial()
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            var testimonials = _context.Testimonials.Include(t => t.Customer).ToList();
            return View(testimonials);
        }

        [HttpPost]
        public IActionResult UpdateIsApproved(decimal id, decimal? isApproved)
        {
            // Retrieve the testimonial from the database based on the ID.
            var testimonial = _context.Testimonials.Find(id);

            if (testimonial != null)
            {
                // Update the IsApproved property and save the changes.
                testimonial.Isapproved = isApproved;
                _context.SaveChanges();

                // Return a response indicating success, if needed.
                return Json(new { success = 1 });
            }

            // Return a response indicating an error, if needed.
            return Json(new { success = 0 });
        }
        public IActionResult PendingOrders()
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            var orderDetails = _context.OrderDetails
            .Include(od => od.Customer) // Include the Customer navigation property
            .Include(od => od.Product)  // Include the Product navigation property
            .Where(od => od.Status == "Pending").ToList();
            return View(orderDetails);
        }

        [HttpPost]
        public IActionResult UpdateStatus(decimal id, string? status)
        {
            var orderDetail = _context.OrderDetails.Find(id);

            if (orderDetail != null)
            {
                orderDetail.Status = status;
                orderDetail.DeliveryDate = DateTime.Now;
                _context.SaveChanges();
                CheckAndSendCompleteOrdersEmail(id);
                return Json(new { success = 1 });
            }
            return Json(new { success = 0 });
        }




        public IActionResult CheckAndSendCompleteOrdersEmail(decimal targetOrderId)
        {
            var targetOrder = _context.OrderDetails
                .Include(od => od.Customer)
                .Where(od => od.Id == targetOrderId)
                .FirstOrDefault();

            if (targetOrder != null)
            {
                decimal? customerId = targetOrder.CustomerId;
                var userLogin = _context.UserLogins.FirstOrDefault(ul => ul.CustomerId == customerId);
                string customerUserName = userLogin?.UserName;

                var ordersWithSameDate = _context.OrderDetails
                    .Where(od => od.OrderDate == targetOrder.OrderDate)
                    .ToList();

                bool allComplete = ordersWithSameDate.All(od => od.Status == "Completed");

                if (allComplete && !string.IsNullOrEmpty(customerUserName))
                {
                    // Construct a message that includes all orders with the same date and status
                    string subject = "Delivery Confirmation:";
                    string message = "All orders placed on have been delivered. Details:\n";

                    //foreach (var order in ordersWithSameDate)
                    //{
                    //    message += $"Order ID: {order.Id}, Product: {order.Product?.Name}, Quantity: {order.Quantity}\n";
                    //}
                    message += "Thank you.\n";

                    SendEmail(customerUserName, subject, message);
                }
            }

            return View(); // You can return a view or redirect as needed
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


        public IActionResult Contact()
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            var contactUs = _context.ContactUs.ToList();
            return View(contactUs);
        }

        // GET: Profile/Edit/5
        public async Task<IActionResult> ProfileEdit(decimal id)
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            UserLogin userLogin = await _context.UserLogins.Include(u => u.Customer).FirstOrDefaultAsync(m => m.CustomerId == id);

            if (userLogin == null)
            {
                return NotFound();
            }
            return View(userLogin);
        }
        // POST: Profile/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProfileEdit(decimal? id, [Bind("Id,UserName,Password,RoleId,CustomerId")] Customer customer, UserLogin userLogin, decimal? customerId, string FirstName, string LastName, string? Address, string? Phone, string? ImagePath, IFormFile? ImageFile, DateTime? RegistrationDate)
        {
            customer = await _context.Customers.Where(x => x.Id == customerId).FirstOrDefaultAsync();
            customer.FirstName = FirstName;
            customer.LastName = LastName;
            customer.Phone = Phone;
            customer.Address = Address;
            customer.RegistrationDate = RegistrationDate;
            customer.ImageFile = ImageFile;
            userLogin.CustomerId = customerId;
            context1.HttpContext.Session.SetString("name_customer", customer.FirstName);


            if (id != userLogin.Id)
            {
                return NotFound();
            }
            if (customer.ImageFile != null)
            {
                string wwwRootPath = _webHostEnviroment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + "_" + customer.ImageFile.FileName;
                string path = Path.Combine(wwwRootPath + "/HomeAssets/img/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await customer.ImageFile.CopyToAsync(fileStream);
                }
                customer.ImagePath = fileName;
                context1.HttpContext.Session.SetString("img_customer", fileName);
            }
            else
            {
                customer.ImagePath = ImagePath;
            }

            _context.Update(customer);
            _context.Entry(userLogin).State = EntityState.Modified;
            _context.Update(userLogin);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ProfileEdit), new { customerId });
        }

        private bool UserLoginExists(decimal id)
        {
            return (_context.UserLogins?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}