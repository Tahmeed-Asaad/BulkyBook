using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBookWeb.DataAccess;

namespace BulkyBook.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository

    {
        private readonly ApplicationDbContext _db;

        public ShoppingCartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public int DecrementCount(ShoppingCart shoppingCart, int count)
        {
            shoppingCart.Count= shoppingCart.Count- count;

            return shoppingCart.Count;
        }

        public int IncrementCount(ShoppingCart shoppingCart, int count)
        {
            shoppingCart.Count = shoppingCart.Count + count;
            return shoppingCart.Count;
        }

        //Save method is implemented inside unit of work. It was here before
        /* void IShoppingCartRepository.Update(ShoppingCart obj)
         {
             _db.ShoppingCarts.Update(obj);
         }*/
    }
}
