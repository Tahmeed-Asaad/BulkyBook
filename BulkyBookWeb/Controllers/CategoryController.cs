using BulkyBookWeb.Data;
using BulkyBookWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyBookWeb.Controllers
{
    public class CategoryController : Controller

    {

        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db=db;
        }
    
        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList= _db.Categories;   

            //Console.WriteLine(DateTime.Now.ToString());

            return View(objCategoryList);
        }

        //Its a GET METHOD. This method will lead to a view which has a form to create new categories
        public IActionResult Create()
        {
            return View();
        }

        //Post method. It will actually create new categories and insert the category into database table.  
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(Category obj)
        {
            //ModelState.IsValid wil check if obj is a proper instance of category class.
            //That means all the constraints in the class should be there.So, name can't be
            //null and displayorder should be integer.
            if (ModelState.IsValid)
            {

                _db.Categories.Add(obj);
                //Actually adds the new category into table
                _db.SaveChanges();
                //Tempdata stays only for one redirect.On reload it disappears
                TempData["Success"] = "Category created successfully";
                //Redirecting to Index Action
                return RedirectToAction(nameof(Index));
            }

            return View(obj);
        }

        //Get

        public IActionResult Edit(int? id)
        {
            if(id==null || id == 0)
            {
                return NotFound();
            }

            var category= _db.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        //Post method. It will update the category into database table.  
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(Category obj)
        {
            //ModelState.IsValid wil check if obj is a proper instance of category class.
            //That means all the constraints in the class should be there.So, name can't be
            //null and displayorder should be integer.
            if (ModelState.IsValid)
            {

                _db.Categories.Update(obj);
                //Actually adds the new category into table
                _db.SaveChanges();
                //Tempdata stays only for one redirect.On reload it disappears
                TempData["Success"] = "Category Edited successfully";
                //Redirecting to Index Action
                return RedirectToAction(nameof(Index));
            }

            return View(obj);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = _db.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        //Post method. It will delete the category from database table.  
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var category = _db.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            _db.Categories.Remove(category);
            _db.SaveChanges();
            //Tempdata stays only for one redirect.On reload it disappears
            TempData["Success"] = "Category deleted successfully";
            //Redirecting to Index Action
            return RedirectToAction(nameof(Index));
            

            
        }
    }
}
