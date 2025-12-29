using Microsoft.Extensions.DependencyInjection;
using PropertyManagmentSystem.Application.Interfaces;
using PropertyManagmentSystem.Application.Services;
using PropertyManagmentSystem.Infrastructure.Repositories;
using PropertyManagmentSystem.Infrastructure.Interfaces;
using PropertyManagmentSystem.ViewModels;
using PropertyManagmentSystem.Views;
using System;
using System.Windows;

namespace PropertyManagmentSystem
{
    public partial class App : System.Windows.Application
    {
        private readonly ServiceProvider _serviceProvider;

        // Свойство для доступа к контейнеру из других мест
        public static ServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            // Инициализируем XAML ресурсы и обработчики, определенные в App.xaml
            InitializeComponent();

            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
            ServiceProvider = _serviceProvider; // Сохраняем для статического доступа
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Регистрация репозиториев
            services.AddSingleton<BuildingRepository>();
            services.AddSingleton<RoomRepository>();
            services.AddSingleton<IIndividualContractorRepository, IndividualContractorRepository>();
            services.AddSingleton<ILegalEntityContractorRepository, LegalEntityContractorRepository>();
            services.AddSingleton<AgreementRepository>();

            // Регистрация сервисов
            services.AddSingleton<IBuildingService, BuildingService>();
            services.AddSingleton<IContractorService, ContractorService>();
            services.AddSingleton<IAgreementService, AgreementService>();

            // ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<BuildingViewModel>();
            services.AddSingleton<ContractorViewModel>();
            services.AddSingleton<AgreementViewModel>();
            services.AddTransient<CreateAgreementViewModel>();

            // Views (UserControls)
            services.AddTransient<BuildingView>();
            services.AddTransient<ContractorView>();
            services.AddTransient<AgreementView>();

            // Главное окно
            services.AddSingleton<MainWindow>();

            // зарегистрировать генератор id, если отсутствует
            services.AddSingleton<IContractorIdGenerator, ContractorIdGenerator>();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<MainWindow>();

            // Устанавливаем DataContext через DI
            mainWindow.DataContext = _serviceProvider.GetService<MainViewModel>();

            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider.Dispose();
            base.OnExit(e);
        }
    }
}