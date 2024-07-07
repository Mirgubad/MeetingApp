using MeetApp.Infrastructure.Commons.Abstracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MeetApp.Infrastructure.Commons.Concretes
{
    public abstract class Repository<T> : IRepository<T>
        where T : class
    {
        private readonly DbContext _db;
        private readonly DbSet<T> _table;

        protected Repository(DbContext db)
        {
            _db = db;
            _table = _db.Set<T>();
        }


        public async Task<T> AddAsync(T model)
        {
            await _table.AddAsync(model);
            await SaveAsync();
            return model;
        }

        public async Task<T> EditAsync(T model)
        {
            _table.Entry(model).State = EntityState.Modified;
            await SaveAsync();
            return model;
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate is null)
                return await _table.FirstOrDefaultAsync();

            var data = await _table.FirstOrDefaultAsync(predicate);

            //if (data == null)
            //{
            //    throw new NotFoundException("RECORD_NOT_FOUND");
            //}
            return data;


        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate = null, bool tracking = true)
        {
            var query = _table.AsQueryable();

            if (!tracking)
                query = query.AsNoTracking();

            if (predicate is not null)
                query = query.Where(predicate);

            return query;
        }

        public async Task RemoveAsync(T model)
        {
            _db.Remove(model);
            await SaveAsync();
        }

        public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
        {
            return await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
