using System.Linq.Expressions;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        //Declaring methods from Category Controller Class
        T GetFirstorDefault(Expression<Func<T, bool>> filter, string? includeProperties=null);
        //includeproperties is being used to display category name from categoryId which is a foregn key in product model class.
        //Also as we are using DataTable instead of traditional list we need to do this.
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter=null, string? includeProperties = null);
        void Add(T entity);
        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entities);

        //Update is not declared here because for different models the update algorithm logic will be different

    }
}
