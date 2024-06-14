using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClasses.ViewModels
{
    public class SummaryVM
    {
        public IEnumerable<UserCart> UserCartList { get; set; }
        public UserOrderHeader? orderSummary { get; set; }
        public string? cartUserId {  get; set; }
        public IEnumerable<SelectListItem>? PaymentOptions { get; set; }
        public double? PaymentPaidByCard { get; set; } = 0.00;
    }
}
