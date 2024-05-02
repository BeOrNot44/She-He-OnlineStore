using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using She___He_Store.Models;

namespace She___He_Store.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ModelContext _context;
        private readonly IHttpContextAccessor context1;

        private readonly IWebHostEnvironment _webHostEnviroment; //declare Variable
        //Update the constructer
        public CustomersController(ModelContext context, IWebHostEnvironment webHostEnviroment, IHttpContextAccessor httpContextAccessor) //declare IWebHostEnvironment parameter
        {
            context1 = httpContextAccessor;
            _context = context;
            _webHostEnviroment = webHostEnviroment; //dependency injection
        }



        // GET: Customers
        public async Task<IActionResult> Index()
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            return _context.Customers != null ?
                        View(await _context.Customers.ToListAsync()) :
                        Problem("Entity set 'ModelContext.Customers'  is null.");
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Address,Phone,ImagePath,RegistrationDate")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,FirstName,LastName,Address,Phone,ImageFile,RegistrationDate")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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
                }

                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index","Customers");
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'ModelContext.Customers'  is null.");
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                // Remove associated Testimonials
                foreach (var test in customer.Testimonials)
                {
                    _context.Testimonials.Remove(test);
                }
                // Remove associated Testimonials
                foreach (var test in customer.Testimonials)
                {
                    _context.Testimonials.Remove(test);
                }

                // Remove associated Reviews
                foreach (var rev in customer.Reviews)
                {
                    _context.Reviews.Remove(rev);
                }

                // Remove associated OrderDetails
                foreach (var od in customer.OrderDetails)
                {
                    od.CustomerId=null;
                }

                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(decimal id)
        {
            return (_context.Customers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
