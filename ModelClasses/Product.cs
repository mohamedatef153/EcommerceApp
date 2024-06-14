using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClasses
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Range(1,999,ErrorMessage ="Range from 1 to 999 only")]
        [RegularExpression(@"^[0-9]+(\.[0-9]{1,2})$",ErrorMessage ="Please insert two digits after decimal Example: 0.00")]
        public double Price { get; set; }
        [Required]
        [MaxLength(2000,ErrorMessage ="Length cannot exceed more than 30 char")]
        public string Discreption { get; set; }

        public ICollection<PImages>? ImgUrl { get; set;}
        [Required]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public string? HomeImgUrl { get; set; }





    }
}
