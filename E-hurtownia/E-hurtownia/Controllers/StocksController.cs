using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using E_hurtownia.Models;

namespace E_hurtownia.Controllers
{
    public class StocksController : Controller
    {
        private readonly EhurtowniaContext _context;

        public StocksController(EhurtowniaContext context)
        {
            _context = context;
        }

        // GET: Stocks
        public async Task<IActionResult> Index()
        {
            ViewBag.COOKIE_LOGGED_USERNAME = Request.Cookies["COOKIE_LOGGED_USERNAME"];
            ViewBag.CurrentUserID = _context.Users.Where(user => user.Login == Request.Cookies["COOKIE_LOGGED_USERNAME"]).Single().IdUser;
            ViewBag.Storekeepers = _context.Storekeepers.ToList();
            var ehurtowniaContext = _context.Stocks.Include(s => s.FkProductNavigation).Include(s => s.FkStorehouseNavigation);
            return View(await ehurtowniaContext.ToListAsync());
        }

        // GET: Stocks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.COOKIE_LOGGED_USERNAME = Request.Cookies["COOKIE_LOGGED_USERNAME"];

            if (id == null)
            {
                return NotFound();
            }

            var stocks = await _context.Stocks
                .Include(s => s.FkProductNavigation)
                .Include(s => s.FkStorehouseNavigation)
                .FirstOrDefaultAsync(m => m.IdStock == id);
            if (stocks == null)
            {
                return NotFound();
            }

            return View(stocks);
        }

        // GET: Stocks/Create
        public IActionResult Create()
        {
            ViewBag.COOKIE_LOGGED_USERNAME = Request.Cookies["COOKIE_LOGGED_USERNAME"];
            int currentUserID = _context.Users.Where(user => user.Login == Request.Cookies["COOKIE_LOGGED_USERNAME"]).Single().IdUser;

            List<Storehouses> availableStorehouses = new List<Storehouses>();
            List<Storekeepers> availableStorekeepers = _context.Storekeepers.ToList();

            foreach (Storekeepers storekeeper in availableStorekeepers) {
                if (storekeeper.FkUser == currentUserID) {
                    availableStorehouses.Add(_context.Storehouses.Where(storehouse => storehouse.IdStorehouse == storekeeper.FkStorehouse).Single());
                }
            }

            ViewData["FkProduct"] = new SelectList(_context.Products, "IdProduct", "Name");
            ViewData["FkStorehouse"] = new SelectList(availableStorehouses, "IdStorehouse", "IdStorehouse");
            return View();
        }

        // POST: Stocks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdStock,FkProduct,Amount,FkStorehouse,Status")] Stocks stocks)
        {
            ViewBag.COOKIE_LOGGED_USERNAME = Request.Cookies["COOKIE_LOGGED_USERNAME"];

            if (ModelState.IsValid)
            {
                _context.Add(stocks);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FkProduct"] = new SelectList(_context.Products, "IdProduct", "Name", stocks.FkProduct);
            ViewData["FkStorehouse"] = new SelectList(_context.Storehouses, "IdStorehouse", "IdStorehouse", stocks.FkStorehouse);
            return View(stocks);
        }

        // GET: Stocks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.COOKIE_LOGGED_USERNAME = Request.Cookies["COOKIE_LOGGED_USERNAME"];
            int currentUserID = _context.Users.Where(user => user.Login == Request.Cookies["COOKIE_LOGGED_USERNAME"]).Single().IdUser;

            List<Storehouses> availableStorehouses = new List<Storehouses>();
            List<Storekeepers> availableStorekeepers = _context.Storekeepers.ToList();

            foreach (Storekeepers storekeeper in availableStorekeepers) {
                if (storekeeper.FkUser == currentUserID) {
                    availableStorehouses.Add(_context.Storehouses.Where(storehouse => storehouse.IdStorehouse == storekeeper.FkStorehouse).Single());
                }
            }

            if (id == null)
            {
                return NotFound();
            }

            var stocks = await _context.Stocks.FindAsync(id);
            if (stocks == null)
            {
                return NotFound();
            }
            ViewData["FkProduct"] = new SelectList(_context.Products, "IdProduct", "Name", stocks.FkProduct);
            ViewData["FkStorehouse"] = new SelectList(availableStorehouses, "IdStorehouse", "IdStorehouse", stocks.FkStorehouse);
            return View(stocks);
        }

        // POST: Stocks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdStock,FkProduct,Amount,FkStorehouse,Status")] Stocks stocks)
        {
            ViewBag.COOKIE_LOGGED_USERNAME = Request.Cookies["COOKIE_LOGGED_USERNAME"];

            if (id != stocks.IdStock)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stocks);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StocksExists(stocks.IdStock))
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
            ViewData["FkProduct"] = new SelectList(_context.Products, "IdProduct", "Name", stocks.FkProduct);
            ViewData["FkStorehouse"] = new SelectList(_context.Storehouses, "IdStorehouse", "IdStorehouse", stocks.FkStorehouse);
            return View(stocks);
        }

        // GET: Stocks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewBag.COOKIE_LOGGED_USERNAME = Request.Cookies["COOKIE_LOGGED_USERNAME"];

            if (id == null)
            {
                return NotFound();
            }

            var stocks = await _context.Stocks
                .Include(s => s.FkProductNavigation)
                .Include(s => s.FkStorehouseNavigation)
                .FirstOrDefaultAsync(m => m.IdStock == id);
            if (stocks == null)
            {
                return NotFound();
            }

            return View(stocks);
        }

        // POST: Stocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stocks = await _context.Stocks.FindAsync(id);
            _context.Stocks.Remove(stocks);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StocksExists(int id)
        {
            return _context.Stocks.Any(e => e.IdStock == id);
        }
    }
}
