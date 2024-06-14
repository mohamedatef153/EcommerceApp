using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClasses
{
    public class UserOrderHeader
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        [Required]
        public DateTime DateOfOrder { get; set; }
        [Required]
        public DateTime DateOfShipped { get; set; }
        [Required]
        public double TotalOrderAmount { get; set; }
        public string? TrackingNumber { get; set; }
        // carrie name
        public string? Carrier { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? PaymentProcecessDate { get; set; }
        public string? TransactionId { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string DeliveryStreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
