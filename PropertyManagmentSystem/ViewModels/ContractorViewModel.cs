using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PropertyManagmentSystem.Application.DTOs;
using PropertyManagmentSystem.Application.Interfaces;
using PropertyManagmentSystem.Application.Requests;
using PropertyManagmentSystem.Domains;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;

namespace PropertyManagmentSystem.ViewModels
{
    public class ContractorViewModel : ViewModelBase
    {
        private readonly IContractorService _contractorService;

        public ContractorViewModel(IContractorService contractorService)
        {
            _contractorService = contractorService;

            Contractors = new ObservableCollection<ContractorDto>();
            ContractorAgreements = new ObservableCollection<AgreementDto>();

            LoadContractorsCommand = new RelayCommand(LoadContractors);
            LoadAgreementsCommand = new RelayCommand(LoadAgreements);
            AddContractorCommand = new RelayCommand(AddContractor);
            UpdatePhoneCommand = new RelayCommand(UpdatePhone);

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
            set
            {
                _selectedContractor = value;
                OnPropertyChanged();
                if (value != null)
                {
                    LoadAgreements();
                }
            }
        }

        private ObservableCollection<AgreementDto> _contractorAgreements;
        public ObservableCollection<AgreementDto> ContractorAgreements
        {
            get => _contractorAgreements;
            set { _contractorAgreements = value; OnPropertyChanged(); }
        }

        private bool _isIndividualSelected = true;
        public bool IsIndividualSelected
        {
            get => _isIndividualSelected;
            set { _isIndividualSelected = value; OnPropertyChanged(); }
        }

        // Для физического лица
        private string _individualPhone = "";
        public string IndividualPhone
        {
            get => _individualPhone;
            set { _individualPhone = value; OnPropertyChanged(); }
        }

        private string _individualFullName = "";
        public string IndividualFullName
        {
            get => _individualFullName;
            set { _individualFullName = value; OnPropertyChanged(); }
        }

        private string _passportSeries = "";
        public string PassportSeries
        {
            get => _passportSeries;
            set { _passportSeries = value; OnPropertyChanged(); }
        }

        private string _passportNumber = "";
        public string PassportNumber
        {
            get => _passportNumber;
            set { _passportNumber = value; OnPropertyChanged(); }
        }

        private DateTime _passportIssueDate = DateTime.Now;
        public DateTime PassportIssueDate
        {
            get => _passportIssueDate;
            set { _passportIssueDate = value; OnPropertyChanged(); }
        }

        private string _passportIssuedBy = "";
        public string PassportIssuedBy
        {
            get => _passportIssuedBy;
            set { _passportIssuedBy = value; OnPropertyChanged(); }
        }

        // Для юридического лица
        private string _legalPhone = "";
        public string LegalPhone
        {
            get => _legalPhone;
            set { _legalPhone = value; OnPropertyChanged(); }
        }

        private string _companyName = "";
        public string CompanyName
        {
            get => _companyName;
            set { _companyName = value; OnPropertyChanged(); }
        }

        private string _directorName = "";
        public string DirectorName
        {
            get => _directorName;
            set { _directorName = value; OnPropertyChanged(); }
        }

        private string _legalAddress = "";
        public string LegalAddress
        {
            get => _legalAddress;
            set { _legalAddress = value; OnPropertyChanged(); }
        }

        private string _taxId = "";
        public string TaxId
        {
            get => _taxId;
            set { _taxId = value; OnPropertyChanged(); }
        }

        private string _bankName = "";
        public string BankName
        {
            get => _bankName;
            set { _bankName = value; OnPropertyChanged(); }
        }

        private string _accountNumber = "";
        public string AccountNumber
        {
            get => _accountNumber;
            set { _accountNumber = value; OnPropertyChanged(); }
        }

        // ===== Commands =====
        public ICommand LoadContractorsCommand { get; }
        public ICommand LoadAgreementsCommand { get; }
        public ICommand AddContractorCommand { get; }
        public ICommand UpdatePhoneCommand { get; }

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

        private void LoadAgreements()
        {
            if (SelectedContractor == null) return;

            ContractorAgreements.Clear();
            var agreements = _contractorService.GetContractorAgreements(SelectedContractor.Id);
            foreach (var agreement in agreements)
            {
                ContractorAgreements.Add(agreement);
            }
        }

        private void AddContractor()
        {
            if (IsIndividualSelected)
            {
                AddIndividualContractor();
            }
            else
            {
                AddLegalEntityContractor();
            }
        }

        private void AddIndividualContractor()
        {
            if (string.IsNullOrWhiteSpace(IndividualFullName))
            {
                MessageBox.Show("ФИО физического лица обязательно для заполнения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(PassportSeries) || string.IsNullOrWhiteSpace(PassportNumber))
            {
                MessageBox.Show("Серия и номер паспорта обязательны для заполнения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var request = new CreateIndividualContractorRequest
            {
                Phone = IndividualPhone,
                FullName = IndividualFullName,
                Passport = new PassportDataDto
                {
                    Series = PassportSeries,
                    Number = PassportNumber,
                    IssueDate = PassportIssueDate,
                    IssuedBy = PassportIssuedBy
                }
            };

            try
            {
                _contractorService.AddIndividualContractor(request);

                // Сброс полей
                IndividualPhone = "";
                IndividualFullName = "";
                PassportSeries = "";
                PassportNumber = "";
                PassportIssueDate = DateTime.Now;
                PassportIssuedBy = "";

                LoadContractors();

                MessageBox.Show("Физическое лицо успешно добавлено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении физического лица: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddLegalEntityContractor()
        {
            if (string.IsNullOrWhiteSpace(CompanyName))
            {
                MessageBox.Show("Название компании обязательно для заполнения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(TaxId))
            {
                MessageBox.Show("ИНН обязателен для заполнения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var request = new CreateLegalEntityContractorRequest
            {
                Phone = LegalPhone,
                CompanyName = CompanyName,
                DirectorName = DirectorName,
                LegalAddress = LegalAddress,
                TaxId = TaxId,
                BankDetails = new BankDetailsDto
                {
                    BankName = BankName,
                    AccountNumber = AccountNumber
                }
            };

            try
            {
                _contractorService.AddLegalEntityContractor(request);

                // Сброс полей
                LegalPhone = "";
                CompanyName = "";
                DirectorName = "";
                LegalAddress = "";
                TaxId = "";
                BankName = "";
                AccountNumber = "";

                LoadContractors();

                MessageBox.Show("Юридическое лицо успешно добавлено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении юридического лица: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdatePhone()
        {
            if (SelectedContractor == null) return;

            if (string.IsNullOrWhiteSpace(SelectedContractor.Phone))
            {
                MessageBox.Show("Номер телефона не может быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var request = new UpdateContractorPhoneRequest
            {
                ContractorId = SelectedContractor.Id,
                Phone = SelectedContractor.Phone
            };

            try
            {
                _contractorService.UpdateContractorPhone(request);
                MessageBox.Show("Номер телефона успешно обновлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении номера телефона: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}