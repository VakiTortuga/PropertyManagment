using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagmentSystem.Domains
{
    public class RentedItem : IEquatable<RentedItem>
    {
        // ОСНОВНЫЕ СВОЙСТВА
        public int RoomId { get; private set; }
        public RoomPurpose Purpose { get; private set; }
        public DateTime RentUntil { get; private set; }
        public decimal RentAmount { get; private set; }

        // ДОПОЛНИТЕЛЬНЫЕ СВОЙСТВА
        public DateTime? ActualVacationDate { get; private set; } // Фактическая дата освобождения
        public bool IsEarlyTerminated { get; private set; }
        public string EarlyTerminationReason { get; private set; }

        // КОНСТРУКТОР
        public RentedItem(int roomId, RoomPurpose purpose, DateTime rentUntil, decimal rentAmount)
        {
            Validate(roomId, purpose, rentUntil, rentAmount);

            RoomId = roomId;
            Purpose = purpose;
            RentUntil = rentUntil;
            RentAmount = rentAmount;
            IsEarlyTerminated = false;
        }

        private void Validate(int roomId, RoomPurpose purpose, DateTime rentUntil, decimal rentAmount)
        {
            if (roomId <= 0)
                throw new ArgumentException("ID комнаты должен быть положительным");
            if (rentUntil <= DateTime.Now)
                throw new ArgumentException("Срок аренды должен быть в будущем");
            if (rentAmount <= 0)
                throw new ArgumentException("Сумма аренды должна быть положительной");
        }

        // БИЗНЕС-МЕТОДЫ

        public bool IsActive(DateTime date) => date <= RentUntil && !IsEarlyTerminated;

        public bool IsExpired(DateTime date) => date > RentUntil && !IsEarlyTerminated;

        public void TerminateEarly(string reason, DateTime terminationDate)
        {
            if (IsEarlyTerminated)
                throw new InvalidOperationException("Аренда уже досрочно прекращена");
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Причина расторжения обязательна");
            if (terminationDate > RentUntil)
                throw new ArgumentException("Дата расторжения не может быть позже срока аренды");

            IsEarlyTerminated = true;
            EarlyTerminationReason = reason;
            ActualVacationDate = terminationDate;
        }

        public void ExtendRent(DateTime newRentUntil, decimal? newRentAmount = null)
        {
            if (IsEarlyTerminated)
                throw new InvalidOperationException("Нельзя продлить досрочно прекращённую аренду");
            if (newRentUntil <= RentUntil)
                throw new ArgumentException("Новый срок должен быть позже текущего");

            RentUntil = newRentUntil;
            if (newRentAmount.HasValue && newRentAmount.Value > 0)
                RentAmount = newRentAmount.Value;
        }

        public decimal CalculatePenalty(DateTime terminationDate, decimal penaltyRate)
        {
            if (!IsEarlyTerminated || !ActualVacationDate.HasValue)
                return 0;

            var monthsLeft = (RentUntil - terminationDate).Days / 30.0m;
            return RentAmount * monthsLeft * penaltyRate;
        }

        // ИНФОРМАЦИОННЫЕ МЕТОДЫ

        public int GetDaysRemaining(DateTime date) =>
            IsActive(date) ? (RentUntil - date).Days : 0;

        public bool IsOverdue(DateTime date) => date > RentUntil && !IsEarlyTerminated;

        public string GetStatus(DateTime date)
        {
            if (IsEarlyTerminated) return "Досрочно прекращена";
            if (date > RentUntil) return "Истекла";
            return "Активна";
        }

        // РАВЕНСТВО (Value Object)

        public bool Equals(RentedItem other)
        {
            if (other is null) return false;
            return RoomId == other.RoomId &&
                   Purpose == other.Purpose &&
                   RentUntil == other.RentUntil;
        }

        public override bool Equals(object obj) => Equals(obj as RentedItem);

        public override int GetHashCode()
        {
            return RoomId.GetHashCode() ^ RentUntil.GetHashCode();
        }
    }

    public enum RoomPurpose
    {
        Office,         // Офис
        Kiosk,          // Киоск
        Warehouse,      // Склад
        Retail,         // Торговое помещение
        Cafe,           // Кафе/ресторан
        Other           // Иное
    }
}
