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
        public string RoomNumber { get; set; }
        public decimal Area { get; set; }
        public int FloorNumber { get; set; }
        public FinishingType FinishingType { get; set; }
        public bool HasPhone { get; set; }
        public bool IsRented { get; set; }
        public int? CurrentAgreementId { get; set; }

        // Связи
        public int BuildingId { get; set; }
        public string BuildingAddress { get; set; }

        // Статус
        public bool CanBeRented => !IsRented;
        public string Status => IsRented ? "Арендована" : "Свободна";

        // Для отображения
        public string DisplayInfo => $"Комната {RoomNumber}, {Area} м², {FloorNumber} этаж";
        public string FullInfo => $"{DisplayInfo}, отделка: {FinishingType}, телефон: {(HasPhone ? "есть" : "нет")}";

        // Если арендована
        public string CurrentContractorName { get; set; }
        public decimal? CurrentRentAmount { get; set; }
    }
}
