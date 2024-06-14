using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClasses
{
    public class UserCart
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("ProductId")]
        public int? ProductId { get; set; }
        public Product Product { get; set; }  
        public string userId {  get; set; }
        [ForeignKey("userId")]
        public ApplicationUser ApplicationUser { get; set; }
        [Range(1,100,ErrorMessage ="1 through 100 ")]
        public int Quantity { get; set; }
        [NotMapped]
        public double Price { get; set; }
    }
}
