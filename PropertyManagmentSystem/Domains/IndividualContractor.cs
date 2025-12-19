using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagmentSystem.Domains
{
    public class IndividualContractor : Contractor
    {
        // ОСНОВНЫЕ ДАННЫЕ ФИЗ. ЛИЦА
        public string FullName { get; private set; }
        public PassportData Passport { get; private set; }

        // КОНСТРУКТОР
        public IndividualContractor(int id, string phone, string fullName, PassportData passport)
            : base(id, phone, ContractorType.Individual)
        {
            Validate(fullName, passport);

            FullName = fullName;
            Passport = passport;
        }

        private void Validate(string fullName, PassportData passport)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("ФИО обязательно");
            if (fullName.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length < 3)
                throw new ArgumentException("Укажите ФИО");
            if (passport == null)
                throw new ArgumentNullException(nameof(passport));
        }

        // БИЗНЕС-МЕТОДЫ

        public override string GetDisplayName() => FullName;

        public override string GetContactInfo()
            => $"ФИО: {FullName}\nТелефон: {Phone}\nПаспорт: {Passport}";

        public void UpdateFullName(string newFullName)
        {
            if (string.IsNullOrWhiteSpace(newFullName))
                throw new ArgumentException("ФИО не может быть пустым");

            FullName = newFullName;
        }

        // ДОПОЛНИТЕЛЬНАЯ ВАЛИДАЦИЯ

        public override bool IsValid()
        {
            return base.IsValid() &&
                   !string.IsNullOrWhiteSpace(FullName) &&
                   Passport != null;
        }
    }
}
