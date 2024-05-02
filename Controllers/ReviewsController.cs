using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using She___He_Store.Models;

namespace She___He_Store.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ModelContext _context;
        private readonly IHttpContextAccessor context1;
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnviroment; //declare Variable

        //Update the constructer
        public ReviewsController(ILogger<HomeController> logger, ModelContext context, IWebHostEnvironment webHostEnviroment, IHttpContextAccessor httpContextAccessor) //declare IWebHostEnvironment parameter
        {
            _logger = logger;
            context1 = httpContextAccessor;
            _context = context;
            _webHostEnviroment = webHostEnviroment; //dependency injection
        }

        // GET: Reviews
        public async Task<IActionResult> Index()
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            var modelContext = _context.Reviews.Include(r => r.Customer).Include(r => r.Product);
            return View(await modelContext.ToListAsync());
        }

        public async Task<IActionResult> ReviewProducts(int id)
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            var review = _context.Reviews.Include(r => r.Customer).Where(x => x.ProductId == id).ToList();
            var product = _context.Products.Where(x => x.Id == id).ToList();

            context1.HttpContext.Session.SetInt32("product_idd", id);

            var productReviewModel = Tuple.Create<IEnumerable<Review>, IEnumerable<Product>>(review, product);
            

            decimal? averageRating = review.Any() ? review.Average(x => x.Rating) : 0;
            ViewBag.AverageRating = averageRating;

            return View(productReviewModel);
        }



        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            if (id == null || _context.Reviews == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            // Calculate the average rating
            decimal? averageRating = await _context.Reviews
                .Where(r => r.ProductId == review.ProductId)
                .AverageAsync(r => r.Rating);

            ViewBag.AverageRating = averageRating;

            return View(review);
        }

        // GET: Reviews/Create
        public IActionResult Create()
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            ViewBag.ProductId = context1.HttpContext.Session.GetInt32("product_idd");

            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id");

            //ViewBag.ProductId = id;
            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,CustomerId,ReviewText,Rating,ReviewDate")] Review review)
        {
            if (ModelState.IsValid)
            {
                _context.Add(review);
                await _context.SaveChangesAsync();
                return Redirect($"ReviewProducts/{review.ProductId}");
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", review.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", review.ProductId);
            return View(review);
        }

        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Reviews == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", review.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", review.ProductId);
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,ProductId,CustomerId,Rating,ReviewText,ReviewDate")] Review review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.Id))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", review.CustomerId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", review.ProductId);
            return View(review);
        }

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Reviews == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Reviews == null)
            {
                return Problem("Entity set 'ModelContext.Reviews'  is null.");
            }
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(decimal id)
        {
          return (_context.Reviews?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
