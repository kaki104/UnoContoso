using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace UnoContoso.ViewModels
{
    public class CustomerDetailViewModel : ViewModelBase
    {
        private IRegionNavigationService _navigationService;

        private bool _canGoBack;

        public bool CanGoBack 
        { 
            get => _canGoBack; 
            private set => SetProperty(ref _canGoBack ,value); 
        }

        public ICommand GoBackCommand { get; set; }


        public CustomerDetailViewModel()
        {
        }

        public CustomerDetailViewModel(IContainerProvider containerProvider)
            : base(containerProvider)
        {
            Title = "Customer Details";
            Init();
        }
        private void Init()
        {
            GoBackCommand = new DelegateCommand(
                () => _navigationService.Journal.GoBack(),
                () => CanGoBack)
                .ObservesCanExecute(() => CanGoBack);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            var id = navigationContext.Parameters.GetValue<Guid>("Id");
            _navigationService = navigationContext.NavigationService;
            CanGoBack = _navigationService.Journal.CanGoBack;
        }
    }
}
