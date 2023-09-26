using Delux.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Delux.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProductContext _context;

        public HomeController(ILogger<HomeController> logger, ProductContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index(int pg = 1)
        {
            string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/currencyData.json");
            string jsonText = System.IO.File.ReadAllText(jsonFilePath);
            List<Currency> currencyRates = JsonConvert.DeserializeObject<List<Currency>>(jsonText)!;
            ViewBag.CurrencyRates = currencyRates;
            IEnumerable<Product> products = _context.Products.ToList(); // Load all products into memory
            const int pageSize = 6;
            if (pg < 1)
                pg = 1;
            int recsCount = products.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            pager.ControllerName = "Home"; // сюда запихал Home
            pager.ActionName = "Index"; // сюда запихал "Index" 
            int recSkip = (pg - 1) * pageSize;
            var data = products.Skip(recSkip).Take(pageSize).ToList();
            this.ViewBag.Pager = pager;
            return View(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}