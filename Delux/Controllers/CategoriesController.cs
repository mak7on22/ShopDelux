using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Delux.Models;
using Delux.Enums;

namespace Delux.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ProductContext _context;

        public CategoriesController(ProductContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index(SortState sortState = SortState.NameAsc)
        {
            IEnumerable<Category> categories = _context.Categories.ToList(); // Load all products into memory
            ViewBag.NameSort = sortState == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;

            switch (sortState)
            {
                case SortState.NameAsc: categories = categories.OrderBy(p => p.CategoryName); break;
                case SortState.NameDesc: categories = categories.OrderByDescending(p => p.CategoryName); break;
            }

            return View(categories);
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == category.CategoryName);
                if (existingCategory != null)
                {
                    ModelState.AddModelError("CategoryName", "Категория с таким именем уже существует.");
                    return View(category);
                }

                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryName")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
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
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ProductContext.Categories' is null.");
            }

            var category = await _context.Categories.FindAsync(id);

            if (category != null)
            {
                // Попробуйте найти категорию "Нет категории"
                var unknownCategory = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == "Нет категории");

                if (unknownCategory == null)
                {
                    // Если категория "Нет категории" не существует, создайте ее
                    unknownCategory = new Category { CategoryName = "Нет категории" };
                    _context.Categories.Add(unknownCategory);
                    await _context.SaveChangesAsync();
                }

                // Обновите все продукты с этой категорией на "Нет категории"
                var productsWithDeletedCategory = _context.Products.Where(p => p.CategoryId == category.CategoryId);
                foreach (var product in productsWithDeletedCategory)
                {
                    product.CategoryId = unknownCategory.CategoryId;
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return (_context.Categories?.Any(e => e.CategoryId == id)).GetValueOrDefault();
        }
    }
}
