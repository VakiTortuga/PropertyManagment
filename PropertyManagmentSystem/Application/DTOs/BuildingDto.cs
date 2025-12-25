using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagmentSystem.Application.DTOs
{
    public class BuildingDto
    {
        public int Id { get; set; }
        public string District { get; set; }
        public string Address { get; set; }
        public int FloorsCount { get; set; }
        public string CommandantPhone { get; set; }

        public int TotalRooms { get; set; }
        public int AvailableRooms { get; set; }
        public decimal OccupancyRate { get; set; }
    }
}
