using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        //Declaring methods from Category Controller Class
        T GetFirstorDefault(Expression<Func<T, bool>> filter);
        IEnumerable<T> GetAll();
        void Add(T entity);
        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entities);

        //Update is not declared here because for different models the update algorithm logic will be different

    }
}
