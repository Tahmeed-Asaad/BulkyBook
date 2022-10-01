using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers;
[Area("Admin")]
public class CompanyController : Controller

{

    //private readonly ApplicationDbContext _db;

    //Replacing ApplicationDbContext Category Repository
    //private readonly ICategoryRepository _db;
    //Replacing ICategory Repository with UnitofWork

    private readonly IUnitOfWork _db;
  

    public CompanyController(IUnitOfWork db)
    {
        _db = db;
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

        Company company = new();
        

        if (id == null || id == 0)

        {
            //This condition means we have to create product

            //ViewBag.CategoryList = CategoryList;

            //We will pass CovertYPE List to view using ViewData for learning. categoylist was passed using ViewBag

            //ViewData["CoverTypeList"] = CoverTypeList;


            //return View(product);
            return View(company);
        }

        else
        {
            //This  means we have to update a product
            company = _db.Company.GetFirstorDefault(u => u.Id == id);
            return View(company);
           
        }

        //var category= _db.Categories.Find(id);
        //var categoryFromDbFirst=_db.Categories.FirstOrDefault(c => c.Id == id);
        //var categoryFromDbFirst = _db.GetFirstorDefault(u => u.Id == id);
        
           
        
    }

    //Upsert is Insert+Update
    //Post method. It will insert+update the product into database table.  
    [HttpPost]
    [AutoValidateAntiforgeryToken]
    public IActionResult Upsert(Company obj, IFormFile? file)
    {
        //ModelState.IsValid wil check if obj is a proper instance of category class.
        //That means all the constraints in the class should be there.So, name can't be
        //null and displayorder should be integer.

        //IformFile is needed for image Uploading
        if (ModelState.IsValid)
        {
  
            //_db.Categories.Update(obj);

            if (obj.Id == 0) {
                //Add only when the ID is not present in the table.
                _db.Company.Add(obj);
                TempData["Success"] = "Company Created successfully";
            }

            else
            {
                //If ID is present then we  update.
                _db.Company.Update(obj);
                TempData["Success"] = "Company Updated successfully";
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
        var companyList = _db.Company.GetAll();
        return Json(new {data=companyList});
    }

    

    //Post method. It will delete the category from database table.  
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        //var category = _db.Categories.Find(id);
        var CompanyType = _db.Company.GetFirstorDefault(u => u.Id == id);

        if (CompanyType == null)
        {
            return Json(new {success=false, message="Error while deleting"});
        }

        //For deleting image from folder.
    

        //_db.Categories.Remove(category);
        _db.Company.Remove(CompanyType);
        //_db.SaveChanges();
        _db.Save();
        //Tempdata stays only for one redirect.On reload it disappears
        //TempData["Success"] = "Product deleted successfully";
        //Redirecting to Index Action
        //return RedirectToAction(nameof(Index));
        return Json(new { success = true, message = "Company deleted successfully" });



    }

    #endregion
}
