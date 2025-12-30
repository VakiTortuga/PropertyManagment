using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyManagmentSystem.Application.DTOs;
using PropertyManagmentSystem.Application.Requests;

namespace PropertyManagmentSystem.Application.Interfaces
{
    public interface IBuildingService
    {
        // Здания
        BuildingDto GetBuildingById(int id);
        IEnumerable<BuildingDto> GetAllBuildings();
        void AddBuilding(CreateBuildingRequest request);
        void UpdateBuilding(UpdateBuildingRequest request);
        void DeleteBuilding(int id);

        // Комнаты
        RoomDto GetRoomById(int id);
        IEnumerable<RoomDto> GetRoomsByBuildingId(int buildingId);
        void AddRoomToBuilding(AddRoomRequest request);
        void UpdateRoom(UpdateRoomRequest request);
        void RemoveRoomFromBuilding(int roomId);

        // Отчетность по зданиям
        IEnumerable<RoomDto> GetAvailableRooms();
        IEnumerable<RoomDto> GetRentedRooms();
        decimal GetBuildingOccupancyRate(int buildingId);
        decimal GetTotalRentedArea(int buildingId);
    }
}
