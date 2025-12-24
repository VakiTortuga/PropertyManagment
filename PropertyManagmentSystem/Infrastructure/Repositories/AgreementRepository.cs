using PropertyManagmentSystem.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagmentSystem.Infrastructure.Repositories
{
    public class AgreementRepository : JsonRepositoryBase<Agreement>
    {
        public AgreementRepository() : base("agreements.json")
        {
        }

        // Дополнительные методы для работы с договорами
        public IEnumerable<Agreement> GetActiveAgreements()
        {
            lock (_lock)
            {
                return _items.Where(a => a.Status == Enums.AgreementStatus.Active).ToList();
            }
        }

        public IEnumerable<Agreement> GetDraftAgreements()
        {
            lock (_lock)
            {
                return _items.Where(a => a.Status == Enums.AgreementStatus.Draft).ToList();
            }
        }

        public IEnumerable<Agreement> GetAgreementsByContractorId(int contractorId)
        {
            lock (_lock)
            {
                return _items.Where(a => a.ContractorId == contractorId).ToList();
            }
        }

        public IEnumerable<Agreement> GetExpiringAgreements(DateTime withinDays)
        {
            lock (_lock)
            {
                return _items.Where(a =>
                    a.Status == Enums.AgreementStatus.Active &&
                    a.EndDate <= withinDays &&
                    a.EndDate >= DateTime.Today).ToList();
            }
        }

        public Agreement GetAgreementByRegistrationNumber(string registrationNumber)
        {
            lock (_lock)
            {
                return _items.FirstOrDefault(a => a.RegistrationNumber == registrationNumber);
            }
        }

        // Переопределяем Add для проверки уникальности RegistrationNumber
        public new bool Add(Agreement entity)
        {
            lock (_lock)
            {
                // Проверяем уникальность регистрационного номера
                if (_items.Any(a => a.RegistrationNumber == entity.RegistrationNumber))
                {
                    return false; // Номер уже существует
                }

                return base.Add(entity);
            }
        }
    }
}
