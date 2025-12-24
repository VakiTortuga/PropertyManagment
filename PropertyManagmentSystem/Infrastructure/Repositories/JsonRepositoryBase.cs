using Newtonsoft.Json;
using PropertyManagmentSystem.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagmentSystem.Infrastructure.Repositories
{
    public abstract class JsonRepositoryBase<T> : IRepository<T> where T : class
    {
        protected readonly string _filePath;
        protected List<T> _items;
        protected readonly object _lock = new object();

        protected JsonRepositoryBase(string fileName)
        {
            // Создаем папку для данных
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = Path.Combine(appDataPath, "PropertyManagementSystem");

            Directory.CreateDirectory(appFolder);
            _filePath = Path.Combine(appFolder, fileName);

            LoadData();
        }

        protected virtual void LoadData()
        {
            lock (_lock)
            {
                if (File.Exists(_filePath))
                {
                    try
                    {
                        var json = File.ReadAllText(_filePath);
                        _items = JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка загрузки данных из {_filePath}: {ex.Message}");
                        _items = new List<T>();
                    }
                }
                else
                {
                    _items = new List<T>();
                }
            }
        }

        protected virtual void SaveData()
        {
            lock (_lock)
            {
                try
                {
                    var json = JsonConvert.SerializeObject(_items, Formatting.Indented);
                    File.WriteAllText(_filePath, json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка сохранения в {_filePath}: {ex.Message}");
                }
            }
        }

        // Получение ID из сущности (через reflection)
        protected virtual int GetId(T entity)
        {
            var prop = typeof(T).GetProperty("Id");
            if (prop == null)
                throw new InvalidOperationException($"Тип {typeof(T).Name} не имеет свойства Id");

            return (int)prop.GetValue(entity);
        }

        public IEnumerable<T> GetAll()
        {
            lock (_lock)
            {
                return _items.ToList();
            }
        }

        public T GetById(int id)
        {
            lock (_lock)
            {
                return _items.FirstOrDefault(item => GetId(item) == id);
            }
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            lock (_lock)
            {
                return _items.Where(predicate.Compile()).ToList();
            }
        }

        public bool Add(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            lock (_lock)
            {
                var id = GetId(entity);

                // Проверяем уникальность ID
                if (Exists(id))
                    return false; // ID уже занят

                _items.Add(entity);
                SaveData();
                return true;
            }
        }

        public bool Update(T entity)
        {
            if (entity == null)
                return false;

            lock (_lock)
            {
                var id = GetId(entity);
                var existing = GetById(id);

                if (existing == null)
                    return false; // Сущность не найдена

                var index = _items.IndexOf(existing);
                _items[index] = entity;
                SaveData();
                return true;
            }
        }

        public bool Delete(int id)
        {
            lock (_lock)
            {
                var entity = GetById(id);
                if (entity == null)
                    return false;

                var removed = _items.Remove(entity);
                if (removed)
                    SaveData();

                return removed;
            }
        }

        public bool Exists(int id)
        {
            lock (_lock)
            {
                return GetById(id) != null;
            }
        }

        public int GetNextAvailableId()
        {
            lock (_lock)
            {
                if (!_items.Any())
                    return 1;

                // Находим максимальный ID и возвращаем следующий
                var maxId = _items.Max(item => GetId(item));
                return maxId + 1;
            }
        }

        public bool IsIdAvailable(int id)
        {
            lock (_lock)
            {
                return !Exists(id);
            }
        }
    }
}

