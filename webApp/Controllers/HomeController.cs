using DatabaseAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelClasses.ViewModels;
using System.Diagnostics;
using webApp.Models;
using webApp.Utiltiy;

namespace webApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _UserManager;
        private readonly SignInManager<IdentityUser> _SignInManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _context = context;
            _UserManager = userManager;
            _SignInManager = signInManager;
        }

        public IActionResult Index(string? SearchByName,int? searchByCategoryId)
        {

            var claims = _SignInManager.IsSignedIn(User);
            if (claims)
            {
                var user = _UserManager.GetUserId(User);
                // how many items the user has in cart ..... each item consider as line = cart 
                var count = _context.userCarts.Where(u => u.userId.Contains(user)).Count();
                HttpContext.Session.SetInt32(cartCount.sessionCount, count);
            
            
            
            
            
            
            }
           

            HomeViewModel viewModel = new HomeViewModel();
            if (SearchByName != null)
            {
                viewModel.Categories = _context.Categories.ToList();
                viewModel.Products = _context.products.Where(productName=> EF.Functions.Like(productName.Name,$"%{SearchByName}")).ToList();
            }
            else if (searchByCategoryId !=null)
            {
                viewModel.Products = _context.products.Where(m=>m.CategoryId == searchByCategoryId).ToList();
                viewModel.Categories=_context.Categories.Where(m=>m.Id == searchByCategoryId).ToList();
            }
            else
            {
                viewModel.Categories = _context.Categories.ToList();
                viewModel.Products = _context.products.ToList();
            }
            return View(viewModel);
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
