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
    public class BrandsController : Controller
    {
        private readonly ProductContext _context;

        public BrandsController(ProductContext context)
        {
            _context = context;
        }

        // GET: Brands
        public async Task<IActionResult> Index(SortState sortState = SortState.NameAsc)
        {
            IEnumerable<Brand> brabds = _context.Brands.ToList(); // Load all products into memory
            ViewBag.NameSort = sortState == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewBag.DateReplSort = sortState == SortState.DateReplAsc ? SortState.DateReplDesc : SortState.DateReplAsc;

            switch (sortState)
            {
                case SortState.NameAsc: brabds = brabds.OrderBy(p => p.BrandName); break;
                case SortState.DateReplAsc: brabds = brabds.OrderBy(p => p.FoundationDate); break;
             
                case SortState.NameDesc: brabds = brabds.OrderByDescending(p => p.BrandName); break;
                case SortState.DateReplDesc: brabds = brabds.OrderByDescending(p => p.FoundationDate); break;
                
            }
            return View(brabds);
        }

        // GET: Brands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Brands == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .FirstOrDefaultAsync(m => m.BrandId == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // GET: Brands/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Brands/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BrandId,BrandName,Email,FoundationDate")] Brand brand)
        {
            if (ModelState.IsValid)
            {
                var existingBrand = await _context.Brands
                    .FirstOrDefaultAsync(b => b.BrandName == brand.BrandName);

                if (existingBrand == null)
                {
                    _context.Add(brand);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Бренд с таким именем уже существует.");
                }
            }

            return View(brand);
        }

        // GET: Brands/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Brands == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        // POST: Brands/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BrandId,BrandName,Email,FoundationDate")] Brand brand)
        {
            if (id != brand.BrandId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(brand);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrandExists(brand.BrandId))
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
            return View(brand);
        }

        // GET: Brands/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Brands == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .FirstOrDefaultAsync(m => m.BrandId == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // POST: Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Brands == null)
            {
                return Problem("Entity set 'ProductContext.Brands' is null.");
            }

            var brand = await _context.Brands.FindAsync(id);

            if (brand != null)
            {
                // Попробуйте найти бренд "Неизвестный бренд"
                var unknownBrand = await _context.Brands.FirstOrDefaultAsync(b => b.BrandName == "Неизвестный бренд");

                if (unknownBrand == null)
                {
                    // Если бренд "Неизвестный бренд" не существует, создайте его
                    unknownBrand = new Brand { BrandName = "Без бренда" };
                    // Установите дату обновления на текущую дату и произвольный email
                    unknownBrand.FoundationDate = DateTime.Now.Date;
                    unknownBrand.Email = "example@email.com";
                    _context.Brands.Add(unknownBrand);
                    await _context.SaveChangesAsync();
                }

                // Обновите все продукты с этим брендом на "Неизвестный бренд"
                var productsWithDeletedBrand = _context.Products.Where(p => p.BrandId == brand.BrandId);
                foreach (var product in productsWithDeletedBrand)
                {
                    product.BrandId = unknownBrand.BrandId;
                }

               

                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }



        private bool BrandExists(int id)
        {
          return (_context.Brands?.Any(e => e.BrandId == id)).GetValueOrDefault();
        }
    }
}
