using PropertyManagmentSystem.Domains;
using PropertyManagmentSystem.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagmentSystem.Infrastructure.Repositories
{
    public class LegalEntityContractorRepository : JsonRepositoryBase<LegalEntityContractor>, ILegalEntityContractorRepository
    {
        public LegalEntityContractorRepository() : base("legal_entity_contractors.json")
        {
        }

        // Специфичные методы для юридических лиц
        public IEnumerable<LegalEntityContractor> GetByCompanyName(string companyName)
        {
            lock (_lock)
            {
                return _items
                    .Where(c => c.CompanyName.Contains(companyName))
                    .ToList();
            }
        }

        public IEnumerable<LegalEntityContractor> GetByTaxId(string taxId)
        {
            lock (_lock)
            {
                return _items
                    .Where(c => c.TaxId == taxId)
                    .ToList();
            }
        }

        public IEnumerable<LegalEntityContractor> GetActiveContractors()
        {
            lock (_lock)
            {
                return _items.Where(c => c.IsActive).ToList();
            }
        }

        public LegalEntityContractor GetByPhone(string phone)
        {
            lock (_lock)
            {
                return _items.FirstOrDefault(c => c.Phone == phone);
            }
        }

        // Переопределяем Add для дополнительной валидации
        public new bool Add(LegalEntityContractor entity)
        {
            lock (_lock)
            {
                // Проверяем уникальность телефона
                if (GetByPhone(entity.Phone) != null)
                {
                    return false; // Телефон уже занят
                }

                // Проверяем уникальность ИНН
                if (GetByTaxId(entity.TaxId).Any())
                {
                    return false; // ИНН уже занят
                }

                return base.Add(entity);
            }
        }
    }
}
