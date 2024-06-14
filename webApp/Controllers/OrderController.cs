using DatabaseAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelClasses;
using ModelClasses.ViewModels;
using webApp.Utiltiy;

namespace webApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _UserManager;
        private readonly SignInManager<IdentityUser> _SignInManager;
        [BindProperty]
        public OrderDetailsVM OrderDetailsVM { get; set; }

        public OrderController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _UserManager = userManager;
            _SignInManager = signInManager;
        }


        public IActionResult orderDetailPreview()
        {
            var claim = _SignInManager.IsSignedIn(User);
            if (claim)
            {
                var userId = _UserManager.GetUserId(User);
                var currentUser = _context.applicationUser.FirstOrDefault(x => x.Id == userId);
                SummaryVM summary = new SummaryVM()
                {
                    UserCartList = _context.userCarts.Include(u => u.Product).Where(u => u.userId.Contains(userId)).ToList(),
                    orderSummary = new UserOrderHeader(),
                    cartUserId = userId,
                };
                if (currentUser != null)
                { // assigning the users info from data base as default  data

                    summary.orderSummary.DeliveryStreetAddress = currentUser.Address;
                    summary.orderSummary.City = currentUser.City;
                    summary.orderSummary.State = currentUser.State;
                    summary.orderSummary.PostalCode = currentUser.PostalCode;
                    summary.orderSummary.Name = currentUser.FirstName + " " + currentUser.LastName;
                }
                var count = _context.userCarts.Where(u => u.userId.Contains(_UserManager.GetUserId(User))).ToList().Count();
                HttpContext.Session.SetInt32(cartCount.sessionCount, count);
                return View(summary);
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Summary(SummaryVM VMFromView)
        {
            var claim = _SignInManager.IsSignedIn(User);
            if (claim)
            {
                var userId = _UserManager.GetUserId(User);
                var currentUser = _context.applicationUser.FirstOrDefault(x => x.Id == userId);
                SummaryVM summary = new SummaryVM()
                {
                    UserCartList = _context.userCarts.Include(u => u.Product).Where(u => u.userId.Contains(userId)).ToList(),
                    orderSummary = new UserOrderHeader(),
                    
                };
                if (currentUser != null)
                { // assigning the users info from data base as default  data

                    summary.orderSummary.DeliveryStreetAddress = VMFromView.orderSummary.DeliveryStreetAddress;
                   summary.orderSummary.UserId= VMFromView.cartUserId;
                    summary.orderSummary.City = VMFromView.orderSummary.City;
                    summary.orderSummary.State = VMFromView.orderSummary.State;
                    summary.orderSummary.PhoneNumber = VMFromView.orderSummary.PhoneNumber; 
                    summary.orderSummary.PostalCode = VMFromView.orderSummary.PostalCode;
                    summary.orderSummary.Name =    VMFromView.orderSummary.Name;
                    summary.orderSummary.TotalOrderAmount = VMFromView.orderSummary.TotalOrderAmount;
                    summary.orderSummary.DateOfOrder= DateTime.Now;
                    summary.orderSummary.OrderStatus = "pending";
                    summary.orderSummary.PaymentStatus = "Not Paid";
                    await _context.AddAsync(summary.orderSummary);
                    await _context.SaveChangesAsync();

                }
                if ( VMFromView.orderSummary.TotalOrderAmount>0)
                {
                    var CardChargeFee = (VMFromView.orderSummary.TotalOrderAmount / 100) * 2.90 + .30;
                    double creditCardBalance = 3000.00;
                    if(creditCardBalance > VMFromView.orderSummary.TotalOrderAmount + CardChargeFee)
                    {
                        return RedirectToAction("OrderSuccess", new { id = summary.orderSummary.Id });
                    }
                    else
                    {
                        return RedirectToAction("OrderCancle");
                    }
                }
               
            }
            return View();
        }
        public IActionResult OrderCancle()
        { 

          return RedirectToAction("CartIndex","Cart"); 
        
        }
    
        public IActionResult OrderSuccess(int id)
        {

            var claim = _SignInManager.IsSignedIn(User);
            if (claim) { 
             var userId= _UserManager.GetUserId(User);
                // we will need to empty the cart after purchasing 
                var UserCartRemove = _context.userCarts.Where(u=>u.userId.Contains(userId)).ToList();   
                var orderProcessed= _context.orderHeaders.FirstOrDefault(h=>h.Id==id);
                if (orderProcessed != null) { 
                if(orderProcessed.PaymentStatus== "Not Paid")
                    {
                        orderProcessed.PaymentStatus = "Paid";
                    }
                }
                // add the items from cart to order details table 
                foreach (var list in UserCartRemove)
                    {
                        OrderDetails orederReceived = new OrderDetails()
                        {
                            OrderHeaderId = orderProcessed.Id,
                            productId = (int) list.ProductId,
                            Count = list.Quantity,

                        };
                        _context.orderDetails.Add(orederReceived);
                    }
                    //remove itesm from cart for the current user 
                    _context.userCarts.RemoveRange(UserCartRemove);
                    _context.SaveChanges();
                    var count = _context.userCarts.Where(u => u.userId.Contains(_UserManager.GetUserId(User))).ToList().Count();
                    HttpContext.Session.SetInt32(cartCount.sessionCount, count);
                
                
            }
            return View();
        }
        public IActionResult OrderHistory(string? status)
        {
            IEnumerable<UserOrderHeader> header = new List<UserOrderHeader>();
            var userId = _UserManager.GetUserId(User);
            if (status !=null && status !="All")
            {
                if (User.IsInRole("Admin"))
                {
                    header= _context.orderHeaders.Where(u=>u.OrderStatus == status).ToList();    
                }
                else
                {
                    header = _context.orderHeaders.Where(u => u.OrderStatus == status && u.UserId == userId).ToList();
                }

            }
            else
            {
                if (User.IsInRole("Admin"))
                {
                    header = _context.orderHeaders.ToList();
                }
                else
                {
                    header = _context.orderHeaders.Where(u => u.UserId== userId).ToList();
                }
            }
          
            return View(header);

        }
       public IActionResult OrderDetails(int id)
        {
            OrderDetailsVM = new OrderDetailsVM();
            OrderDetailsVM.orderHeader = _context.orderHeaders.FirstOrDefault(h=>h.Id == id);
            OrderDetailsVM.userProductList= _context.orderDetails.Include(u=>u.Product).Where(u=>u.OrderHeaderId == id).ToList();

             return View(OrderDetailsVM); 
        
        
        
        }
        public IActionResult InProcess()
        {
            var orderToUpdate = _context.orderHeaders.FirstOrDefault(u=>u.Id == OrderDetailsVM.orderHeader.Id);
            if (orderToUpdate != null)
            {
                orderToUpdate.OrderStatus= updateOrderStatus.PaymentStatusInprocess;
                _context.orderHeaders.Update(orderToUpdate);
                _context.SaveChanges();
            }

            return RedirectToAction("OrderDetails", new {id= OrderDetailsVM.orderHeader.Id});

        }
        public IActionResult Shipped()
        {
            var orderToUpdate = _context.orderHeaders.FirstOrDefault(u => u.Id == OrderDetailsVM.orderHeader.Id);
            if (orderToUpdate != null)
            {
                orderToUpdate.OrderStatus = updateOrderStatus.PaymentStatusShipped;
                orderToUpdate.Carrier = OrderDetailsVM.orderHeader.Carrier;
                orderToUpdate.TrackingNumber = OrderDetailsVM.orderHeader.TrackingNumber;
                orderToUpdate.DateOfShipped = DateTime.Now;
                _context.orderHeaders.Update(orderToUpdate);
                _context.SaveChanges();
            }

            return RedirectToAction("OrderDetails", new { id = OrderDetailsVM.orderHeader.Id });

        }
        public IActionResult Delivered()
        {
            var orderToUpdate = _context.orderHeaders.FirstOrDefault(u => u.Id == OrderDetailsVM.orderHeader.Id);
            if (orderToUpdate != null)
            {
                orderToUpdate.OrderStatus = updateOrderStatus.PaymentStatusCompleted;
               
                
                _context.orderHeaders.Update(orderToUpdate);
                _context.SaveChanges();
            }

            return RedirectToAction("OrderDetails", new { id = OrderDetailsVM.orderHeader.Id });

        }

    }
}