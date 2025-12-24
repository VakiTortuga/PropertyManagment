using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagmentSystem.Infrastructure.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(int id);

        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);

        int? Add(T entity);
        bool Update(T entity);
        bool Delete(int id);

        bool Exists(int id);
    }
}
