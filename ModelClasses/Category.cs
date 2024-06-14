using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClasses
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(30,ErrorMessage ="Length Cant be above 30")]
        public string Name { get; set; }

    }
}
