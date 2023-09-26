using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Delux.Models;
using Newtonsoft.Json;
using Delux.Enums;

namespace Delux.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductContext _context;

        public ProductsController(ProductContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(int pg = 1, SortState sortState = SortState.NameAsc)
        {
            const int pageSize = 6;
            if (pg < 1)
                pg = 1;
            IQueryable<Product> productsQuery = _context.Products;
            switch (sortState)
            {
                case SortState.NameAsc: productsQuery = productsQuery.OrderBy(p => p.Name); break;
                case SortState.PriceAsc: productsQuery = productsQuery.OrderBy(p => p.Price); break;

                case SortState.DateCreateAsc: productsQuery = productsQuery.OrderBy(p => p.CreatedAt); break;
                case SortState.DateReplAsc: productsQuery = productsQuery.OrderBy(p => p.UpdatedAt); break;

                case SortState.CategoryAsc: productsQuery = productsQuery.OrderBy(p => p.CategoryId); break;
                case SortState.BrandAsc: productsQuery = productsQuery.OrderBy(p => p.BrandId); break;

                case SortState.NameDesc: productsQuery = productsQuery.OrderByDescending(p => p.Name); break;
                case SortState.PriceDesc: productsQuery = productsQuery.OrderByDescending(p => p.Price); break;

                case SortState.DateCreateDesc: productsQuery = productsQuery.OrderByDescending(p => p.CreatedAt); break;
                case SortState.DateReplDesc: productsQuery = productsQuery.OrderByDescending(p => p.UpdatedAt); break;

                case SortState.CategoryDesc: productsQuery = productsQuery.OrderByDescending(p => p.CategoryId); break;
                case SortState.BrandDesc: productsQuery = productsQuery.OrderByDescending(p => p.BrandId); break;
            }
            int recsCount = await productsQuery.CountAsync();
            var pager = new Pager(recsCount, pg, pageSize);
            pager.ControllerName = "Products"; // cюда запихал продукс
            pager.ActionName = "Index"; // сюда индекс
            int recSkip = (pg - 1) * pageSize;
            var data = await productsQuery
                .Skip(recSkip)
                .Take(pageSize)
                .ToListAsync();
            this.ViewBag.Pager = pager;
            #region
            ViewBag.NameSort = sortState == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewBag.PriceSort = sortState == SortState.PriceAsc ? SortState.PriceDesc : SortState.PriceAsc;
            ViewBag.DataCteateSort = sortState == SortState.DateCreateAsc ? SortState.DateCreateDesc : SortState.DateCreateAsc;
            ViewBag.DateReplSort = sortState == SortState.DateReplAsc ? SortState.DateReplDesc : SortState.DateReplAsc;
            ViewBag.CategorySort = sortState == SortState.CategoryAsc ? SortState.CategoryDesc : SortState.CategoryAsc;
            ViewBag.BrandSort = sortState == SortState.BrandAsc ? SortState.BrandDesc : SortState.BrandAsc;
            var productContext = _context.Products.Include(p => p.Brands).Include(p => p.Categories);
            ViewBag.Tags = productContext;
            ViewBag.Brands = _context.Brands.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            #endregion
            return View(data);
        }
        [HttpGet]
        public IActionResult Search(string term)
        {
            var results = _context.Products
                .Where(p => p.Name.Contains(term))
                .Select(p => $"{p.ProductId},{p.Name}") // Формируем строку с ProductId и Name, разделенными запятой
                .ToList();

            return Content(string.Join(";", results)); // Используем точку с запятой для разделения результатов
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/currencyData.json");
            string jsonText = System.IO.File.ReadAllText(jsonFilePath);
            List<Currency> currencyRates = JsonConvert.DeserializeObject<List<Currency>>(jsonText)!;
            ViewBag.CurrencyRates = currencyRates;
            if (id == null || _context.Products == null)
                return NotFound();
            var phone = await _context.Products
            .Include(p => p.Brands)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(m => m.ProductId == id);

            if (phone == null)
                return NotFound();
            return View(phone);
        }

        // GET: Products/Create
        public IActionResult Create(int ReviewId)
        {
            var reviewId = _context.Reviews
                .Where(p => p.ReviewId == ReviewId)
                .Select(p => p.ReviewId)
                .FirstOrDefault();
            ViewData["ReviewId"] = reviewId;

            List<Category> categories = _context.Categories.ToList();
            List<Brand> brands = _context.Brands.ToList();

            ViewData["Categories"] = categories;
            ViewData["Brands"] = brands;

            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Name,Price,Description,ImageUrl,CreatedAt,UpdatedAt,CategoryId,BrandId")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.CreatedAt = DateTime.Now;
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            List<Category> categories = _context.Categories.ToList();
            List<Brand> brands = _context.Brands.ToList();

            ViewData["Categories"] = categories;
            ViewData["Brands"] = brands;
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,Price,Description,ImageUrl,CreatedAt,UpdatedAt,CategoryId,BrandId")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.UpdatedAt = DateTime.Now;
                    _context.Update(product);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brands)
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'PhoneContext.Phons' is null.");
            }

            var phone = await _context.Products.FindAsync(id);

            if (phone != null)
            {
                // Получаем все связанные отзывы
                var relatedReviews = _context.Reviews.Where(r => r.ProductId == id);

                // Удаление всех связанных отзывов
                _context.Reviews.RemoveRange(relatedReviews);

                // Затем удаляем сам телефон
                _context.Products.Remove(phone);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
