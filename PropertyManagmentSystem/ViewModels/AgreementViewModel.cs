using PropertyManagmentSystem.Application.DTOs;
using PropertyManagmentSystem.Application.Interfaces;
using PropertyManagmentSystem.Application.Requests;
using PropertyManagmentSystem.Enums;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PropertyManagmentSystem.ViewModels
{
    public class AgreementViewModel : ViewModelBase
    {
        private readonly IAgreementService _agreementService;
        private readonly IContractorService _contractorService;
        private readonly IBuildingService _building_service;

        public AgreementViewModel(
            IAgreementService agreementService,
            IContractorService contractorService,
            IBuildingService buildingService)
        {
            _agreementService = agreementService;
            _contractorService = contractorService;
            _building_service = buildingService;

            Agreements = new ObservableCollection<AgreementDto>();
            ActiveAgreements = new ObservableCollection<AgreementDto>();
            Contractors = new ObservableCollection<ContractorDto>();
            AvailableRooms = new ObservableCollection<RoomDisplay>();

            SelectedAgreementRooms = new ObservableCollection<RentedItemDisplay>();

            LoadAgreementsCommand = new RelayCommand(LoadAgreements);
            LoadActiveAgreementsCommand = new RelayCommand(LoadActiveAgreements);
            LoadContractorsCommand = new RelayCommand(LoadContractors);
            LoadAvailableRoomsCommand = new RelayCommand(LoadAvailableRooms);
            CreateAgreementCommand = new RelayCommand(CreateAgreement);
            SignAgreementCommand = new RelayCommand(SignAgreement);
            CancelAgreementCommand = new RelayCommand(CancelAgreement);
            ProlongAgreementCommand = new RelayCommand(ProlongAgreement);
            AddRentedItemCommand = new RelayCommand(AddRentedItem);
            CalculatePenaltyCommand = new RelayCommand(CalculatePenalty);

            // Подписываемся на события, чтобы автоматически обновлять списки
            try
            {
                _contractorService.ContractorsChanged += LoadContractors;
            }
            catch { }

            try
            {
                _building_service.RoomsChanged += LoadAvailableRooms;
            }
            catch { }

            // Подписываемся на события AgreementService чтобы обновлять UI при изменениях
            try
            {
                _agreementService.AgreementsChanged += () =>
                {
                    // Обновление UI через диспетчер (безопасно из любого потока)
                    try
                    {
                        System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            LoadAgreements();
                            LoadActiveAgreements();
                        }));
                    }
                    catch
                    {
                    }
                };
            }
            catch { }

            try
            {
                _agreementService.RoomsChanged += () =>
                {
                    try
                    {
                        System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            LoadAvailableRooms();
                        }));
                    }
                    catch { }
                };
            }
            catch { }

            LoadAgreements();
            LoadActiveAgreements();
            LoadContractors();
            LoadAvailableRooms();
        }

        // ===== Properties =====

        private ObservableCollection<AgreementDto> _agreements;
        public ObservableCollection<AgreementDto> Agreements
        {
            get => _agreements;
            set { _agreements = value; OnPropertyChanged(); }
        }

        private AgreementDto _selectedAgreement;
        public AgreementDto SelectedAgreement
        {
            get => _selectedAgreement;
            set
            {
                _selectedAgreement = value;
                OnPropertyChanged();
                // When selection changes update detailed view
                LoadSelectedAgreementDetails();
            }
        }

        private ObservableCollection<AgreementDto> _activeAgreements;
        public ObservableCollection<AgreementDto> ActiveAgreements
        {
            get => _activeAgreements;
            set { _activeAgreements = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ContractorDto> _contractors;
        public ObservableCollection<ContractorDto> Contractors
        {
            get => _contractors;
            set { _contractors = value; OnPropertyChanged(); }
        }

        // Теперь AvailableRooms содержит удобные объекты с адресом здания
        private ObservableCollection<RoomDisplay> _availableRooms;
        public ObservableCollection<RoomDisplay> AvailableRooms
        {
            get => _availableRooms;
            set { _availableRooms = value; OnPropertyChanged(); }
        }

        private ContractorDto _selectedContractor;
        public ContractorDto SelectedContractorForNewAgreement
        {
            get => _selectedContractor;
            set { _selectedContractor = value; OnPropertyChanged(); }
        }

        private RoomDisplay _selectedRoom;
        public RoomDisplay SelectedRoomForRent
        {
            get => _selectedRoom;
            set { _selectedRoom = value; OnPropertyChanged(); }
        }

        private string _registrationNumber = "";
        public string RegistrationNumber
        {
            get => _registrationNumber;
            set { _registrationNumber = value; OnPropertyChanged(); }
        }

        private DateTime _startDate = DateTime.Now;
        public DateTime StartDate
        {
            get => _startDate;
            set { _startDate = value; OnPropertyChanged(); }
        }

        private DateTime _endDate = DateTime.Now.AddYears(1);
        public DateTime EndDate
        {
            get => _endDate;
            set { _endDate = value; OnPropertyChanged(); }
        }

        private PaymentFrequency _paymentFrequency = PaymentFrequency.Monthly;
        public PaymentFrequency PaymentFrequency
        {
            get => _paymentFrequency;
            set { _paymentFrequency = value; OnPropertyChanged(); }
        }

        private decimal _penaltyRate = 0.1m;
        public decimal PenaltyRate
        {
            get => _penaltyRate;
            set { _penaltyRate = value; OnPropertyChanged(); }
        }

        // Для добавления арендованного помещения
        private RoomPurpose _roomPurpose = RoomPurpose.Office;
        public RoomPurpose RoomPurpose
        {
            get => _roomPurpose;
            set { _roomPurpose = value; OnPropertyChanged(); }
        }

        private DateTime _rentUntil = DateTime.Now.AddYears(1);
        public DateTime RentUntil
        {
            get => _rentUntil;
            set { _rentUntil = value; OnPropertyChanged(); }
        }

        private decimal _rentAmount = 0;
        public decimal RentAmount
        {
            get => _rentAmount;
            set { _rentAmount = value; OnPropertyChanged(); }
        }

        // Enum-коллекции для привязки в UI
        public Array PaymentFrequencies =>
            Enum.GetValues(typeof(PaymentFrequency));

        public Array RoomPurposes =>
            Enum.GetValues(typeof(RoomPurpose));

        // Collection with friendly room info for selected agreement
        private ObservableCollection<RentedItemDisplay> _selectedAgreementRooms;
        public ObservableCollection<RentedItemDisplay> SelectedAgreementRooms
        {
            get => _selectedAgreementRooms;
            set { _selectedAgreementRooms = value; OnPropertyChanged(); }
        }

        private string _selectedAgreementContractorName;
        public string SelectedAgreementContractorName
        {
            get => _selectedAgreementContractorName;
            set { _selectedAgreementContractorName = value; OnPropertyChanged(); }
        }

        // ===== Commands =====

        public ICommand LoadAgreementsCommand { get; }
        public ICommand LoadActiveAgreementsCommand { get; }
        public ICommand LoadContractorsCommand { get; }
        public ICommand LoadAvailableRoomsCommand { get; }
        public ICommand CreateAgreementCommand { get; }
        public ICommand SignAgreementCommand { get; }
        public ICommand CancelAgreementCommand { get; }
        public ICommand ProlongAgreementCommand { get; }
        public ICommand AddRentedItemCommand { get; }
        public ICommand CalculatePenaltyCommand { get; }

        // ===== Methods =====

        private void LoadAgreements()
        {
            try
            {
                // Сохраняем предыдущий выбор (если был)
                var previousSelectedId = SelectedAgreement?.Id;

                Agreements.Clear();
                var agreements = _agreementService.GetAllAgreements();
                foreach (var agreement in agreements)
                {
                    Agreements.Add(agreement);
                }

                // Восстанавливаем выбор: если раньше было выбрано — пытаемся найти его по Id,
                // иначе выбираем первый только если до этого ничего не было выбрано.
                if (previousSelectedId.HasValue)
                {
                    var restore = Agreements.FirstOrDefault(a => a.Id == previousSelectedId.Value);
                    if (restore != null)
                    {
                        SelectedAgreement = restore;
                        return;
                    }
                }

                if (SelectedAgreement == null && Agreements.Count > 0)
                {
                    SelectedAgreement = Agreements[0];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке договоров: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadActiveAgreements()
        {
            try
            {
                ActiveAgreements.Clear();
                var agreements = _agreementService.GetActiveAgreements();
                foreach (var agreement in agreements)
                {
                    ActiveAgreements.Add(agreement);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке активных договоров: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadContractors()
        {
            try
            {
                var previousSelectedId = SelectedContractorForNewAgreement?.Id;

                Contractors.Clear();
                var contractors = _contractorService.GetAllContractors();
                foreach (var contractor in contractors)
                {
                    Contractors.Add(contractor);
                }

                if (previousSelectedId.HasValue)
                {
                    var restore = Contractors.FirstOrDefault(c => c.Id == previousSelectedId.Value);
                    if (restore != null)
                    {
                        SelectedContractorForNewAgreement = restore;
                        return;
                    }
                }

                if (SelectedContractorForNewAgreement == null && Contractors.Count > 0)
                {
                    SelectedContractorForNewAgreement = Contractors[0];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке арендаторов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAvailableRooms()
        {
            try
            {
                var previousRoomId = SelectedRoomForRent?.Id;

                AvailableRooms.Clear();
                var rooms = _building_service.GetAvailableRooms();
                foreach (var room in rooms)
                {
                    string address = string.Empty;
                    try
                    {
                        var building = _building_service.GetBuildingById(room.BuildingId);
                        if (building != null)
                        {
                            // Попытка взять понятное поле адреса
                            address = building.Address ?? string.Empty;
                        }
                    }
                    catch { }

                    AvailableRooms.Add(new RoomDisplay
                    {
                        Id = room.Id,
                        RoomNumber = room.RoomNumber,
                        BuildingAddress = address
                    });
                }

                if (previousRoomId.HasValue)
                {
                    var restore = AvailableRooms.FirstOrDefault(r => r.Id == previousRoomId.Value);
                    if (restore != null)
                    {
                        SelectedRoomForRent = restore;
                        return;
                    }
                }

                if (SelectedRoomForRent == null && AvailableRooms.Count > 0)
                {
                    SelectedRoomForRent = AvailableRooms[0];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке доступных помещений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateAgreement()
        {
            if (SelectedContractorForNewAgreement == null)
            {
                MessageBox.Show("Выберите арендатора", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(RegistrationNumber))
            {
                MessageBox.Show("Регистрационный номер обязателен для заполнения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (EndDate <= StartDate)
            {
                MessageBox.Show("Дата окончания должна быть позже даты начала", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (PenaltyRate < 0 || PenaltyRate > 1)
            {
                MessageBox.Show("Штрафная ставка должна быть от 0 до 1", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var request = new CreateAgreementRequest
            {
                RegistrationNumber = RegistrationNumber,
                StartDate = StartDate,
                EndDate = EndDate,
                PaymentFrequency = PaymentFrequency,
                ContractorId = SelectedContractorForNewAgreement.Id,
                PenaltyRate = PenaltyRate
            };

            try
            {
                _agreementService.CreateAgreement(request);

                // Сброс полей
                RegistrationNumber = "";
                StartDate = DateTime.Now;
                EndDate = DateTime.Now.AddYears(1);
                PenaltyRate = 0.1m;

                MessageBox.Show("Договор успешно создан", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadAgreements();
                LoadActiveAgreements();
                LoadAvailableRooms();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании договора: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SignAgreement()
        {
            if (SelectedAgreement == null)
            {
                MessageBox.Show("Выберите договор", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _agreementService.SignAgreement(SelectedAgreement.Id);
                MessageBox.Show("Договор успешно подписан", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadAgreements();
                LoadActiveAgreements();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подписании договора: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelAgreement()
        {
            if (SelectedAgreement == null)
            {
                MessageBox.Show("Выберите договор", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Вы уверены, что хотите отменить договор №{SelectedAgreement.RegistrationNumber}?",
                "Подтверждение отмены",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _agreementService.CancelAgreement(SelectedAgreement.Id, "По требованию арендатора");
                    MessageBox.Show("Договор успешно отменен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadAgreements();
                    LoadActiveAgreements();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при отмене договора: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ProlongAgreement()
        {
            if (SelectedAgreement == null)
            {
                MessageBox.Show("Выберите договор", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Вы уверены, что хотите продлить договор №{SelectedAgreement.RegistrationNumber}?",
                "Подтверждение продления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var request = new ProlongAgreementRequest
                    {
                        ExistingAgreementId = SelectedAgreement.Id,
                        NewStartDate = DateTime.Now,
                        NewEndDate = DateTime.Now.AddYears(1)
                    };

                    var newAgreement = _agreementService.ProlongAgreement(request);
                    MessageBox.Show($"Договор успешно продлен. Новый договор №{newAgreement.RegistrationNumber}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadAgreements();
                    LoadActiveAgreements();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при продлении договора: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddRentedItem()
        {
            if (SelectedAgreement == null)
            {
                MessageBox.Show("Выберите договор", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedRoomForRent == null)
            {
                MessageBox.Show("Выберите помещение для аренды", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (RentAmount <= 0)
            {
                MessageBox.Show("Сумма аренды должна быть больше 0", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (RentUntil <= DateTime.Now)
            {
                MessageBox.Show("Дата окончания аренды должна быть в будущем", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var request = new AddRentedItemRequest
            {
                AgreementId = SelectedAgreement.Id,
                RoomId = SelectedRoomForRent.Id,
                Purpose = RoomPurpose,
                RentUntil = RentUntil,
                RentAmount = RentAmount
            };

            try
            {
                _agreementService.AddRentedItemToAgreement(request);

                // Сброс полей
                RoomPurpose = RoomPurpose.Office;
                RentUntil = DateTime.Now.AddYears(1);
                RentAmount = 0;

                MessageBox.Show("Арендованное помещение успешно добавлено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                // После добавления мы перезагрузим списки — методы Load* теперь попытаются восстановить выбор по Id.
                LoadAgreements();
                LoadAvailableRooms();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении арендованного помещения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CalculatePenalty()
        {
            if (SelectedAgreement == null)
            {
                MessageBox.Show("Выберите договор", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var penalty = _agreementService.CalculatePenalty(SelectedAgreement.Id);
                MessageBox.Show($"Размер штрафа по договору №{SelectedAgreement.RegistrationNumber}: {penalty:C}", "Расчет штрафа", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при расчете штрафа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadSelectedAgreementDetails()
        {
            SelectedAgreementRooms.Clear();
            SelectedAgreementContractorName = string.Empty;

            if (SelectedAgreement == null) return;

            // load contractor name
            try
            {
                var contractor = _contractorService.GetContractorById(SelectedAgreement.ContractorId);
                SelectedAgreementContractorName = contractor?.DisplayName ?? string.Empty;
            }
            catch { SelectedAgreementContractorName = string.Empty; }

            if (SelectedAgreement.RentedItems == null) return;

            // В методе LoadSelectedAgreementDetails() при создании RentedItemDisplay
            foreach (var ri in SelectedAgreement.RentedItems)
            {
                try
                {
                    var room = _building_service.GetRoomById(ri.RoomId);
                    var roomNumber = room?.RoomNumber ?? ri.RoomId.ToString();

                    string buildingAddress = string.Empty;
                    try
                    {
                        var building = _building_service.GetBuildingById(room.BuildingId);
                        buildingAddress = building?.Address ?? string.Empty;
                    }
                    catch { /* ignore */ }

                    SelectedAgreementRooms.Add(new RentedItemDisplay
                    {
                        RoomId = ri.RoomId,
                        RoomNumber = roomNumber,
                        BuildingAddress = buildingAddress,
                        Purpose = ri.Purpose,
                        RentAmount = ri.RentAmount,
                        RentUntil = ri.RentUntil
                    });
                }
                catch
                {
                    SelectedAgreementRooms.Add(new RentedItemDisplay
                    {
                        RoomId = ri.RoomId,
                        RoomNumber = ri.RoomId.ToString(),
                        BuildingAddress = string.Empty,
                        Purpose = ri.Purpose,
                        RentAmount = ri.RentAmount,
                        RentUntil = ri.RentUntil
                    });
                }
            }
        }

        // Small helper display DTO
        public class RentedItemDisplay
        {
            public int RoomId { get; set; }
            public string RoomNumber { get; set; }
            public string BuildingAddress { get; set; }
            public RoomPurpose Purpose { get; set; }
            public decimal RentAmount { get; set; }
            public DateTime RentUntil { get; set; }
        }

        // Локальный DTO для отображения комнаты с адресом здания
        public class RoomDisplay
        {
            public int Id { get; set; }
            public string RoomNumber { get; set; }
            public string BuildingAddress { get; set; }
            public override string ToString() => string.IsNullOrWhiteSpace(BuildingAddress) ? RoomNumber : $"{RoomNumber} — {BuildingAddress}";
        }
    }
}