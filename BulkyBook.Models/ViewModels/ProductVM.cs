using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    public class ProductVM
    {
        //This Class is another way of passing model to view.The first two was ViewBag  and ViewData in ProductControoler Class.
       public Product Product { get; set; }
        // Dropdown list of category and CoverType List
        [ValidateNever]
       public IEnumerable<SelectListItem> CategoryList { get; set; }
        [ValidateNever]
       public  IEnumerable<SelectListItem> CoverTypeList { get; set; }
    }
}
