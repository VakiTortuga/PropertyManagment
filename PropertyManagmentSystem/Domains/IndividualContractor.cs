using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyManagmentSystem.Enums;
using Newtonsoft.Json;

namespace PropertyManagmentSystem.Domains
{
    public class IndividualContractor : Contractor
    {
        // ОСНОВНЫЕ ДАННЫЕ ФИЗ. ЛИЦА
        public string FullName { get; private set; }
        public PassportData Passport { get; private set; }

        // КОНСТРУКТОР для JSON десериализации
        [JsonConstructor]
        public IndividualContractor(int id, string phone, string fullName, PassportData passport)
            : base(id, phone, ContractorType.Individual)
        {
            // При загрузке из JSON минимальная проверка
            FullName = fullName ?? string.Empty;
            Passport = passport;
        }

        // СТАТИЧЕСКИЙ МЕТОД для создания с полной валидацией
        public static IndividualContractor Create(int id, string phone, string fullName, PassportData passport)
        {
            ValidateBase(id, phone);
            Validate(fullName, passport);
            return new IndividualContractor(id, phone, fullName, passport);
        }

        private static void Validate(string fullName, PassportData passport)
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
