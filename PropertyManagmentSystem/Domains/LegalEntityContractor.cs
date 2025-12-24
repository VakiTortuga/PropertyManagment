using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyManagmentSystem.Enums;

namespace PropertyManagmentSystem.Domains
{
    public class LegalEntityContractor : Contractor
    {
        // ОСНОВНЫЕ ДАННЫЕ ЮР. ЛИЦА
        public string CompanyName { get; private set; }
        public string DirectorName { get; private set; }
        public string LegalAddress { get; private set; }
        public string TaxId { get; private set; } // ИНН
        public BankDetails BankDetails { get; private set; }

        // КОНСТРУКТОР
        public LegalEntityContractor(
            int id,
            string phone,
            string companyName,
            string directorName,
            string legalAddress,
            string taxId,
            BankDetails bankDetails)
            : base(id, phone, ContractorType.LegalEntity)
        {
            Validate(companyName, directorName, legalAddress, taxId, bankDetails);

            CompanyName = companyName;
            DirectorName = directorName;
            LegalAddress = legalAddress;
            TaxId = taxId;
            BankDetails = bankDetails;
        }

        private void Validate(
            string companyName,
            string directorName,
            string legalAddress,
            string taxId,
            BankDetails bankDetails)
        {
            if (string.IsNullOrWhiteSpace(companyName))
                throw new ArgumentException("Название компании обязательно");
            if (string.IsNullOrWhiteSpace(directorName))
                throw new ArgumentException("ФИО руководителя обязательно");
            if (string.IsNullOrWhiteSpace(legalAddress))
                throw new ArgumentException("Юридический адрес обязателен");
            if (string.IsNullOrWhiteSpace(taxId) || taxId.Length != 10 && taxId.Length != 12)
                throw new ArgumentException("ИНН должен содержать 10 или 12 цифр");
            if (bankDetails == null)
                throw new ArgumentNullException(nameof(bankDetails));
        }

        // БИЗНЕС-МЕТОДЫ

        public override string GetDisplayName() => CompanyName;

        public override string GetContactInfo()
            => $"Компания: {CompanyName}\n" +
               $"Руководитель: {DirectorName}\n" +
               $"Адрес: {LegalAddress}\n" +
               $"Телефон: {Phone}\n" +
               $"ИНН: {TaxId}\n" +
               $"Банк: {BankDetails}";

        public void UpdateCompanyName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Название компании не может быть пустым");

            CompanyName = newName;
        }

        public void UpdateDirector(string newDirectorName)
        {
            if (string.IsNullOrWhiteSpace(newDirectorName))
                throw new ArgumentException("ФИО руководителя не может быть пустым");

            DirectorName = newDirectorName;
        }

        public void UpdateLegalAddress(string newAddress)
        {
            if (string.IsNullOrWhiteSpace(newAddress))
                throw new ArgumentException("Адрес не может быть пустым");

            LegalAddress = newAddress;
        }

        public void UpdateBankDetails(BankDetails newBankDetails)
        {
            if (newBankDetails == null)
                throw new ArgumentNullException(nameof(newBankDetails));

            BankDetails = newBankDetails;
        }

        // ДОПОЛНИТЕЛЬНАЯ ВАЛИДАЦИЯ

        public override bool IsValid()
        {
            return base.IsValid() &&
                   !string.IsNullOrWhiteSpace(CompanyName) &&
                   !string.IsNullOrWhiteSpace(DirectorName) &&
                   !string.IsNullOrWhiteSpace(LegalAddress) &&
                   !string.IsNullOrWhiteSpace(TaxId) &&
                   BankDetails != null;
        }

        // ДОПОЛНИТЕЛЬНЫЕ МЕТОДЫ

        public string GetShortInfo() => $"{CompanyName} (ИНН: {TaxId})";

        public bool IsTaxIdValid()
        {
            return !string.IsNullOrEmpty(TaxId) &&
                   TaxId.All(char.IsDigit) &&
                   (TaxId.Length == 10 || TaxId.Length == 12);
        }
    }
}
