using PropertyManagmentSystem.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagmentSystem.Application.DTOs
{
    public class ContractorDto
    {
        public int Id { get; set; }
        public ContractorType Type { get; set; }
        public string Phone { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; }

        public List<int> AgreementIds { get; set; } = new List<int>();

        public string DisplayName { get; set; }
        public string ContactInfo { get; set; }

        public int TotalAgreements { get; set; }
        public int ActiveAgreements { get; set; }
        public bool CanCreateNewAgreement { get; set; }
    }
}
