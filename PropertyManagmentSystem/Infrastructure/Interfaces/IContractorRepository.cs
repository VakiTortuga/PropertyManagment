using PropertyManagmentSystem.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagmentSystem.Infrastructure.Interfaces
{
    // Общий интерфейс для всех репозиториев контрагентов
    public interface IContractorRepository<T> : IRepository<T> where T : Contractor
    {
        IEnumerable<T> GetActiveContractors();
        T GetByPhone(string phone);
    }
}
