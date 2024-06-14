using DatabaseAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelClasses;
using ModelClasses.ViewModels;
using webApp.Utiltiy;

namespace webApp.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _UserManager;
        private readonly SignInManager<IdentityUser> _SignInManager;

        public CartController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _UserManager = userManager;
            _SignInManager = signInManager;
        }

        

        public IActionResult CartIndex()
        {
            var claims = _SignInManager.IsSignedIn(User);
            if (claims)
            {
                var userid= _UserManager.GetUserId(User);
                CartViewModel model = new CartViewModel()
                {
                    productList = _context.userCarts.Include(u => u.Product).Where(m=>m.userId == userid).ToList(),
                };
                var count = _context.userCarts.Where(u => u.userId.Contains(userid)).Count();
                HttpContext.Session.SetInt32(cartCount.sessionCount, count);
                return View(model);
            }
            return Content("not signed in");
           
        }
        [Authorize]
        public async Task<IActionResult> AddtoCart(int? productId, string? returnUrl)
        {
            var productToCart= await _context.products.FirstOrDefaultAsync(m=>m.Id==productId);
            var checkIfuserSignedin = _SignInManager.IsSignedIn(User);
            if (checkIfuserSignedin)
            {
                var user = _UserManager.GetUserId(User);
                if (user != null) {
                    //1- check if signed in user has card or not 
                    var getTheCartIfAnyExistForTheUser = await _context.userCarts.Where(u => u.userId.Contains(user)).ToListAsync();
                    if (getTheCartIfAnyExistForTheUser.Count() > 0)
                    {
                        //2- check if item already in the cart or not 
                        var getTheQuantity = getTheCartIfAnyExistForTheUser.FirstOrDefault(p => p.ProductId == productId);
                        if (getTheQuantity != null)
                        {
                            // 3- if it already their add the quantity and update the cart by one 
                            getTheQuantity.Quantity = getTheQuantity.Quantity + 1;
                            _context.userCarts.Update(getTheQuantity);

                        }
                        else
                        {
                            // user has card but adding new item 
                            UserCart newitem = new UserCart()
                            {
                                ProductId = productId,
                                userId = user,
                                Quantity = 1
                            };
                            await _context.userCarts.AddAsync(newitem);

                        }
                    }
                    else {

                        // user has no cart 
                        UserCart newitem = new UserCart()
                        {
                            ProductId = productId,
                            userId = user,
                            Quantity = 1
                        };

                        await _context.userCarts.AddAsync(newitem);
                    }
                await _context.SaveChangesAsync();
                }
            }
            if (returnUrl!=null)
            {
                return RedirectToAction("CartIndex", "Cart");
            }

            return RedirectToAction("Index","Home");
        }
        public IActionResult MinusItem(int ProductId)
        {
            if (ProductId != null)
            {
                var item = _context.userCarts.FirstOrDefault(p => p.ProductId == ProductId);
                if (item != null)
                {
                    item.Quantity--;

                    if (item.Quantity <= 0)
                    {
                        _context.userCarts.Remove(item);
                    }
                    else
                    {
                        _context.userCarts.Update(item);
                    }

                    _context.SaveChanges();
                }

               
            }
            return RedirectToAction("CartIndex", "Cart");
        }
        public IActionResult Delete(int ProductId) {
            var item = _context.userCarts.FirstOrDefault(p => p.ProductId == ProductId);
            if (item != null)
            {
                _context.userCarts.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("CartIndex", "Cart");
        }
    }
}
