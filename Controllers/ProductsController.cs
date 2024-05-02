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
    public class ProductsController : Controller
    {
        private readonly ModelContext _context;
        private readonly IHttpContextAccessor context1;

        private readonly IWebHostEnvironment _webHostEnviroment; //declare Variable
        //Update the constructer
        public ProductsController(ModelContext context, IWebHostEnvironment webHostEnviroment, IHttpContextAccessor httpContextAccessor) //declare IWebHostEnvironment parameter
        {
            context1 = httpContextAccessor;
            _context = context;
            _webHostEnviroment = webHostEnviroment; //dependency injection
        }
        public IActionResult GetCategory()
        {
            var category = _context.Categories.Include(t => t.Products).ToList();

            return View(category);

        }


        // GET: Products
        public async Task<IActionResult> Index()
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            //ViewBag.CategoryName = context1.HttpContext.Session.SetString("CategoryName");

            var modelContext = _context.Products.Include(p => p.Category);
            return View(await modelContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            var categoryNames = _context.Categories.Select(c => c.CategoryName).ToList();
            ViewBag.CategoryNames = new SelectList(categoryNames);

            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,StockQuantity,CategoryId,ImageFile,SalePrice")] Product product,string catName)
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");
            Category category =await _context.Categories.Where(x => x.CategoryName == catName).FirstOrDefaultAsync();
            product.CategoryId = category.Id;
            if (ModelState.IsValid)
            {
                if (product.ImageFile != null)
                {
                    string wwwRootPath = _webHostEnviroment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/HomeAssets/img/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(fileStream);
                    }
                    product.ImagePath = fileName;
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //var categories = await _context.Categories.ToListAsync();
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", product.CategoryId);
            //ViewData["CategoryId"] = new SelectList(categories, "Id", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            var categoryNames = _context.Categories.Select(c => c.CategoryName).ToList();
            ViewBag.CategoryNames = new SelectList(categoryNames);


            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }


            ViewBag.ImgProduct = product.ImagePath;
            var categories = await _context.Categories.ToListAsync();
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", product.CategoryId);

            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,Name,Description,Price,StockQuantity,CategoryId,ImageFile,SalePrice")] Product product,string imgPath)
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (product.ImageFile != null)
                {
                    string wwwRootPath = _webHostEnviroment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/HomeAssets/img/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(fileStream);
                    }
                    product.ImagePath = fileName;
                }
                else
                {
                    product.ImagePath = imgPath;
                }

                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            ViewBag.id_customer = context1.HttpContext.Session.GetInt32("id_customer");
            ViewBag.CustomerName = context1.HttpContext.Session.GetString("name_customer");
            ViewBag.CustomerImg = context1.HttpContext.Session.GetString("img_customer");
            ViewBag.AdminRole = context1.HttpContext.Session.GetString("Admin");

            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'ModelContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                // Set the associated OrderDetails' ProductId to null
                foreach (var orderDetail in product.OrderDetails)
                {
                    _context.OrderDetails.Remove(orderDetail);
                }
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(decimal id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
