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
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using System.Windows.Input;

namespace PropertyManagmentSystem.ViewModels
{
    public class AgreementViewModel : ViewModelBase
    {
        private readonly IAgreementService _agreementService;
        private readonly IContractorService _contractorService;
        private readonly IBuildingService _buildingService;

        public AgreementViewModel(
            IAgreementService agreementService,
            IContractorService contractorService,
            IBuildingService buildingService)
        {
            _agreementService = agreementService;
            _contractorService = contractorService;
            _buildingService = buildingService;

            Agreements = new ObservableCollection<AgreementDto>();
            ActiveAgreements = new ObservableCollection<AgreementDto>();
            Contractors = new ObservableCollection<ContractorDto>();
            AvailableRooms = new ObservableCollection<RoomDto>();

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
            set { _selectedAgreement = value; OnPropertyChanged(); }
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

        private ObservableCollection<RoomDto> _availableRooms;
        public ObservableCollection<RoomDto> AvailableRooms
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

        private RoomDto _selectedRoom;
        public RoomDto SelectedRoomForRent
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
                Agreements.Clear();
                var agreements = _agreementService.GetAllAgreements();
                foreach (var agreement in agreements)
                {
                    Agreements.Add(agreement);
                }

                if (Agreements.Count > 0)
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
                Contractors.Clear();
                var contractors = _contractorService.GetAllContractors();
                foreach (var contractor in contractors)
                {
                    Contractors.Add(contractor);
                }

                if (Contractors.Count > 0)
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
                AvailableRooms.Clear();
                var rooms = _buildingService.GetAvailableRooms();
                foreach (var room in rooms)
                {
                    AvailableRooms.Add(room);
                }

                if (AvailableRooms.Count > 0)
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
                Purpose = RoomPurpose, // Теперь это enum, а не string
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
    }
}