using System.Linq.Expressions;

namespace MeetApp.Infrastructure.Commons.Abstracts
{
    public interface IRepository<T>
        where T : class
    {

        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate = null, bool tracking = false);
        Task<T> GetAsync(Expression<Func<T, bool>> predicate = null);
        Task<T> AddAsync(T model);
        Task<T> EditAsync(T model);
        Task RemoveAsync(T model);
        Task<int> SaveAsync(CancellationToken cancellationToken = default);

    }
}
