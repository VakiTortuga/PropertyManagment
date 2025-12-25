using PropertyManagmentSystem.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagmentSystem.Infrastructure.Interfaces
{
    public interface ILegalEntityContractorRepository : IContractorRepository<LegalEntityContractor>
    {
        IEnumerable<LegalEntityContractor> GetByCompanyName(string companyName);
        IEnumerable<LegalEntityContractor> GetByTaxId(string taxId);
    }
}
