using System;
using System.Windows.Threading;
using PropertyManagmentSystem.Application.Interfaces;

namespace PropertyManagmentSystem.Application.Services
{
    public class AdjustableClock : IClock
    {
        private readonly DispatcherTimer _autoTimer;
        public static AdjustableClock Instance { get; } = new AdjustableClock();

        private AdjustableClock()
        {
            // По умолчанию синхронизируемся с системным временем
            _now = DateTime.Now;

            // Автоматический тик — обновляем каждую секунду, чтобы "время шло"
            _autoTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _autoTimer.Tick += (s, e) =>
            {
                _now = _now.AddSeconds(1);
                TimeChanged?.Invoke(_now);
            };
            _autoTimer.Start();
        }

        private DateTime _now;
        public DateTime Now => _now;

        public event Action<DateTime> TimeChanged;

        // Перемотать на заданный интервал
        public void Advance(TimeSpan span)
        {
            if (span == TimeSpan.Zero) return;
            _now = _now.Add(span);
            TimeChanged?.Invoke(_now);
        }

        // Установить конкретную дату/время
        public void Set(DateTime dateTime)
        {
            _now = dateTime;
            TimeChanged?.Invoke(_now);
        }
    }
}
