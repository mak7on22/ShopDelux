using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Delux.Models;
using Microsoft.CodeAnalysis;

namespace Delux.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ProductContext _context;

        public OrdersController(ProductContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var productContext = _context.Orders.Include(o => o.Products);
            return View(await productContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if(id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Products)
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            ViewData["ProductName"] = order.Products.Name;

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create(int ProductId)
        {
            List<Product> products = _context.Products.ToList();
            var productName = _context.Products.Where(p => p.ProductId == ProductId).Select(p => p.Name).FirstOrDefault();
            var productUrl = _context.Products.Where(p => p.ProductId == ProductId).Select(p => p.ImageUrl).FirstOrDefault();
            ViewData["Products"] = products;
            ViewData["ProductName"] = productName;
            ViewData["ProductUrl"] = productUrl;
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,OrderDate,NameUser,Address,ContactPhone,Email,ProductId")] Order order)
        {
            if (ModelState.IsValid)
            {
                // Проверяем существование продукта с указанным ProductId
                var product = await _context.Products.FindAsync(order.ProductId);

                if (product != null)
                {
                    // Заполняем объект Order данными о продукте, включая его имя
                    order.Products = product;

                    // Добавляем заказ в контекст
                    _context.Add(order);

                    // Сохраняем изменения в базе данных
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Если продукт с указанным ProductId не найден, обработать эту ситуацию, например, выдать ошибку
                    ModelState.AddModelError("ProductId", "Выбранный продукт не существует.");
                }
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Description", order.ProductId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,OrderDate,NameUser,Address,ContactPhone,Email,ProductId")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Products) // Включите связанный товар
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            ViewData["ProductName"] = order.Products.Name; // Используйте имя товара

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'ProductContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }
    }
}
