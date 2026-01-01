using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyManagmentSystem.Enums;
using Newtonsoft.Json;

namespace PropertyManagmentSystem.Domains
{
    public class Agreement
    {
        // ОСНОВНЫЕ СВОЙСТВА
        public int Id { get; private set; }
        public string RegistrationNumber { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public PaymentFrequency PaymentFrequency { get; private set; }
        public string AdditionalTerms { get; private set; }
        public decimal PenaltyRate { get; private set; } // Процент штрафа

        // СВЯЗИ
        public int ContractorId { get; private set; }
        private List<RentedItem> _rentedItems = new List<RentedItem>();
        public IReadOnlyCollection<RentedItem> RentedItems => _rentedItems.AsReadOnly();

        // СТАТУС И ИСТОРИЯ
        public AgreementStatus Status { get; private set; } = AgreementStatus.Draft;
        public DateTime? SignedDate { get; private set; }
        public DateTime? CancellationDate { get; private set; }
        public string CancellationReason { get; private set; }

        // КОНСТРУКТОР для JSON десериализации
        [JsonConstructor]
        public Agreement(
            int id,
            string registrationNumber,
            DateTime startDate,
            DateTime endDate,
            PaymentFrequency paymentFrequency,
            int contractorId,
            decimal penaltyRate = 0.1m,
            AgreementStatus status = AgreementStatus.Draft,
            DateTime? signedDate = null,
            DateTime? cancellationDate = null,
            string cancellationReason = null,
            IEnumerable<RentedItem> rentedItems = null)
        {
            // При загрузке из JSON минимальная проверка
            Id = id;
            RegistrationNumber = registrationNumber ?? string.Empty;
            StartDate = startDate;
            EndDate = endDate;
            PaymentFrequency = paymentFrequency;
            ContractorId = contractorId;
            PenaltyRate = penaltyRate >= 0 && penaltyRate <= 1 ? penaltyRate : 0.1m;

            // При десериализации используем переданные значения (если есть)
            Status = status;
            SignedDate = signedDate;
            CancellationDate = cancellationDate;
            CancellationReason = cancellationReason;

            _rentedItems = rentedItems?.ToList() ?? new List<RentedItem>();
        }

        // СТАТИЧЕСКИЙ МЕТОД для создания нового договора (с полной валидацией)
        public static Agreement Create(
            int id,
            string registrationNumber,
            DateTime startDate,
            DateTime endDate,
            PaymentFrequency paymentFrequency,
            int contractorId,
            decimal penaltyRate = 0.1m)
        {
            Validate(id, registrationNumber, startDate, endDate, penaltyRate, contractorId);
            return new Agreement(id, registrationNumber, startDate, endDate, paymentFrequency, contractorId, penaltyRate);
        }

        private static void Validate(
            int id,
            string registrationNumber,
            DateTime startDate,
            DateTime endDate,
            decimal penaltyRate,
            int contractorId)
        {
            if (id <= 0)
                throw new ArgumentException("ID договора должен быть положительным");
            if (string.IsNullOrWhiteSpace(registrationNumber))
                throw new ArgumentException("Регистрационный номер обязателен");
            if (startDate >= endDate)
                throw new ArgumentException("Дата начала должна быть раньше даты окончания");
            if (startDate < DateTime.Today)
                throw new ArgumentException("Дата начала не может быть в прошлом");
            if (penaltyRate < 0 || penaltyRate > 1)
                throw new ArgumentException("Штрафная ставка должна быть от 0 до 1 (0-100%)");
            if (contractorId <= 0)
                throw new ArgumentException("ID арендатора должен быть положительным");
        }

        // БИЗНЕС-МЕТОДЫ ДЛЯ ПРЕДМЕТОВ АРЕНДЫ

        public void AddRentedItem(RentedItem item)
        {
            if (Status != AgreementStatus.Draft)
                throw new InvalidOperationException("Можно добавлять помещения только в черновик договора");

            if (_rentedItems.Any(ri => ri.RoomId == item.RoomId))
                throw new InvalidOperationException($"Комната с ID {item.RoomId} уже в договоре");

            // Проверяем, что срок аренды предмета не превышает срок договора
            if (item.RentUntil > EndDate)
                throw new InvalidOperationException(
                    $"Срок аренды комнаты ({item.RentUntil:dd.MM.yyyy}) " +
                    $"превышает срок действия договора ({EndDate:dd.MM.yyyy})");

            _rentedItems.Add(item);
        }

        public void RemoveRentedItem(int roomId)
        {
            if (Status != AgreementStatus.Draft)
                throw new InvalidOperationException("Можно удалять помещения только из черновика договора");

            var item = _rentedItems.FirstOrDefault(ri => ri.RoomId == roomId);
            if (item == null)
                throw new ArgumentException($"Комната с ID {roomId} не найдена в договоре");

            _rentedItems.Remove(item);
        }

        public RentedItem GetRentedItem(int roomId) =>
            _rentedItems.FirstOrDefault(ri => ri.RoomId == roomId);

        // МЕТОДЫ ДЛЯ РАБОТЫ С ДОГОВОРОМ

        // backward-compatible Sign — вызывает SignAt с DateTime.Now
        public void Sign()
        {
            SignAt(DateTime.Now);
        }

        // Подписать договор в момент now (используйте при виртуальном времени)
        public void SignAt(DateTime now)
        {
            if (Status != AgreementStatus.Draft)
                throw new InvalidOperationException("Можно подписать только черновик договора");

            if (_rentedItems.Count == 0)
                throw new InvalidOperationException("Договор должен содержать хотя бы одно помещение");

            Status = AgreementStatus.Active;
            SignedDate = now;
        }

        // backward-compatible Cancel
        public void Cancel(string reason)
        {
            CancelAt(reason, DateTime.Now);
        }

        // Отмена договора в момент now (используйте при виртуальном времени)
        public void CancelAt(string reason, DateTime now)
        {
            if (Status != AgreementStatus.Active)
                throw new InvalidOperationException("Можно отменить только активный договор");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Причина отмены обязательна");

            Status = AgreementStatus.Cancelled;
            CancellationDate = now;
            CancellationReason = reason;

            // При отмене досрочно прекращаем все аренды, используя переданное время
            foreach (var item in _rentedItems.Where(ri => ri.IsActive(now)))
            {
                item.TerminateEarly($"Досрочное расторжение договора: {reason}", now);
            }
        }

        // backward-compatible Complete
        public void Complete()
        {
            CompleteAt(DateTime.Now);
        }

        // Завершить договор в момент now (используйте при виртуальном времени)
        public void CompleteAt(DateTime now)
        {
            if (Status != AgreementStatus.Active)
                throw new InvalidOperationException("Можно завершить только активный договор");

            if (now < EndDate)
                throw new InvalidOperationException("Нельзя завершить договор до истечения срока");

            Status = AgreementStatus.Completed;
        }

        public void Extend(DateTime newEndDate, decimal? newPenaltyRate = null)
        {
            if (Status != AgreementStatus.Active)
                throw new InvalidOperationException("Можно продлить только активный договор");

            if (newEndDate <= EndDate)
                throw new ArgumentException("Новая дата окончания должна быть позже текущей");

            EndDate = newEndDate;
            if (newPenaltyRate.HasValue)
                PenaltyRate = newPenaltyRate.Value;
        }

        // РАСЧЁТНЫЕ МЕТОДЫ

        public bool IsActive(DateTime date) =>
            Status == AgreementStatus.Active &&
            date >= StartDate &&
            date <= EndDate;

        public decimal CalculateTotalMonthlyRent()
        {
            return _rentedItems.Sum(item => item.RentAmount);
        }

        public decimal CalculateTotalRentForPeriod()
        {
            var months = (EndDate.Year - StartDate.Year) * 12 + EndDate.Month - StartDate.Month;
            return CalculateTotalMonthlyRent() * months;
        }

        public decimal CalculatePenalty(DateTime date)
        {
            if (Status != AgreementStatus.Cancelled || !CancellationDate.HasValue)
                return 0;

            var totalPenalty = 0m;
            foreach (var item in _rentedItems)
            {
                totalPenalty += item.CalculatePenalty(CancellationDate.Value, PenaltyRate);
            }
            return totalPenalty;
        }

        public List<RentedItem> GetActiveRentedItems(DateTime date) =>
            _rentedItems.Where(ri => ri.IsActive(date)).ToList();

        public List<RentedItem> GetExpiredRentedItems(DateTime date) =>
            _rentedItems.Where(ri => ri.IsExpired(date)).ToList();

        // ИНФОРМАЦИОННЫЕ МЕТОДЫ

        public int GetDaysRemaining(DateTime date)
        {
            if (!IsActive(date)) return 0;
            return (EndDate - date).Days;
        }

        public bool HasRoom(int roomId) => _rentedItems.Any(ri => ri.RoomId == roomId);

        // ВАЛИДАЦИЯ

        public bool IsValid()
        {
            return Id > 0 &&
                   !string.IsNullOrWhiteSpace(RegistrationNumber) &&
                   StartDate < EndDate &&
                   ContractorId > 0 &&
                   _rentedItems.Count > 0;
        }
    }

}
