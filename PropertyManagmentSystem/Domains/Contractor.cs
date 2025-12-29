using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyManagmentSystem.Enums;
using Newtonsoft.Json;

namespace PropertyManagmentSystem.Domains
{
    public abstract class Contractor
    {
        // БАЗОВЫЕ СВОЙСТВА
        public int Id { get; private set; }
        public ContractorType Type { get; protected set; }
        public string Phone { get; protected set; }
        public DateTime RegistrationDate { get; private set; }
        public bool IsActive { get; private set; } = true;

        // СВЯЗИ
        private List<int> _agreementIds = new List<int>();
        public IReadOnlyCollection<int> AgreementIds => _agreementIds.AsReadOnly();

        // КОНСТРУКТОР для JSON десериализации
        [JsonConstructor]
        protected Contractor(int id, string phone, ContractorType type)
        {
            // При загрузке из JSON минимальная проверка
            Id = id;
            Phone = phone ?? string.Empty;
            Type = type;
            RegistrationDate = DateTime.UtcNow;
        }

        // СТАТИЧЕСКИЙ МЕТОД для валидации
        protected static void ValidateBase(int id, string phone)
        {
            if (id <= 0)
                throw new ArgumentException("ID должен быть положительным числом");
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Телефон обязателен");
            if (phone.Length < 5)
                throw new ArgumentException("Телефон слишком короткий");
        }

        // БИЗНЕС-МЕТОДЫ ДЛЯ ДОГОВОРОВ

        public void AddAgreement(int agreementId)
        {
            if (agreementId <= 0)
                throw new ArgumentException("ID договора должен быть положительным");

            if (_agreementIds.Contains(agreementId))
                throw new InvalidOperationException("Договор уже добавлен этому арендатору");

            _agreementIds.Add(agreementId);
        }

        public void RemoveAgreement(int agreementId)
        {
            if (!_agreementIds.Contains(agreementId))
                throw new ArgumentException("Договор не найден у этого арендатора");

            _agreementIds.Remove(agreementId);
        }

        public bool HasActiveAgreements(IDictionary<int, Agreement> allAgreements)
        {
            if (allAgreements == null)
                return false;

            return _agreementIds.Any(agreementId =>
                allAgreements.TryGetValue(agreementId, out var agreement) &&
                agreement.IsActive(DateTime.Now));
        }

        public int GetActiveAgreementsCount(IDictionary<int, Agreement> allAgreements)
        {
            if (allAgreements == null)
                return 0;

            return _agreementIds.Count(agreementId =>
                allAgreements.TryGetValue(agreementId, out var agreement) &&
                agreement.IsActive(DateTime.Now));
        }

        // МЕТОДЫ ДЛЯ ИЗМЕНЕНИЯ ДАННЫХ

        public void ChangePhone(string newPhone)
        {
            if (string.IsNullOrWhiteSpace(newPhone))
                throw new ArgumentException("Телефон не может быть пустым");

            if (newPhone == Phone)
                return; // Ничего не меняем

            Phone = newPhone;
        }

        public void Deactivate(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Причина деактивации обязательна");

            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }

        // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ

        public abstract string GetDisplayName(); // Реализуют наследники

        public virtual string GetContactInfo() => $"Телефон: {Phone}";

        public bool HasAgreement(int agreementId) => _agreementIds.Contains(agreementId);

        public int GetAgreementsCount() => _agreementIds.Count;

        public bool CanCreateNewAgreement(IDictionary<int, Agreement> allAgreements)
        {
            if (!IsActive)
                return false;

            // максимум 5 активных договоров на арендатора
            const int maxActiveAgreements = 5;
            return GetActiveAgreementsCount(allAgreements) < maxActiveAgreements;
        }

        // ВАЛИДАЦИОННЫЕ МЕТОДЫ

        public virtual bool IsValid()
        {
            return Id > 0 &&
                   !string.IsNullOrWhiteSpace(Phone) &&
                   Phone.Length >= 5;
        }
    }
}
