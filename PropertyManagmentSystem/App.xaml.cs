using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PropertyManagmentSystem.Application.Interfaces;
using PropertyManagmentSystem.Application.Services;
using PropertyManagmentSystem.ViewModels;

namespace PropertyManagmentSystem
{
    public partial class App : System.Windows.Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Регистрация сервисов
            services.AddSingleton<IBuildingService, BuildingService>();
            services.AddSingleton<IContractorService, ContractorService>();
            services.AddSingleton<IAgreementService, AgreementService>();

            // ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<BuildingViewModel>();
            services.AddSingleton<ContractorViewModel>();
            services.AddSingleton<AgreementViewModel>();
            services.AddTransient<CreateAgreementViewModel>(); // Transient для диалогов

            // Главное окно
            services.AddSingleton<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.DataContext = _serviceProvider.GetService<MainViewModel>();
            mainWindow.Show();
        }
    }
}