using PropertyManagmentSystem.Domains;
using PropertyManagmentSystem.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagmentSystem.Infrastructure.Repositories
{
    public class IndividualContractorRepository : JsonRepositoryBase<IndividualContractor>, IIndividualContractorRepository
    {
        public IndividualContractorRepository() : base("individual_contractors.json")
        {
        }

        // Специфичные методы для физических лиц
        public IEnumerable<IndividualContractor> GetByFullName(string fullName)
        {
            lock (_lock)
            {
                return _items
                    .Where(c => c.FullName.Contains(fullName))
                    .ToList();
            }
        }

        public IEnumerable<IndividualContractor> GetActiveContractors()
        {
            lock (_lock)
            {
                return _items.Where(c => c.IsActive).ToList();
            }
        }

        public IndividualContractor GetByPhone(string phone)
        {
            lock (_lock)
            {
                return _items.FirstOrDefault(c => c.Phone == phone);
            }
        }

        // Переопределяем Add для дополнительной валидации
        public new bool Add(IndividualContractor entity)
        {
            lock (_lock)
            {
                // Проверяем уникальность телефона (опционально)
                if (GetByPhone(entity.Phone) != null)
                {
                    return false; // Телефон уже занят
                }

                return base.Add(entity);
            }
        }
    }
}
