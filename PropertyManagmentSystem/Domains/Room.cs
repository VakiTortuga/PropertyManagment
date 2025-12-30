using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyManagmentSystem.Enums;

namespace PropertyManagmentSystem.Domains
{
    public class Room
    {
        // Поля
        private int _id;
        private string _roomNumber;
        private decimal _area;
        private int _floorNumber;
        private FinishingType _finishingType;
        private bool _hasPhone;
        private int? _currentAgreementId;
        private int _buildingId;

        // Свойства
        public int Id { get; }
        public string RoomNumber { get; }
        public decimal Area { get; }
        public int FloorNumber { get; }
        public FinishingType FinishingType { get; private set; }
        public bool HasPhone { get; private set; }
        public bool IsRented { get; }
        public int? CurrentAgreementId { get; private set; }
        public int BuildingId => _buildingId;

        public Room(int id, string roomNumber, decimal area, int floorNumber,
                    FinishingType finishingType, bool hasPhone)
        {
            Validate(roomNumber, area, floorNumber, finishingType);

            Id = id;
            RoomNumber = roomNumber;
            Area = area;
            FloorNumber = floorNumber;
            FinishingType = finishingType;
            HasPhone = hasPhone;
            IsRented = false;
        }

        private void Validate(string roomNumber, decimal area, int floorNumber, FinishingType finishingType)
        {
            if (string.IsNullOrWhiteSpace(roomNumber))
                throw new ArgumentException("Номер комнаты обязателен");
            if (area <= 0)
                throw new ArgumentException("Площадь должна быть положительной");
            if (floorNumber <= 0)
                throw new ArgumentException("Этаж должен быть положительным");
        }

        // Внутренний метод для установки Building (вызывается из Building.AddRoom)
        internal void SetBuilding(Building building)
        {
            if (building == null)
                throw new ArgumentNullException(nameof(building));

            _buildingId = building.Id;
        }

        // Бизнес-методы
        public void Rent(int agreementId)
        {
            if (IsRented)
                throw new InvalidOperationException("Комната уже арендована");

            CurrentAgreementId = agreementId;
        }

        public void Release()
        {
            CurrentAgreementId = null;
        }

        public void UpdateFinishing(FinishingType newFinishing)
        {
            FinishingType = newFinishing;
        }

        public void InstallPhone()
        {
            if (HasPhone)
                throw new InvalidOperationException("Телефон уже установлен");

            HasPhone = true;
        }

        public void RemovePhone()
        {
            if (!HasPhone)
                throw new InvalidOperationException("Телефон не установлен");

            HasPhone = false;
        }

        public bool CanBeRented() => !IsRented;
    }

}
