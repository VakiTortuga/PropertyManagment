using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PropertyManagmentSystem.Domains
{
    public class Building
    {
        // СВОЙСТВА
        public int Id { get; private set; }
        public string District { get; private set; }
        public string Address { get; private set; }
        public int FloorsCount { get; private set; }
        public string CommandantPhone { get; private set; }

        // Комнаты
        private List<Room> _rooms = new List<Room>();
        public IReadOnlyCollection<Room> Rooms => _rooms.AsReadOnly();

        // ВЫЧИСЛЯЕМЫЕ СВОЙСТВА
        public int TotalRooms => _rooms.Count;
        public int AvailableRooms => _rooms.Count(r => !r.IsRented);
        public bool HasAvailableRooms => AvailableRooms > 0;

        // КОНСТРУКТОР для JSON десериализации
        [JsonConstructor]
        public Building(int id, string district, string address, int floorsCount, string phone)
        {
            // При загрузке из JSON не выполняем жесткую валидацию
            // Только базовая проверка на null
            Id = id;
            District = district ?? string.Empty;
            Address = address ?? string.Empty;
            FloorsCount = floorsCount;
            CommandantPhone = phone ?? string.Empty;
        }

        // СТАТИЧЕСКИЙ МЕТОД для создания нового здания (с полной валидацией)
        public static Building Create(int id, string district, string address, int floorsCount, string phone)
        {
            Validate(district, address, floorsCount, phone);
            return new Building(id, district, address, floorsCount, phone);
        }

        private static void Validate(string district, string address, int floorsCount, string phone)
        {
            if (string.IsNullOrWhiteSpace(district))
                throw new ArgumentException("Район обязателен");
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Адрес обязателен");
            if (floorsCount <= 0)
                throw new ArgumentException("Число этажей должно быть положительным");
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Телефон коменданта обязателен");
        }

        // МЕТОДЫ ДЛЯ КОМНАТ

        public void AddRoom(Room room)
        {
            if (room == null)
                throw new ArgumentNullException(nameof(room));

            // этаж комнаты не может превышать этажность здания
            if (room.FloorNumber > FloorsCount)
                throw new InvalidOperationException(
                    $"Этаж комнаты ({room.FloorNumber}) превышает этажность здания ({FloorsCount})");

            // номер комнаты должен быть уникальным в здании
            if (_rooms.Any(r => r.RoomNumber == room.RoomNumber))
                throw new InvalidOperationException(
                    $"Комната с номером '{room.RoomNumber}' уже существует в здании");

            room.SetBuilding(this); // Устанавливаем связь
            _rooms.Add(room);
        }

        public void RemoveRoom(int roomId)
        {
            var room = _rooms.FirstOrDefault(r => r.Id == roomId);
            if (room == null)
                throw new ArgumentException($"Комната с ID {roomId} не найдена");

            // нельзя удалить арендованную комнату
            if (room.IsRented)
                throw new InvalidOperationException(
                    $"Нельзя удалить комнату '{room.RoomNumber}', так как она арендована");

            _rooms.Remove(room);
        }

        public Room GetRoom(int roomId) => _rooms.FirstOrDefault(r => r.Id == roomId);

        public Room GetRoomByNumber(string roomNumber) =>
            _rooms.FirstOrDefault(r => r.RoomNumber == roomNumber);

        // МЕТОДЫ ДЛЯ ИЗМЕНЕНИЯ ЗДАНИЯ

        public void ChangeDistrict(string newDistrict)
        {
            if (string.IsNullOrWhiteSpace(newDistrict))
                throw new ArgumentException("Район не может быть пустым");

            District = newDistrict;
        }

        public void ChangeCommandantPhone(string newPhone)
        {
            if (string.IsNullOrWhiteSpace(newPhone))
                throw new ArgumentException("Телефон не может быть пустым");

            // Можно добавить проверку формата телефона
            if (!IsValidPhone(newPhone))
                throw new ArgumentException("Неверный формат телефона");

            CommandantPhone = newPhone;
        }

        private bool IsValidPhone(string phone)
        {
            return phone.Any(char.IsDigit) && phone.Length >= 5;
        }

        // БИЗНЕС-МЕТОДЫ

        public List<Room> GetAvailableRooms() =>
            _rooms.Where(r => !r.IsRented).ToList();

        public List<Room> GetRoomsOnFloor(int floorNumber)
        {
            if (floorNumber <= 0 || floorNumber > FloorsCount)
                throw new ArgumentException($"Неверный номер этажа. Допустимо: 1-{FloorsCount}");

            return _rooms.Where(r => r.FloorNumber == floorNumber).ToList();
        }

        public decimal GetTotalRentedArea() =>
            _rooms.Where(r => r.IsRented).Sum(r => r.Area);

        public decimal GetTotalArea() => _rooms.Sum(r => r.Area);

        public decimal GetOccupancyRate() =>
            TotalRooms > 0 ? (decimal)_rooms.Count(r => r.IsRented) / TotalRooms : 0;

        // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ

        public bool ContainsRoom(int roomId) => _rooms.Any(r => r.Id == roomId);

        public bool ContainsRoomWithNumber(string roomNumber) =>
            _rooms.Any(r => r.RoomNumber == roomNumber);
    }
}
