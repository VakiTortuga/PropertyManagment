using PropertyManagmentSystem.Domains;
using PropertyManagmentSystem.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagmentSystem.Application.DTOs
{
    public class IndividualContractorDto : ContractorDto
    {
        public string FullName { get; set; }
        public PassportDto Passport { get; set; }
    }
}
