using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyManagmentSystem.Enums;
using Newtonsoft.Json;

namespace PropertyManagmentSystem.Domains
{
    public class Room
    {
        public int Id { get; private set; }
        public string RoomNumber { get; private set; }
        public decimal Area { get; private set; }
        public int FloorNumber { get; private set; }
        public FinishingType FinishingType { get; private set; }
        public bool HasPhone { get; private set; }
        public bool IsRented { get; }
        public int? CurrentAgreementId { get; private set; }

        // Ссылка на Building (для навигации) - теперь это свойство которое может быть сохранено в JSON
        public int BuildingId { get; private set; }

        // КОНСТРУКТОР для JSON десериализации
        [JsonConstructor]
        public Room(int id, string roomNumber, decimal area, int floorNumber,
                    FinishingType finishingType, bool hasPhone, int buildingId = 0)
        {
            // При загрузке из JSON минимальная проверка
            Id = id;
            RoomNumber = roomNumber ?? string.Empty;
            Area = area;
            FloorNumber = floorNumber;
            FinishingType = finishingType;
            HasPhone = hasPhone;
            IsRented = false;
            BuildingId = buildingId;
        }

        // СТАТИЧЕСКИЙ МЕТОД для создания нового помещения (с полной валидацией)
        public static Room Create(int id, string roomNumber, decimal area, int floorNumber,
                                  FinishingType finishingType, bool hasPhone)
        {
            Validate(roomNumber, area, floorNumber, finishingType);
            return new Room(id, roomNumber, area, floorNumber, finishingType, hasPhone, 0);
        }

        private static void Validate(string roomNumber, decimal area, int floorNumber, FinishingType finishingType)
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

            BuildingId = building.Id;
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
