using PropertyManagmentSystem.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagmentSystem.Application.DTOs
{
    public class RoomDto
    {
        public int Id { get; set; }
        public int BuildingId { get; set; }
        public string RoomNumber { get; set; }
        public decimal Area { get; set; }
        public int FloorNumber { get; set; }
        public FinishingType FinishingType { get; set; }
        public bool HasPhone { get; set; }
        public bool IsRented { get; set; }
        public int? CurrentAgreementId { get; set; }
    }
}
