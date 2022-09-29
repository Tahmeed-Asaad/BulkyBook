using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBookWeb.DataAccess;

namespace BulkyBook.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product obj)
        {

            //_db.Products.Update(obj);

            var objFromDb = _db.Products.FirstOrDefault(u => u.Id == obj.Id);

            if(objFromDb != null)
            {
                objFromDb.Title=obj.Title;
                objFromDb.Description=obj.Description;
                objFromDb.ISBN=obj.ISBN;
                objFromDb.ListPrice=obj.ListPrice;
                objFromDb.Price=obj.Price;
                objFromDb.Price100=obj.Price100;
                objFromDb.Price50=obj.Price50;
                objFromDb.Author=obj.Author;
                objFromDb.CategoryId=obj.CategoryId;
                objFromDb.CoverTypeID=obj.CoverTypeID;

                if (obj.ImageURL != null)
                {
                    objFromDb.ImageURL=obj.ImageURL;
                }

            }
            }


    }
}
