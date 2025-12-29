using PropertyManagmentSystem.Application.DTOs;
using PropertyManagmentSystem.Application.Interfaces;
using PropertyManagmentSystem.Application.Requests;
using PropertyManagmentSystem.Domains;
using PropertyManagmentSystem.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagmentSystem.Application.Services
{
    public class BuildingService : IBuildingService
    {
        private readonly BuildingRepository _buildingRepo;
        private readonly RoomRepository _roomRepo;

        public BuildingService(BuildingRepository buildingRepo, RoomRepository roomRepo)
        {
            _buildingRepo = buildingRepo;
            _roomRepo = roomRepo;
        }

        public BuildingDto GetBuildingById(int id)
        {
            var b = _buildingRepo.GetById(id);
            if (b == null) return null;
            
            // Загружаем комнаты для здания
            LoadRoomsForBuilding(b);
            return MapBuilding(b);
        }

        public IEnumerable<BuildingDto> GetAllBuildings()
        {
            var buildings = _buildingRepo.GetAll();
            
            // Загружаем комнаты для каждого здания
            foreach (var building in buildings)
            {
                LoadRoomsForBuilding(building);
            }
            
            return buildings.Select(MapBuilding);
        }

        // ВСПОМОГАТЕЛЬНЫЙ МЕТОД для загрузки комнат в здание
        private void LoadRoomsForBuilding(Building building)
        {
            var rooms = _roomRepo.GetRoomsByBuildingId(building.Id).ToList();
            foreach (var room in rooms)
            {
                try
                {
                    building.AddRoom(room);
                }
                catch
                {
                    // Комната уже добавлена или не подходит - пропускаем
                }
            }
        }

        public void AddBuilding(CreateBuildingRequest request)
        {
            var building = Building.Create(
                _buildingRepo.GetNextAvailableId(),
                request.District,
                request.Address,
                request.FloorsCount,
                request.CommandantPhone);

            _buildingRepo.Add(building);
        }

        public void UpdateBuilding(UpdateBuildingRequest request)
        {
            var building = _buildingRepo.GetById(request.BuildingId);
            building.ChangeDistrict(request.District);
            building.ChangeCommandantPhone(request.CommandantPhone);
            _buildingRepo.Update(building);
        }

        public void DeleteBuilding(int id)
            => _buildingRepo.Delete(id);

        public RoomDto GetRoomById(int id)
            => MapRoom(_roomRepo.GetById(id));

        public IEnumerable<RoomDto> GetRoomsByBuildingId(int buildingId)
            => _roomRepo.GetRoomsByBuildingId(buildingId).Select(MapRoom);

        public void AddRoomToBuilding(int buildingId, AddRoomRequest request)
        {
            // Получаем здание и ЗАГРУЖАЕМ его комнаты из репозитория
            var building = _buildingRepo.GetById(buildingId);
            if (building == null)
                throw new ArgumentException($"Здание с ID {buildingId} не найдено");
            
            // Предварительно загружаем все комнаты этого здания
            LoadRoomsForBuilding(building);

            // Создаем новую комнату
            var room = Room.Create(
                _roomRepo.GetNextAvailableId(),
                request.RoomNumber,
                request.Area,
                request.FloorNumber,
                request.FinishingType,
                request.HasPhone);

            // Добавляем комнату в здание (для валидации - проверяет уникальность номера и этажность)
            // И устанавливаем BuildingId
            building.AddRoom(room);

            // Сохраняем комнату в репозитории (теперь с правильным BuildingId)
            _roomRepo.Add(room);
            
            // Обновляем здание в репозитории (для консистентности)
            _buildingRepo.Update(building);
        }

        public void UpdateRoom(UpdateRoomRequest request)
        {
            var room = _roomRepo.GetById(request.RoomId);
            room.UpdateFinishing(request.FinishingType);
            _roomRepo.Update(room);
        }

        public void RemoveRoomFromBuilding(int roomId)
        {
            var room = _roomRepo.GetById(roomId);
            if (room != null)
            {
                var building = _buildingRepo.GetById(room.BuildingId);
                if (building != null)
                {
                    building.RemoveRoom(roomId);
                    _buildingRepo.Update(building);
                }
            }
            
            _roomRepo.Delete(roomId);
        }

        public IEnumerable<RoomDto> GetAvailableRooms()
            => _roomRepo.GetAvailableRooms().Select(MapRoom);

        public IEnumerable<RoomDto> GetRentedRooms()
            => _roomRepo.GetRentedRooms().Select(MapRoom);

        public decimal GetBuildingOccupancyRate(int buildingId)
        {
            var building = _buildingRepo.GetById(buildingId);
            if (building == null) return 0;
            
            LoadRoomsForBuilding(building);
            return building.GetOccupancyRate();
        }

        public decimal GetTotalRentedArea(int buildingId)
        {
            var building = _buildingRepo.GetById(buildingId);
            if (building == null) return 0;
            
            LoadRoomsForBuilding(building);
            return building.GetTotalRentedArea();
        }

        private BuildingDto MapBuilding(Building b) => new BuildingDto
        {
            Id = b.Id,
            District = b.District,
            Address = b.Address,
            FloorsCount = b.FloorsCount,
            CommandantPhone = b.CommandantPhone,
            TotalRooms = b.TotalRooms,
            AvailableRooms = b.AvailableRooms,
            OccupancyRate = b.GetOccupancyRate()
        };

        private RoomDto MapRoom(Room r) => new RoomDto
        {
            Id = r.Id,
            BuildingId = r.BuildingId,
            RoomNumber = r.RoomNumber,
            Area = r.Area,
            FloorNumber = r.FloorNumber,
            FinishingType = r.FinishingType,
            HasPhone = r.HasPhone,
            IsRented = r.IsRented,
            CurrentAgreementId = r.CurrentAgreementId
        };
    }

}
