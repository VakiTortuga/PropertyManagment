using System;
using System.Windows;
using System.Windows.Threading;
using PropertyManagmentSystem.Application.Services;
using PropertyManagmentSystem.Application.Interfaces;

namespace PropertyManagmentSystem.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Подписываемся на обновления времени (UI-поток)
            try
            {
                var clock = AdjustableClock.Instance;
                clock.TimeChanged += OnClockTimeChanged;
                // Установим начальное значение
                CurrentTimeText.Text = clock.Now.ToString("g");
            }
            catch
            {
                // безопасно игнорируем
            }
        }

        private void OnClockTimeChanged(DateTime now)
        {
            // Обновляем UI на UI-потоке
            Dispatcher.BeginInvoke((Action)(() =>
            {
                CurrentTimeText.Text = now.ToString("g");
            }), DispatcherPriority.Normal);
        }

        private void AdvanceDay_Click(object sender, RoutedEventArgs e)
        {
            AdjustableClock.Instance.Advance(TimeSpan.FromDays(1));
        }

        private void AdvanceMonth_Click(object sender, RoutedEventArgs e)
        {
            // приблизительная перемотка на 1 месяц
            var now = AdjustableClock.Instance.Now;
            var newDate = now.AddMonths(1);
            AdjustableClock.Instance.Set(newDate);
        }

        private void ResetToNow_Click(object sender, RoutedEventArgs e)
        {
            AdjustableClock.Instance.Set(DateTime.Now);
        }
    }
}
