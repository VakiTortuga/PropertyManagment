using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PropertyManagmentSystem.Application.DTOs;
using PropertyManagmentSystem.Application.Interfaces;
using PropertyManagmentSystem.Application.Requests;
using PropertyManagmentSystem.Domains;
using PropertyManagmentSystem.Enums;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows;


namespace PropertyManagmentSystem.ViewModels
{
    public class BuildingViewModel : ViewModelBase
    {
        private readonly IBuildingService _buildingService;

        public BuildingViewModel(IBuildingService buildingService)
        {
            _buildingService = buildingService;

            Buildings = new ObservableCollection<BuildingDto>();
            Rooms = new ObservableCollection<RoomDto>();

            LoadBuildingsCommand = new RelayCommand(LoadBuildings);
            LoadRoomsCommand = new RelayCommand(LoadRooms);
            AddBuildingCommand = new RelayCommand(AddBuilding);
            UpdateBuildingCommand = new RelayCommand(UpdateBuilding);
            DeleteBuildingCommand = new RelayCommand(DeleteBuilding);
            AddRoomCommand = new RelayCommand(AddRoom);
            UpdateRoomCommand = new RelayCommand(UpdateRoom);
            DeleteRoomCommand = new RelayCommand(DeleteRoom);
            LoadStatisticsCommand = new RelayCommand(LoadStatistics);

            LoadBuildings();
        }

        // ===== Properties =====

        private ObservableCollection<BuildingDto> _buildings;
        public ObservableCollection<BuildingDto> Buildings
        {
            get => _buildings;
            set { _buildings = value; OnPropertyChanged(); }
        }

        private BuildingDto _selectedBuilding;
        public BuildingDto SelectedBuilding
        {
            get => _selectedBuilding;
            set
            {
                _selectedBuilding = value;
                OnPropertyChanged();
                if (value != null)
                {
                    LoadRooms();
                    LoadStatistics();
                }
            }
        }

        private ObservableCollection<RoomDto> _rooms;
        public ObservableCollection<RoomDto> Rooms
        {
            get => _rooms;
            set { _rooms = value; OnPropertyChanged(); }
        }

        private RoomDto _selectedRoom;
        public RoomDto SelectedRoom
        {
            get => _selectedRoom;
            set { _selectedRoom = value; OnPropertyChanged(); }
        }

        private decimal _occupancyRate;
        public decimal OccupancyRate
        {
            get => _occupancyRate;
            set { _occupancyRate = value; OnPropertyChanged(); }
        }

        private int _totalRooms;
        public int TotalRooms
        {
            get => _totalRooms;
            set { _totalRooms = value; OnPropertyChanged(); }
        }

        private int _availableRooms;
        public int AvailableRooms
        {
            get => _availableRooms;
            set { _availableRooms = value; OnPropertyChanged(); }
        }

        // Для создания нового здания
        private string _newBuildingDistrict = "";
        public string NewBuildingDistrict
        {
            get => _newBuildingDistrict;
            set { _newBuildingDistrict = value; OnPropertyChanged(); }
        }

        private string _newBuildingAddress = "";
        public string NewBuildingAddress
        {
            get => _newBuildingAddress;
            set { _newBuildingAddress = value; OnPropertyChanged(); }
        }

        private int _newBuildingFloorsCount = 1;
        public int NewBuildingFloorsCount
        {
            get => _newBuildingFloorsCount;
            set { _newBuildingFloorsCount = value; OnPropertyChanged(); }
        }

        private string _newBuildingCommandantPhone = "";
        public string NewBuildingCommandantPhone
        {
            get => _newBuildingCommandantPhone;
            set { _newBuildingCommandantPhone = value; OnPropertyChanged(); }
        }

        // Для создания новой комнаты
        private string _newRoomNumber = "";
        public string NewRoomNumber
        {
            get => _newRoomNumber;
            set { _newRoomNumber = value; OnPropertyChanged(); }
        }

        private decimal _newRoomArea = 20;
        public decimal NewRoomArea
        {
            get => _newRoomArea;
            set { _newRoomArea = value; OnPropertyChanged(); }
        }

        private int _newRoomFloor = 1;
        public int NewRoomFloor
        {
            get => _newRoomFloor;
            set { _newRoomFloor = value; OnPropertyChanged(); }
        }

        private FinishingType _newRoomFinishingType = FinishingType.Standard;
        public FinishingType NewRoomFinishingType
        {
            get => _newRoomFinishingType;
            set { _newRoomFinishingType = value; OnPropertyChanged(); }
        }

        private bool _newRoomHasPhone = true;
        public bool NewRoomHasPhone
        {
            get => _newRoomHasPhone;
            set { _newRoomHasPhone = value; OnPropertyChanged(); }
        }

