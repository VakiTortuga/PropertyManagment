using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PropertyManagmentSystem.Domains
{
    // Банковские реквизиты юридического лица
    public class BankDetails : IEquatable<BankDetails>
    {
        public string BankName { get; }
        public string AccountNumber { get; }

        // КОНСТРУКТОР для JSON десериализации
        [JsonConstructor]
        public BankDetails(string bankName, string accountNumber)
        {
            // При загрузке из JSON минимальная проверка
            BankName = bankName ?? string.Empty;
            AccountNumber = accountNumber ?? string.Empty;
        }

        // СТАТИЧЕСКИЙ МЕТОД для создания с полной валидацией
        public static BankDetails Create(string bankName, string accountNumber)
        {
            Validate(bankName, accountNumber);
            return new BankDetails(bankName, accountNumber);
        }

        private static void Validate(string bankName, string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(bankName))
                throw new ArgumentException("Название банка обязательно");
            if (string.IsNullOrWhiteSpace(accountNumber) || accountNumber.Length < 5)
                throw new ArgumentException("Номер счёта слишком короткий");
        }

        public bool Equals(BankDetails other)
        {
            if (other is null) return false;
            return AccountNumber == other.AccountNumber;
        }

        public override bool Equals(object obj) => Equals(obj as BankDetails);
        public override int GetHashCode() => AccountNumber.GetHashCode();

        public override string ToString() => $"{BankName}, счёт {AccountNumber}";
    }
}
