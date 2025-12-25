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
        public PassportDataDto Passport { get; set; }

        public string PassportSeries => Passport?.Series;
        public string PassportNumber => Passport?.Number;
        public string PassportIssuedBy => Passport?.IssuedBy;
        public string PassportIssueDate => Passport?.IssueDate.ToString("dd.MM.yyyy");

        public IndividualContractorDto()
        {
            Type = ContractorType.Individual;
        }
    }
}
