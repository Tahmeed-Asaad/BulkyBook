using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBookWeb.DataAccess;

namespace BulkyBook.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository

    {
        private readonly ApplicationDbContext _db;

        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        //Save method is implemented inside unit of work. It was here before
       /* void IApplicationUserRepository.Update(ApplicationUser obj)
        {
            _db.ApplicationUsers.Update(obj);
        }*/
    }
}
