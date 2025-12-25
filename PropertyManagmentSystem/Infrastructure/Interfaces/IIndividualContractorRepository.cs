using PropertyManagmentSystem.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagmentSystem.Infrastructure.Interfaces
{
    public interface IIndividualContractorRepository : IContractorRepository<IndividualContractor>
    {
        IEnumerable<IndividualContractor> GetByFullName(string fullName);
    }
}
