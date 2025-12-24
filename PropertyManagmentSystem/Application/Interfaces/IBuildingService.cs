using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyManagmentSystem.Application.DTOs;

namespace PropertyManagmentSystem.Application.Interfaces
{
    public interface IBuildingService
    {
        // Здания
        BuildingDto GetBuildingById(int id);
        IEnumerable<BuildingDto> GetAllBuildings();
        void AddBuilding(BuildingDto buildingDto);
        void UpdateBuilding(BuildingDto buildingDto);
        void DeleteBuilding(int id);

        // Комнаты
        RoomDto GetRoomById(int id);
        IEnumerable<RoomDto> GetRoomsByBuildingId(int buildingId);
        void AddRoomToBuilding(int buildingId, RoomDto roomDto);
        void UpdateRoom(RoomDto roomDto);
        void RemoveRoomFromBuilding(int roomId);

        // Отчетность по зданиям
        IEnumerable<RoomDto> GetAvailableRooms();
        IEnumerable<RoomDto> GetRentedRooms();
        decimal GetBuildingOccupancyRate(int buildingId);
        decimal GetTotalRentedArea(int buildingId);
    }
}
