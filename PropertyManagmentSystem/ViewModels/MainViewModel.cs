using System.Windows.Input;
using PropertyManagmentSystem.Application.Interfaces;

namespace PropertyManagmentSystem.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public BuildingViewModel BuildingVM { get; }
        public ContractorViewModel ContractorVM { get; }
        public AgreementViewModel AgreementVM { get; }

        public ICommand ShowBuildingsCommand { get; }
        public ICommand ShowContractorsCommand { get; }
        public ICommand ShowAgreementsCommand { get; }

        public MainViewModel(
            IBuildingService buildingService,
            IContractorService contractorService,
            IAgreementService agreementService
            )
        {
            BuildingVM = new BuildingViewModel(buildingService);
            ContractorVM = new ContractorViewModel(contractorService);
            AgreementVM = new AgreementViewModel(agreementService, contractorService, buildingService);

            ShowBuildingsCommand = new RelayCommand(() => CurrentViewModel = BuildingVM);
            ShowContractorsCommand = new RelayCommand(() => CurrentViewModel = ContractorVM);
            ShowAgreementsCommand = new RelayCommand(() => CurrentViewModel = AgreementVM);

            CurrentViewModel = BuildingVM;
        }
    }
}