        // ===== Commands =====
        public ICommand LoadBuildingsCommand { get; }
        public ICommand LoadRoomsCommand { get; }
        public ICommand AddBuildingCommand { get; }
        public ICommand UpdateBuildingCommand { get; }
        public ICommand DeleteBuildingCommand { get; }
        public ICommand AddRoomCommand { get; }
        public ICommand UpdateRoomCommand { get; }
        public ICommand DeleteRoomCommand { get; }
        public ICommand LoadStatisticsCommand { get; }

        // ===== Methods =====

        private void LoadBuildings()
        {
            Buildings.Clear();
            var buildings = _buildingService.GetAllBuildings();
            foreach (var building in buildings)
            {
                Buildings.Add(building);
            }

            if (Buildings.Count > 0)
            {
                SelectedBuilding = Buildings[0];
            }
        }

        private void LoadRooms()
        {
            if (SelectedBuilding == null) return;

            Rooms.Clear();
            var rooms = _buildingService.GetRoomsByBuildingId(SelectedBuilding.Id);
            foreach (var room in rooms)
            {
                Rooms.Add(room);
            }

            if (Rooms.Count > 0)
            {
                SelectedRoom = Rooms[0];
            }
        }

        private void LoadStatistics()
        {
            if (SelectedBuilding == null) return;

            OccupancyRate = _buildingService.GetBuildingOccupancyRate(SelectedBuilding.Id);
            var availableRooms = _buildingService.GetAvailableRooms();
            AvailableRooms = availableRooms.Count(r => r.BuildingId == SelectedBuilding.Id);
            TotalRooms = Rooms.Count;
        }

        private void AddBuilding()
        {
            if (string.IsNullOrWhiteSpace(NewBuildingAddress))
            {
                MessageBox.Show("Адрес здания обязателен для заполнения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var request = new CreateBuildingRequest
            {
                District = NewBuildingDistrict,
                Address = NewBuildingAddress,
                FloorsCount = NewBuildingFloorsCount,
                CommandantPhone = NewBuildingCommandantPhone
            };

            try
            {
                _buildingService.AddBuilding(request);

                // Сброс полей
                NewBuildingDistrict = "";
                NewBuildingAddress = "";
                NewBuildingFloorsCount = 1;
                NewBuildingCommandantPhone = "";

                LoadBuildings();

                MessageBox.Show("Здание успешно добавлено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении здания: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateBuilding()
        {
            if (SelectedBuilding == null) return;

            var request = new UpdateBuildingRequest
            {
                BuildingId = SelectedBuilding.Id,
                District = SelectedBuilding.District,
                CommandantPhone = SelectedBuilding.CommandantPhone
            };

            try
            {
                _buildingService.UpdateBuilding(request);
                MessageBox.Show("Информация о здании успешно обновлена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении здания: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteBuilding()
        {
            if (SelectedBuilding == null) return;

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить здание по адресу: {SelectedBuilding.Address}?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _buildingService.DeleteBuilding(SelectedBuilding.Id);
                    LoadBuildings();
                    MessageBox.Show("Здание успешно удалено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении здания: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddRoom()
        {
            if (SelectedBuilding == null)
            {
                MessageBox.Show("Выберите здание для добавления комнаты", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(NewRoomNumber))
            {
                MessageBox.Show("Номер комнаты обязателен для заполнения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (NewRoomArea <= 0)
            {
                MessageBox.Show("Площадь комнаты должна быть больше 0", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (NewRoomFloor <= 0 || NewRoomFloor > SelectedBuilding.FloorsCount)
            {
                MessageBox.Show($"Номер этажа должен быть от 1 до {SelectedBuilding.FloorsCount}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var request = new AddRoomRequest
            {
                BuildingId = SelectedBuilding.Id,
                RoomNumber = NewRoomNumber,
                Area = NewRoomArea,
                FloorNumber = NewRoomFloor,
                FinishingType = NewRoomFinishingType,
                HasPhone = NewRoomHasPhone
            };

            try
            {
                _buildingService.AddRoomToBuilding(SelectedBuilding.Id, request);

                // Сброс полей
                NewRoomNumber = "";
                NewRoomArea = 20;
                NewRoomFloor = 1;
                NewRoomFinishingType = FinishingType.Standard;
                NewRoomHasPhone = true;

                LoadRooms();
                LoadStatistics();

                MessageBox.Show("Комната успешно добавлена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении комнаты: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateRoom()
        {
            if (SelectedRoom == null) return;

            var request = new UpdateRoomRequest
            {
                RoomId = SelectedRoom.Id,
                FinishingType = SelectedRoom.FinishingType,
                HasPhone = SelectedRoom.HasPhone
            };

            try
            {
                _buildingService.UpdateRoom(request);
                MessageBox.Show("Информация о комнате успешно обновлена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении комнаты: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteRoom()
        {
            if (SelectedRoom == null) return;

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить комнату №{SelectedRoom.RoomNumber}?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _buildingService.RemoveRoomFromBuilding(SelectedRoom.Id);
                    LoadRooms();
                    LoadStatistics();
                    MessageBox.Show("Комната успешно удалена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении комнаты: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}