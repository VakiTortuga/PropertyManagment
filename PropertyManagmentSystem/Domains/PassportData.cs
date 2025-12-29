using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PropertyManagmentSystem.Domains
{
    // Паспортные данные физического лица
    public class PassportData : IEquatable<PassportData>
    {
        public string Series { get; }
        public string Number { get; }
        public DateTime IssueDate { get; }
        public string IssuedBy { get; }

        // КОНСТРУКТОР для JSON десериализации
        [JsonConstructor]
        public PassportData(string series, string number, DateTime issueDate, string issuedBy)
        {
            // При загрузке из JSON минимальная проверка
            Series = series ?? string.Empty;
            Number = number ?? string.Empty;
            IssueDate = issueDate;
            IssuedBy = issuedBy ?? string.Empty;
        }

        // СТАТИЧЕСКИЙ МЕТОД для создания с полной валидацией
        public static PassportData Create(string series, string number, DateTime issueDate, string issuedBy)
        {
            Validate(series, number, issueDate, issuedBy);
            return new PassportData(series, number, issueDate, issuedBy);
        }

        private static void Validate(string series, string number, DateTime issueDate, string issuedBy)
        {
            if (string.IsNullOrWhiteSpace(series) || series.Length != 4)
                throw new ArgumentException("Серия паспорта должна содержать 4 цифры");
            if (string.IsNullOrWhiteSpace(number) || number.Length != 6)
                throw new ArgumentException("Номер паспорта должен содержать 6 цифр");
            if (issueDate > DateTime.Now)
                throw new ArgumentException("Дата выдачи не может быть в будущем");
            if (string.IsNullOrWhiteSpace(issuedBy))
                throw new ArgumentException("Кем выдан - обязательно");
        }

        public bool Equals(PassportData other)
        {
            if (other is null) return false;
            return Series == other.Series && Number == other.Number;
        }

        public override bool Equals(object obj) => Equals(obj as PassportData);
        public override int GetHashCode()
        {
            return Series.GetHashCode() ^ Number.GetHashCode();
        }

        public override string ToString() => $"{Series} {Number}, выдан {IssuedBy} {IssueDate:dd.MM.yyyy}";
    }
}
