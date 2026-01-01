using System;

namespace PropertyManagmentSystem.Application.Interfaces
{
    public interface IClock
    {
        DateTime Now { get; }
        event Action<DateTime> TimeChanged;
        void Advance(TimeSpan span);
        void Set(DateTime dateTime);
    }
}
