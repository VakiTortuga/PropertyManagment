using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PropertyManagmentSystem.Application.DTOs;
using PropertyManagmentSystem.Application.Interfaces;
using PropertyManagmentSystem.Application.Requests;
using PropertyManagmentSystem.Domains;
using PropertyManagmentSystem.Enums;
using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Input;
using System.Windows;

namespace PropertyManagmentSystem.ViewModels
{
    public class CreateAgreementViewModel : ViewModelBase
    {
        private readonly IAgreementService _agreementService;
        private readonly IContractorService _contractorService;

        public CreateAgreementViewModel(
            IAgreementService agreementService,
            IContractorService contractorService)
        {
            _agreementService = agreementService;
            _contractorService = contractorService;

            Contractors = new ObservableCollection<ContractorDto>();

            CreateCommand = new RelayCommand(CreateAgreement);
            CancelCommand = new RelayCommand(Cancel);

            LoadContractors();
        }

        // ===== Properties =====

        private ObservableCollection<ContractorDto> _contractors;
        public ObservableCollection<ContractorDto> Contractors
        {
            get => _contractors;
            set { _contractors = value; OnPropertyChanged(); }
        }

        private ContractorDto _selectedContractor;
        public ContractorDto SelectedContractor
        {
            get => _selectedContractor;
            set { _selectedContractor = value; OnPropertyChanged(); }
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
            set
            {
                _startDate = value;
                OnPropertyChanged();
                // Автоматически обновляем EndDate, если он раньше StartDate
                if (_endDate < _startDate)
                {
                    EndDate = _startDate.AddYears(1);
                }
            }
        }

        private DateTime _endDate = DateTime.Now.AddYears(1);
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged();
            }
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

        public Array PaymentFrequencies =>
            Enum.GetValues(typeof(PaymentFrequency));

        // ===== Commands =====
        public ICommand CreateCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action AgreementCreated;

        // ===== Methods =====

        private void LoadContractors()
        {
            Contractors.Clear();
            var contractors = _contractorService.GetAllContractors();
            foreach (var contractor in contractors)
            {
                Contractors.Add(contractor);
            }

            if (Contractors.Count > 0)
            {
                SelectedContractor = Contractors[0];
            }
        }

        private void CreateAgreement()
        {
            if (SelectedContractor == null)
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
                ContractorId = SelectedContractor.Id,
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
                AgreementCreated?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании договора: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel()
        {
            // Сброс полей при отмене
            RegistrationNumber = "";
            StartDate = DateTime.Now;
            EndDate = DateTime.Now.AddYears(1);
            PenaltyRate = 0.1m;

            AgreementCreated?.Invoke();
        }
    }
}