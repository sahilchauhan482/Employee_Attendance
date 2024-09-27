using System.Linq.Expressions;

namespace EmployeeAPI.Repository.IRepository
{
    public interface IRepository<T> where T : class

    {
        Task<T> GetById(int id);

        Task< IEnumerable<T>> GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null,
            string IncludeProperties = null, int? take = null);
        Task Add(T entity);
        void Delete(T entity);
        void Update(T entity);

        //Task<T> FirstOrDefault(Expression<Func<T, bool>> filter = null, string IncludeProperties = null);
        Task <T> FirstOrDefault(
            Expression<Func<T, bool>> filter = null,
            string IncludeProperties = null
            );
    }
}
