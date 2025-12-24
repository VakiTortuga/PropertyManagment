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
        
        // Добавление с проверкой уникальности ID
        bool Add(T entity);
        
        // Обновление существующей сущности
        bool Update(T entity);
        
        // Удаление по ID
        bool Delete(int id);
        
        // Проверка существования
        bool Exists(int id);
        
        // Получение следующего доступного ID
        int GetNextAvailableId();
        
        // Проверка, доступен ли ID для использования
        bool IsIdAvailable(int id);
    }
}
