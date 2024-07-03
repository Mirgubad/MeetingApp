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


        public T Add(T model)
        {
            _table.Add(model);
            Save();
            return model;
        }

        public T Edit(T model)
        {
            _table.Entry(model).State = EntityState.Modified;
            Save();
            return model;
        }

        public T Get(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return _table.FirstOrDefault();
            }

            var model = _table.FirstOrDefault(predicate);

            return model;
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate = null)
        {
            var query = _table.AsQueryable();
            if (true)
            {
                query = query.AsNoTracking();
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return query;
        }

        public void Remove(T model)
        {
            _db.Remove(model);
            Save();
        }

        public int Save()
        {
            return _db.SaveChanges();
        }
    }
}
