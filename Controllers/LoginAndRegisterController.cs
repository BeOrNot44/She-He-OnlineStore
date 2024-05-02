using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg.OpenPgp;
using She___He_Store.Models;
using System.Security.Claims;
using System.Threading.Tasks;


namespace She___He_Store.Controllers
{
    public class LoginAndRegisterController : Controller
    {
        private readonly ModelContext _context;
        private readonly IHttpContextAccessor context1;
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnviroment; //declare Variable

        //Update the constructer
        public LoginAndRegisterController(ILogger<HomeController> logger, ModelContext context, IWebHostEnvironment webHostEnviroment, IHttpContextAccessor httpContextAccessor) //declare IWebHostEnvironment parameter
        {
            _logger = logger;
            context1 = httpContextAccessor;
            _context = context;
            _webHostEnviroment = webHostEnviroment; //dependency injection
        }

        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost] //added for Logout
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // Sign the user out
            HttpContext.SignOutAsync();

            // Redirect to the home page or another page after log out
            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        public async Task<IActionResult> Login([Bind("UserName, Password")] UserLogin userLogin)
        {
            var auth = _context.UserLogins.Where(x => x.UserName == userLogin.UserName && x.Password == userLogin.Password).SingleOrDefault();

            if (auth != null)
            {
                //To Authenticate the user 
                var identity = new ClaimsIdentity(new[] //added for logout
                {
                 new Claim(ClaimTypes.Name, auth.UserName), // Set the user's name or identifier
                  // You can add additional claims here if needed
                 }, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Sign in the user with the created identity
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                //to here

                //1.Admin
                //2.User 
                var customer = _context.Customers.Where(m => m.Id == auth.CustomerId).FirstOrDefault();
                switch (auth.RoleId)
                {
                    case 1:
                        context1.HttpContext.Session.SetInt32("id_customer", (int)customer.Id);
                        context1.HttpContext.Session.SetString("name_customer", customer.FirstName);
                        context1.HttpContext.Session.SetString("img_customer", customer.ImagePath);

                        context1.HttpContext.Session.SetString("Admin","1");
                        ViewBag.AdminRole= context1.HttpContext.Session.GetString("Admin");


                        HttpContext.Session.SetString("AdminName", auth.UserName);

                        return RedirectToAction("Dashboard", "Admin");
                    case 2:
                        context1.HttpContext.Session.SetInt32("id_customer", (int)customer.Id);
                        context1.HttpContext.Session.SetString("name_customer", customer.FirstName);
                        context1.HttpContext.Session.SetString("img_customer", customer.ImagePath);

                        context1.HttpContext.Session.SetString("Admin", "0");
                        ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

                        //HttpContext.Session.SetString("hi","meme");
                        //ViewBag.hi = context1.HttpContext.Session.GetString("hi");
                        //context1.HttpContext.Session.SetInt32("UserId", (int)auth.CustomerId);
                        //context1.HttpContext.Session.Clear();
                        ////context1.HttpContext.Session.SetInt32("profile", 0);
                        //context1.HttpContext.Session.SetInt32("total", 0);

                        context1.HttpContext.Session.SetInt32("id", (int)auth.Id);
                        return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                // Incorrect password: Provide an error message
                ModelState.AddModelError("*", "Your username or password is incorrect.");
            }

            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Id,FirstName,LastName,Address,Phone,ImagePath")] Customer customer, string UserName, string Password)
        {
            if (ModelState.IsValid)
            {
                // Check if the provided UserName already exists in your database
                bool isUserNameTaken = _context.UserLogins.Any(u => u.UserName == UserName);

                if (isUserNameTaken)
                {
                    ModelState.AddModelError("UserName", "The provided Email is already in use. Please choose a different email.");
                    return View(customer);
                }

                //if (customer.ImageFile != null)
                //{
                //    string wwwRootPath = _webHostEnviroment.WebRootPath;
                //    string fileName = Guid.NewGuid().ToString() + "_" + customer.ImageFile.FileName;
                //    string path = Path.Combine(wwwRootPath + "/HomeAssets/img/", fileName);
                //    using (var fileStream = new FileStream(path, FileMode.Create))
                //    {
                //        await customer.ImageFile.CopyToAsync(fileStream);
                //    }
                //    customer.ImagePath = fileName;
                //}
                customer.RegistrationDate = DateTime.Now;
                _context.Add(customer);
                await _context.SaveChangesAsync();

                //Add Login Details
                UserLogin login = new UserLogin();
                login.RoleId = 2;
                login.Password = Password;
                login.UserName = UserName;
                login.CustomerId = customer.Id;

                _context.Add(login);
                await _context.SaveChangesAsync();

                // Redirect to the "Login" action
                return RedirectToAction("Login");


            }
            return View(customer);
        }






        // GET: Profile/Edit/5
        public async Task<IActionResult> ProfileEdit(decimal id)
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            UserLogin userLogin = await _context.UserLogins
    .Include(u => u.Customer)
    .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (userLogin == null)
            {
                return NotFound();
            }
            return View(userLogin);
        }

        // POST: Profile/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProfileEdit(decimal? id, [Bind("Id,UserName,Password,RoleId,CustomerId")] Customer customer,UserLogin userLogin ,decimal? customerId, string FirstName, string LastName, string? Address, string? Phone, string? ImagePath,IFormFile? ImageFile, DateTime? RegistrationDate)
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
