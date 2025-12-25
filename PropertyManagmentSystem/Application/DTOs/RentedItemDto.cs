using PropertyManagmentSystem.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagmentSystem.Application.DTOs
{
    public class RentedItemDto
    {
        public int RoomId { get; set; }
        public RoomPurpose Purpose { get; set; }
        public DateTime RentUntil { get; set; }
        public decimal RentAmount { get; set; }

        public bool IsEarlyTerminated { get; set; }
        public DateTime? ActualVacationDate { get; set; }
        public string EarlyTerminationReason { get; set; }
    }
}
