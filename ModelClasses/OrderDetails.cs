using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClasses
{
    public class OrderDetails
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey("OrderHeaderId")]
        public int OrderHeaderId { get; set; }
        public UserOrderHeader? OrderHeader { get; set; }
        [Required]
        [ForeignKey("productId")]
        public int productId {  get; set; }
        public Product? Product { get; set; }
        [Required]//quantity of each unique item for specific individual User order header
        public int Count {  get; set; }
        [Required]
        public double Price { get; set; }
        

    }
}
