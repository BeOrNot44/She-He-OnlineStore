using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using She___He_Store.Models;

namespace She___He_Store.Controllers
{
    public class UserLoginsController : Controller
    {
        private readonly ModelContext _context;
        private readonly IHttpContextAccessor context1;
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnviroment; //declare Variable

        //Update the constructer
        public UserLoginsController(ILogger<HomeController> logger, ModelContext context, IWebHostEnvironment webHostEnviroment, IHttpContextAccessor httpContextAccessor) //declare IWebHostEnvironment parameter
        {
            _logger = logger;
            context1 = httpContextAccessor;
            _context = context;
            _webHostEnviroment = webHostEnviroment; //dependency injection
        }



        // GET: UserLogins
        public async Task<IActionResult> Index()
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            var modelContext = _context.UserLogins.Include(u => u.Customer).Include(u => u.Role);
            return View(await modelContext.ToListAsync());
        }
        public async Task<IActionResult> Profile(decimal? id)
        {

            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            var userLogins = await _context.UserLogins
                .Include(u => u.Customer)
                .ToListAsync();

            return View(userLogins);
        }


        // GET: UserLogins/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.UserLogins == null)
            {
                return NotFound();
            }

            var userLogin = await _context.UserLogins
                .Include(u => u.Customer)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userLogin == null)
            {
                return NotFound();
            }

            return View(userLogin);
        }

        // GET: UserLogins/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id");
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id");
            return View();
        }

        // POST: UserLogins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,Password,RoleId,CustomerId")] UserLogin userLogin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userLogin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", userLogin.CustomerId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", userLogin.RoleId);
            return View(userLogin);
        }



        //// Helper method to check if UserLogin exists
        //private bool UserLoginExists(decimal id)
        //{
        //    return _context.UserLogins.Any(e => e.Id == id);
        //}















        //// GET: UserLogins/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            if (id == null || _context.UserLogins == null)
            {
                return NotFound();
            }

            var userLogin = await _context.UserLogins
                .Include(u => u.Customer)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (userLogin == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", userLogin.CustomerId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", userLogin.RoleId);
            return View(userLogin);
        }

        // POST: UserLogins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,UserName,Password,RoleId,CustomerId")] UserLogin userLogin)
        {
            if (id != userLogin.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userLogin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserLoginExists(userLogin.Id))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", userLogin.CustomerId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", userLogin.RoleId);
            return View(userLogin);
        }

        // GET: Userlogins/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.UserLogins == null)
            {
                return NotFound();
            }

            var userLogin = await _context.UserLogins
                .Include(u => u.Customer)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userLogin == null)
            {
                return NotFound();
            }

            return View(userLogin);
        }

        // POST: Userlogins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.UserLogins == null)
            {
                return Problem("Entity set 'ModelContext.UserLogins'  is null.");
            }
            var userLogin = await _context.UserLogins.FindAsync(id);
            if (userLogin != null)
            {
                _context.UserLogins.Remove(userLogin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserLoginExists(decimal id)
        {
            return (_context.UserLogins?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
