using PropertyManagmentSystem.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagmentSystem.Application.DTOs
{
    public class AgreementDto
    {
        public int Id { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public PaymentFrequency PaymentFrequency { get; set; }

        public int ContractorId { get; set; }
        public AgreementStatus Status { get; set; }

        public DateTime? SignedDate { get; set; }
        public DateTime? CancellationDate { get; set; }
        public string CancellationReason { get; set; }

        public decimal PenaltyRate { get; set; }
        public decimal TotalMonthlyRent { get; set; }

        public IReadOnlyCollection<RentedItemDto> RentedItems { get; set; }
    }

}
