using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using She___He_Store.Models;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;


namespace She___He_Store.Controllers
{
    public class HomeController : Controller
    {
        private readonly ModelContext _context;
        private readonly IHttpContextAccessor context1;
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnviroment; //declare Variable

        //Update the constructer
        public HomeController(ILogger<HomeController> logger, ModelContext context, IWebHostEnvironment webHostEnviroment, IHttpContextAccessor httpContextAccessor) //declare IWebHostEnvironment parameter
        {
            _logger = logger;
            context1 = httpContextAccessor;
            _context = context;
            _webHostEnviroment = webHostEnviroment; //dependency injection
        }
    

    public IActionResult Index()
        {

            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");


            ViewBag.hero = context1.HttpContext.Session.GetString("hero");
            if (ViewBag.hero == null || ViewBag.hero == "")
            {
                context1.HttpContext.Session.SetString("hero", "hero.png");
            }
            ViewBag.hero = context1.HttpContext.Session.GetString("hero");
            //
            ViewBag.heroText = context1.HttpContext.Session.GetString("heroText");
            if (ViewBag.heroText == null || ViewBag.heroText == "")
            {
                context1.HttpContext.Session.SetString("heroText", "Book she&He For Your Dream Food");
            }
            ViewBag.heroText = context1.HttpContext.Session.GetString("heroText");



            //
            ViewBag.About = context1.HttpContext.Session.GetString("About");
            if (ViewBag.About == null || ViewBag.About == "")
            {
                context1.HttpContext.Session.SetString("About", "about.jpg");
            }
            ViewBag.About = context1.HttpContext.Session.GetString("About");
            //
            ViewBag.AboutText = context1.HttpContext.Session.GetString("AboutText");
            if (ViewBag.AboutText == null || ViewBag.AboutText == "")
            {
                context1.HttpContext.Session.SetString("AboutText", "Trusted By many satisfied clients");
            }
            ViewBag.AboutText = context1.HttpContext.Session.GetString("AboutText");
            //
            ViewBag.AboutText2 = context1.HttpContext.Session.GetString("AboutText2");
            if (ViewBag.AboutText2 == null || ViewBag.AboutText2 == "")
            {
                context1.HttpContext.Session.SetString("AboutText2", "She&He restaurant is a culinary haven where flavor and creativity converge to delight your senses. \r\n                    Our menu offers a diverse range of exquisite dishes crafted with the finest ingredients and a passion \r\n  for gastronomy. Join us for a remarkable dining experience that combines taste, innovation, and warmth.\r\n");
            }
            ViewBag.AboutText2 = context1.HttpContext.Session.GetString("AboutText2");


            //
            ViewBag.Video = context1.HttpContext.Session.GetString("Video");
            if (ViewBag.Video == null || ViewBag.Video == "")
            {
                context1.HttpContext.Session.SetString("Video","<iframe class=\"embed-responsive-item\" id=\"video\" src=\"https://www.youtube.com/embed/vT8Vt4yRE04?si=2QxjvtisC80vC8ho\" title=\"YouTube video player\" allow=\"autoplay\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share\" allowfullscreen allowscriptaccess=\"always\"></iframe>");
            }
            ViewBag.Video = context1.HttpContext.Session.GetString("Video");



            ViewBag.NumberofCustomer = _context.Customers.Count();
            ViewBag.NumberofTestimonials = _context.Testimonials.Count();
            ViewBag.Numberoforders = _context.OrderDetails.Count();

            var testimonials = _context.Testimonials
                .Where(t => t.Isapproved == 1)
                .Include(t => t.Customer)
                .ToList();

            ViewBag.TestimonialInfo = testimonials;

            return View();
        }

