using PropertyManagmentSystem.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagmentSystem.Infrastructure.Repositories
{
    public class RoomRepository : JsonRepositoryBase<Room>
    {
        public RoomRepository() : base("rooms.json")
        {
        }

        // Дополнительные методы для работы с комнатами
        public IEnumerable<Room> GetRoomsByBuildingId(int buildingId)
        {
            lock (_lock)
            {
                return _items.Where(r => r.BuildingId == buildingId).ToList();
            }
        }

        public IEnumerable<Room> GetAvailableRooms()
        {
            lock (_lock)
            {
                return _items.Where(r => !r.IsRented).ToList();
            }
        }

        public IEnumerable<Room> GetRentedRooms()
        {
            lock (_lock)
            {
                return _items.Where(r => r.IsRented).ToList();
            }
        }

        public Room GetRoomByNumber(string roomNumber)
        {
            lock (_lock)
            {
                return _items.FirstOrDefault(r => r.RoomNumber == roomNumber);
            }
        }

        public IEnumerable<Room> GetRoomsOnFloor(int floorNumber)
        {
            lock (_lock)
            {
                return _items.Where(r => r.FloorNumber == floorNumber).ToList();
            }
        }
    }
}
