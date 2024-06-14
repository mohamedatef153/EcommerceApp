using DatabaseAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelClasses;

namespace webApp.Controllers
{
    public class CategoryController : Controller
    { private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories= _context.Categories.ToList();
            return View(categories);
        }
        [Authorize]
        public IActionResult Upsert(int? id)
        {
           if(id == null)
            {
                Category category = new Category();
                 return View(category);
               
            }
            else
            {
                var category = _context.Categories.FirstOrDefault(c => c.Id == id);
                return View(category);
            }

        }
        [HttpPost]
        public async Task<IActionResult> Upsert(int? id,Category category)
        {
            if(id == null) { 
            var founditem= await _context.Categories.FirstOrDefaultAsync(c => c.Name == category.Name);
             if(founditem != null)
                {
                    TempData["AlertMessage"]= category.Name+ " Is an existing Item in Category List";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["AlertMessage"] = category.Name + " Has been added  in Category List";
                    await _context.Categories.AddAsync(category);
                }
            }
            else
            {
                var item= await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
                item.Name = category.Name;
                TempData["AlertMessage"] = category.Name + " Has been Edited  in Category List";
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }
        public IActionResult Delete(int id)
        {
            
           
            
                var category = _context.Categories.FirstOrDefault(c => c.Id == id);
                return View(category);
            

        }
        [HttpPost]
        public async Task<IActionResult> Delete(Category category)
        {



            var item = _context.Categories.FirstOrDefault(c => c.Id == category.Id);
           _context.Categories.Remove(item);
            await _context.SaveChangesAsync();
            TempData["AlertMessage"] = category.Name + " Has been deleted from Category List";

            return RedirectToAction("Index");
        }
    }
}