        public IActionResult EditHome()
        {

            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            ViewBag.hero = context1.HttpContext.Session.GetString("hero");
            ViewBag.heroText = context1.HttpContext.Session.GetString("heroText");
            //
            ViewBag.About = context1.HttpContext.Session.GetString("About");
            ViewBag.AboutText = context1.HttpContext.Session.GetString("AboutText");
            ViewBag.AboutText2 = context1.HttpContext.Session.GetString("AboutText2");
            //
            ViewBag.Video = context1.HttpContext.Session.GetString("Video");


            ViewBag.NumberofCustomer = _context.Customers.Count();
            ViewBag.NumberofTestimonials = _context.Testimonials.Count();
            ViewBag.Numberoforders = _context.OrderDetails.Count();

            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditHome(string? heroText, IFormFile? ImageFile1,IFormFile? ImageFile  ,string? AboutText, string? AboutText2, string? Video)
        {
            if (ImageFile1 != null)
            {
                string wwwRootPath1 = _webHostEnviroment.WebRootPath;
                string fileName1 = Guid.NewGuid().ToString() + "_" + ImageFile1.FileName;
                string path1 = Path.Combine(wwwRootPath1 + "/HomeAssets/img/", fileName1);
                using (var fileStream = new FileStream(path1, FileMode.Create))
                {
                    // Save the file content to the fileStream
                    ImageFile1.CopyTo(fileStream);
                }

                context1.HttpContext.Session.SetString("hero", fileName1);
            }
            context1.HttpContext.Session.SetString("heroText", heroText);




            if (ImageFile != null)
            {
                string wwwRootPath = _webHostEnviroment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                string path = Path.Combine(wwwRootPath + "/HomeAssets/img/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    // Save the file content to the fileStream
                    ImageFile.CopyTo(fileStream);
                }

                context1.HttpContext.Session.SetString("About", fileName);
            }
            context1.HttpContext.Session.SetString("AboutText", AboutText);
            context1.HttpContext.Session.SetString("AboutText2", AboutText2);


            context1.HttpContext.Session.SetString("Video", Video);

            return RedirectToAction(nameof(Index));

        }


        // GET: ContactUs/Create
        public IActionResult CreateContact()
        {
            return View();
        }

        // POST: ContactUs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateContact([Bind("Id,Country,City,VegetarianFriendly,Email,Phone,DateAdded,DescriptionText")] ContactU contactU)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contactU);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contactU);
        }


        public IActionResult Menu()
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            var categories = _context.Categories.ToList(); //
            return View(categories); //
        }

        public IActionResult GetProductByCategory(int id) //
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            var products = _context.Products.Where(p => p.CategoryId == id).ToList(); //
            return View(products); //
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(decimal? productId, decimal? customerId, decimal? quantity, decimal? TotalAmount, decimal? categoryId)
        {
            if (ModelState.IsValid)
            {
                OrderDetail cart = new OrderDetail();
                cart.OrderDate = DateTime.Today;
                cart.ProductId = productId;
                if (quantity == null)
                {
                    cart.Quantity = 1;
                }
                else
                {
                    cart.Quantity = quantity;
                }
                var price = _context.Products.FirstOrDefault(p => p.Id == productId)?.Price;
                var salePrice = _context.Products.FirstOrDefault(p => p.Id == productId)?.SalePrice;


                if (price != null && price > 0)
                {
                    cart.TotalAmount = price * cart.Quantity;
                    if (salePrice != null && salePrice > 0)
                    {
                        cart.TotalAmount = cart.TotalAmount * (1 - salePrice / 100);
                    }
                }
                cart.CustomerId = customerId;
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return Redirect($"/Home/GetProductByCategory/{categoryId}");

            }
            return View();

        }




        [HttpGet]
        public IActionResult Search()
        {
            var modelContext = _context.OrderDetails.Include(p => p.Customer).Include(p => p.Product).ToList();
            //var modelContext = _context.ProductCustomers.Include(p =>p.Customer).Include(p => p.Product).ToList();
            return View(modelContext);

        }
        [HttpPost]

        public async Task<IActionResult> Search(DateTime? startDate, DateTime? endDate)

        {

            var modelContext = _context.OrderDetails.Include(p => p.Customer).Include(p => p.Product);

            //startDate : DateFrom 

            //endDate : DateTo

            if (startDate == null && endDate == null)
            {
                return View(modelContext);
            }
            else if (startDate == null && endDate != null)
            {
                var result = await modelContext.Where(x =>
                x.OrderDate.Value.Date == endDate).ToListAsync();
                return View(result);
            }
            else if (startDate != null && endDate == null)
            {
                var result = await modelContext.Where(x =>
                x.OrderDate.Value.Date == startDate).ToListAsync();
                return View(result);
            }
            else
            {
                var result = await modelContext.Where(x =>
                x.OrderDate.Value.Date >= startDate && x.OrderDate <= endDate).ToListAsync();
                return View(result);
            }
        }


        public IActionResult AboutUs()
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            return View();
        }
        public IActionResult ContactUs()
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



    }
}