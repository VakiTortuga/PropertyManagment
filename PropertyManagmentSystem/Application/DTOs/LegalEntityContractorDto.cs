using PropertyManagmentSystem.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagmentSystem.Application.DTOs
{
    public class LegalEntityContractorDto : ContractorDto
    {
        public string CompanyName { get; set; }
        public string DirectorName { get; set; }
        public string LegalAddress { get; set; }
        public string TaxId { get; set; }
        public BankDetailsDto BankDetails { get; set; }
    }
}
