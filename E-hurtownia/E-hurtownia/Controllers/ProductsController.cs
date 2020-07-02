using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using E_hurtownia.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using Microsoft.Extensions.Logging;

namespace E_hurtownia.Controllers
{
    public class ProductsController : Controller
    {
        private readonly EhurtowniaContext _context;

        public ProductsController(EhurtowniaContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var ehurtowniaContext = _context.Products.Include(p => p.FkUnitNavigation);
            return View(await ehurtowniaContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .Include(p => p.FkUnitNavigation)
                .FirstOrDefaultAsync(m => m.IdProduct == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["FkUnit"] = new SelectList(_context.Units, "IdUnit", "Name");
            return View();
        }

        public async Task<IActionResult> ChangeImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }
            ViewBag.FileEmptyError = TempData["FileEmptyError"];
            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file, int idProduct)
        {
            if (file == null)
            {
                TempData["FileEmptyError"] = true;
                return RedirectToAction("ChangeImage", new RouteValueDictionary(new { id = idProduct }));
            }

            var product = await _context.Products.FindAsync(idProduct);

            if (product == null)
                return NotFound();

            product.DeleteImageFile();

            product.ImgFile = Path.Combine(@"/images/products/", file.FileName);
            await _context.SaveChangesAsync();

            string serverPath = product.GetServerPath(product.ImgFile);

            using (var stream = System.IO.File.Create(serverPath))
                await file.CopyToAsync(stream);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ChangePdf(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }
            ViewBag.FileEmptyError = TempData["FileEmptyError"];
            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> UploadPdf(IFormFile file, int idProduct)
        {
            if (file == null)
            {
                TempData["FileEmptyError"] = true;
                return RedirectToAction("ChangePdf", new RouteValueDictionary(new { id = idProduct }));
            }

            var product = await _context.Products.FindAsync(idProduct);

            if (product == null)
                return NotFound();

            product.DeletePdfFile();

            product.PdfFile = Path.Combine(@"/pdf/products/", file.FileName);
            await _context.SaveChangesAsync();

            string serverPath = product.GetServerPath(product.PdfFile);

            using (var stream = System.IO.File.Create(serverPath))
                await file.CopyToAsync(stream);

            return RedirectToAction("Index");
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdProduct,Name,BasePricePerUnit,ImgFile,PdfFile,FkUnit")] Products products)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    products.IdProduct = _context.Products.Max(product => product.IdProduct) + 1;
                }
                catch(Exception e)
                {
                    products.IdProduct = 1;
                }
                _context.Add(products);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FkUnit"] = new SelectList(_context.Units, "IdUnit", "Name", products.FkUnit);
            return View(products);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }
            ViewData["FkUnit"] = new SelectList(_context.Units, "IdUnit", "Name", products.FkUnit);
            return View(products);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdProduct,Name,BasePricePerUnit,ImgFile,PdfFile,FkUnit")] Products products)
        {
            if (id != products.IdProduct)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(products);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsExists(products.IdProduct))
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
            ViewData["FkUnit"] = new SelectList(_context.Units, "IdUnit", "Name", products.FkUnit);
            return View(products);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .Include(p => p.FkUnitNavigation)
                .FirstOrDefaultAsync(m => m.IdProduct == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var products = await _context.Products.FindAsync(id);
            _context.Products.Remove(products);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsExists(int id)
        {
            return _context.Products.Any(e => e.IdProduct == id);
        }
    }
}
