using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]

public class CoverTypeController : Controller

{

    //private readonly ApplicationDbContext _db;

    //Replacing ApplicationDbContext Category Repository
    //private readonly ICategoryRepository _db;
    //Replacing ICategory Repository with UnitofWork

    private readonly IUnitOfWork _db;

    public CoverTypeController(IUnitOfWork db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        //IEnumerable<Category> objCategoryList= _db.Categories;
        IEnumerable<CoverType> objCoverTypeList = _db.CoverType.GetAll();

        //Console.WriteLine(DateTime.Now.ToString());

        return View(objCoverTypeList);
    }

    //Its a GET METHOD. This method will lead to a view which has a form to create new categories
    public IActionResult Create()
    {
        return View();
    }

    //Post method. It will actually create new categories and insert the category into database table.  
    [HttpPost]
    [AutoValidateAntiforgeryToken]
    public IActionResult Create(CoverType obj)
    {
        //ModelState.IsValid wil check if obj is a proper instance of category class.
        //That means all the constraints in the class should be there.So, name can't be
        //null and displayorder should be integer.
        if (ModelState.IsValid)
        {

            //_db.Categories.Add(obj);
            //_db.Add(obj);
            _db.CoverType.Add(obj);
            //Actually adds the new category into table
            //_db.SaveChanges();
            _db.Save();
            //Tempdata stays only for one redirect.On reload it disappears
            TempData["Success"] = "CoverType created successfully";
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
        var coverTypeFromDbFirst = _db.CoverType.GetFirstorDefault(u => u.Id == id);

        if (coverTypeFromDbFirst == null)
        {
            return NotFound();
        }
        return View(coverTypeFromDbFirst);
    }

    //Post method. It will update the category into database table.  
    [HttpPost]
    [AutoValidateAntiforgeryToken]
    public IActionResult Edit(CoverType obj)
    {
        //ModelState.IsValid wil check if obj is a proper instance of category class.
        //That means all the constraints in the class should be there.So, name can't be
        //null and displayorder should be integer.
        if (ModelState.IsValid)
        {

            //_db.Categories.Update(obj);
            _db.CoverType.Update(obj);
            //Actually adds the new category into table
            //_db.SaveChanges();
            _db.Save();
            //Tempdata stays only for one redirect.On reload it disappears
            TempData["Success"] = "CoverType Edited successfully";
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
        var coverTypeFromDbFirst = _db.CoverType.GetFirstorDefault(u => u.Id == id);

        if (coverTypeFromDbFirst == null)
        {
            return NotFound();
        }
        return View(coverTypeFromDbFirst);
    }

    //Post method. It will delete the category from database table.  
    [HttpPost]
    [AutoValidateAntiforgeryToken]
    public IActionResult DeletePost(int? id)
    {
        //var category = _db.Categories.Find(id);
        var coverType = _db.CoverType.GetFirstorDefault(u => u.Id == id);

        if (coverType == null)
        {
            return NotFound();
        }

        //_db.Categories.Remove(category);
        _db.CoverType.Remove(coverType);
        //_db.SaveChanges();
        _db.Save();
        //Tempdata stays only for one redirect.On reload it disappears
        TempData["Success"] = "CoverType deleted successfully";
        //Redirecting to Index Action
        return RedirectToAction(nameof(Index));



    }
}
