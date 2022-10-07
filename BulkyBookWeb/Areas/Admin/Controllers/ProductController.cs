using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using BulkyBook.Utility;

namespace BulkyBookWeb.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class ProductController : Controller

{

    //private readonly ApplicationDbContext _db;

    //Replacing ApplicationDbContext Category Repository
    //private readonly ICategoryRepository _db;
    //Replacing ICategory Repository with UnitofWork

    private readonly IUnitOfWork _db;
    //Connecting to wwwroot for images
    private readonly IWebHostEnvironment _hostEnvironment;

    public ProductController(IUnitOfWork db, IWebHostEnvironment hostEnvironment)
    {
        _db = db;
        _hostEnvironment = hostEnvironment;
    }

    public IActionResult Index()
    {
        //IEnumerable<Category> objCategoryList= _db.Categories;

        return View();
    }

    
    //Get
    //Upsert is Insert+Update

    public IActionResult Upsert(int? id)

    {
        /* Product product = new();
         // Dropdown list of category and CoverType List
         IEnumerable<SelectListItem> CategoryList = _db.Category.GetAll().Select(

             u=> new SelectListItem
             {
                 Text = u.Name,
                 //When Text is Selected then Value is returned
                 Value= u.Id.ToString() 
             }

         );

         IEnumerable<SelectListItem> CoverTypeList = _db.CoverType.GetAll().Select(

          u => new SelectListItem
          {
              Text = u.Name,
              //When Text is Selected then Value is returned
              Value = u.Id.ToString()
          }

      );*/

        //Using ProductVM innstead of ViewData and ViewBag
        //If we need 10 more properties along with Category and CoverType we can add them in ProductVM. It will make controller less ugly.
        //This is called Tightly Binded View

        ProductVM productVM = new()
        {
            Product = new(),

            CategoryList = _db.Category.GetAll().Select(

             u => new SelectListItem
             {
                 Text = u.Name,
                 //When Text is Selected then Value is returned
                 Value = u.Id.ToString()
             }),

            CoverTypeList = _db.CoverType.GetAll().Select(

            u => new SelectListItem
            {
                Text = u.Name,
                //When Text is Selected then Value is returned
                Value = u.Id.ToString()
            }),

        };



        if (id == null || id == 0)

        {
            //This condition means we have to create product

            //ViewBag.CategoryList = CategoryList;

            //We will pass CovertYPE List to view using ViewData for learning. categoylist was passed using ViewBag

            //ViewData["CoverTypeList"] = CoverTypeList;


            //return View(product);
            return View(productVM);
        }

        else
        {
            //This  means we have to update a product
            productVM.Product = _db.Product.GetFirstorDefault(u => u.Id == id);
            return View(productVM);
           
        }

        //var category= _db.Categories.Find(id);
        //var categoryFromDbFirst=_db.Categories.FirstOrDefault(c => c.Id == id);
        //var categoryFromDbFirst = _db.GetFirstorDefault(u => u.Id == id);
        
           
        
    }

    //Upsert is Insert+Update
    //Post method. It will insert+update the product into database table.  
    [HttpPost]
    [AutoValidateAntiforgeryToken]
    public IActionResult Upsert(ProductVM obj, IFormFile? file)
    {
        //ModelState.IsValid wil check if obj is a proper instance of category class.
        //That means all the constraints in the class should be there.So, name can't be
        //null and displayorder should be integer.

        //IformFile is needed for image Uploading
        if (ModelState.IsValid)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;

            if(file != null)
            {
                //File not equals null means file was uploaded
                string filename = Guid.NewGuid().ToString();
                //file upload path definition
                var uploads = Path.Combine(wwwRootPath, @"images\products");
                var extension = Path.GetExtension(file.FileName);

           

                if (obj.Product.ImageURL != null)
                {
                    //This if block is for updating product.
                    //Delete Oldimage Url and then  insert new url for that row
                    //1 forward slash is escape character. 2 forward slash is required thus in TrimStart.
                    var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageURL.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        //If file exists so we delete that
                        System.IO.File.Delete(oldImagePath);
                    }

                }

                using(var fileStreams= new FileStream(Path.Combine(uploads, filename+extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }

                obj.Product.ImageURL = @"\images\products\" + filename + extension;

            }
            //_db.Categories.Update(obj);

            if (obj.Product.Id == 0) {
                //Add only when the ID is not present in the table.
                _db.Product.Add(obj.Product);
                TempData["Success"] = "Product Created successfully";
            }

            else
            {
                //If ID is present then we  update.
                _db.Product.Update(obj.Product);
                TempData["Success"] = "Product Updated successfully";
            }
            //Actually adds the new category into table
            //_db.SaveChanges();
            _db.Save();
            //Tempdata stays only for one redirect.On reload it disappears
            //TempData["Success"] = "Product Created successfully";
            //Redirecting to Index Action
            return RedirectToAction(nameof(Index));
        }

        return View(obj);
    }

    //No need for this method as in API region we have another replacing function.
    /*public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        //var category = _db.Categories.Find(id);
        var ProductFromDbFirst = _db.Product.GetFirstorDefault(u => u.Id == id);

        if (ProductFromDbFirst == null)
        {
            return NotFound();
        }
        return View(ProductFromDbFirst);
    }*/

    
    //API CALLS FOR DATA TABLES>NET

    #region API CALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        var productList = _db.Product.GetAll(includeProperties:"Category,CoverType");
        return Json(new {data=productList});
    }

    

    //Post method. It will delete the category from database table.  
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        //var category = _db.Categories.Find(id);
        var ProductType = _db.Product.GetFirstorDefault(u => u.Id == id);

        if (ProductType == null)
        {
            return Json(new {success=false, message="Error while deleting"});
        }

        //For deleting image from folder.
        var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, ProductType.ImageURL.TrimStart('\\'));
        if (System.IO.File.Exists(oldImagePath))
        {
            //If file exists so we delete that
            System.IO.File.Delete(oldImagePath);
        }

        //_db.Categories.Remove(category);
        _db.Product.Remove(ProductType);
        //_db.SaveChanges();
        _db.Save();
        //Tempdata stays only for one redirect.On reload it disappears
        //TempData["Success"] = "Product deleted successfully";
        //Redirecting to Index Action
        //return RedirectToAction(nameof(Index));
        return Json(new { success = true, message = "Product deleted successfully" });



    }

    #endregion
}
