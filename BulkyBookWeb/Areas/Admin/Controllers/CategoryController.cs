using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers;
[Area("Admin")]


public class CategoryController : Controller

{

    //private readonly ApplicationDbContext _db;

    //Replacing ApplicationDbContext Category Repository
    //private readonly ICategoryRepository _db;
    //Replacing ICategory Repository with UnitofWork

    private readonly IUnitOfWork _db;

    public CategoryController(IUnitOfWork db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        //IEnumerable<Category> objCategoryList= _db.Categories;
        IEnumerable<Category> objCategoryList = _db.Category.GetAll();

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

            //_db.Categories.Add(obj);
            //_db.Add(obj);
            _db.Category.Add(obj);
            //Actually adds the new category into table
            //_db.SaveChanges();
            _db.Save();
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
        if (id == null || id == 0)
        {
            return NotFound();
        }

        //var category= _db.Categories.Find(id);
        //var categoryFromDbFirst=_db.Categories.FirstOrDefault(c => c.Id == id);
        //var categoryFromDbFirst = _db.GetFirstorDefault(u => u.Id == id);
        var categoryFromDbFirst = _db.Category.GetFirstorDefault(u => u.Id == id);

        if (categoryFromDbFirst == null)
        {
            return NotFound();
        }
        return View(categoryFromDbFirst);
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

            //_db.Categories.Update(obj);
            _db.Category.Update(obj);
            //Actually adds the new category into table
            //_db.SaveChanges();
            _db.Save();
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

        //var category = _db.Categories.Find(id);
        var CategoryFromDbFirst = _db.Category.GetFirstorDefault(u => u.Id == id);

        if (CategoryFromDbFirst == null)
        {
            return NotFound();
        }
        return View(CategoryFromDbFirst);
    }

    //Post method. It will delete the category from database table.  
    [HttpPost]
    [AutoValidateAntiforgeryToken]
    public IActionResult DeletePost(int? id)
    {
        //var category = _db.Categories.Find(id);
        var category = _db.Category.GetFirstorDefault(u => u.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        //_db.Categories.Remove(category);
        _db.Category.Remove(category);
        //_db.SaveChanges();
        _db.Save();
        //Tempdata stays only for one redirect.On reload it disappears
        TempData["Success"] = "Category deleted successfully";
        //Redirecting to Index Action
        return RedirectToAction(nameof(Index));



    }
}

