using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        [Range(1,1000)]
        [Display(Name = "List Price")]
        public double ListPrice { get; set; }
        [Required]
        [Range(1,1000)]
        [Display(Name = "Price for 1-50")]
        public double Price { get; set; }

        [Required]
        [Range(1, 1000)]
        [Display(Name = "Price for 51-100")]
        public double Price50 { get; set; }

        [Required]
        [Range(1, 1000)]
        [Display(Name = "Price for 100+")]
        public double Price100 { get; set; }
        [ValidateNever]
        public string ImageURL { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        //Entity Framework automatically references CategoryId as Foregn Key of Categrory model as below a Category class is declared
        //Another way to declare foreign key is below. 
        //[ForeignKey("CategoryID")]
        [ValidateNever]
        public Category Category { get; set; }

        [Required]
        [Display(Name ="Cover Type")]
        public int CoverTypeID { get; set; }
        [ValidateNever]
        public CoverType CoverType { get; set; }
    }
}
