using PropertyManagmentSystem.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagmentSystem.Infrastructure.Repositories
{
    public class BuildingRepository : JsonRepositoryBase<Building>
    {
        public BuildingRepository() : base("buildings.json")
        {
        }

        // Дополнительные методы для работы со зданиями
        public IEnumerable<Building> GetBuildingsWithAvailableRooms()
        {
            lock (_lock)
            {
                return _items.Where(b => b.HasAvailableRooms).ToList();
            }
        }

        public Building GetBuildingByAddress(string address)
        {
            lock (_lock)
            {
                return _items.FirstOrDefault(b => b.Address == address);
            }
        }

        public IEnumerable<Building> GetBuildingsInDistrict(string district)
        {
            lock (_lock)
            {
                return _items.Where(b => b.District == district).ToList();
            }
        }
    }
}
