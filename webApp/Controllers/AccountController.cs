using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelClasses;
using ModelClasses.ViewModels;

namespace webApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        //used to register
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        // return url to store the url returned from authorize attr to use it to navigate to the page you want after login ... route-returnurl in the login form 
        public IActionResult Login(string returnUrl=null)
        {
            ViewData["returnUrl"]= returnUrl;
            LoginViewModel vm = new LoginViewModel();
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm, string returnUrl = null)
        {
            if (ModelState.IsValid)
            { 
                var result= await _signInManager.PasswordSignInAsync(vm.Email, vm.Password,isPersistent:false,lockoutOnFailure:true);
                if (result.Succeeded)
                {
                    if(returnUrl!=null && Url.IsLocalUrl(returnUrl))
                    {
                       return Redirect(returnUrl);
                    }else
                      return RedirectToAction("Index","Home");
                }
            }

            vm.LoginStatus = "Unsuccessfull Login";
           

            return View(vm);
        }
        public IActionResult Register()
        {
            RegisterViewModel vm = new RegisterViewModel();
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            // this user will be added to the data base
            var user = new ApplicationUser
            {
                FirstName = vm.applicationUser.FirstName,
                LastName = vm.applicationUser.LastName,
                Email = vm.Email,
                UserName = vm.UserName,
                Address = vm.applicationUser.Address,
                City = vm.applicationUser.City,

            };
            var regiseter = await _userManager.CreateAsync(user, vm.Password);
            if(regiseter.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent:false);
                vm.StatusMessage = "Register Succedded";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                vm.StatusMessage = "Register Failed";
            }
            return View(vm);
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        }
}
