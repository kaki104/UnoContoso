using Microsoft.Toolkit.Uwp.Helpers;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Uno;
using UnoContoso.Helpers;
using UnoContoso.Model;
using UnoContoso.Models;
using UnoContoso.Models.Consts;
using UnoContoso.Repository;
using Windows.UI.Core;

namespace UnoContoso.ViewModels
{
    public class CustomerListViewModel : ViewModelBase
    {
        private readonly IContosoRepository _contosoRepository;

        private IList<CustomerWrapper> _customers;

        public IList<CustomerWrapper> Customers 
        { 
            get => _customers;
            set => SetProperty(ref _customers, value); 
        }

        private CustomerWrapper _selectedCustomer;
        private IRegionNavigationService _navigationService;

        public CustomerWrapper SelectedCustomer
        {
            get { return _selectedCustomer; }
            set { SetProperty(ref _selectedCustomer, value); }
        }

        public ICommand ViewDetailCommand { get; set; }

        public ICommand AddOrderCommand { get; set; }

        public ICommand NewCustomerCommand { get; set; }

        public ICommand SyncCommand { get; set; }

        public CustomerListViewModel()
        {
        }

        public CustomerListViewModel(IContainerProvider containerProvider,
            IContosoRepository contosoRepository)
            : base(containerProvider)
        {
            Title = "Customer";
            _contosoRepository = contosoRepository;
            Customers = new ObservableCollection<CustomerWrapper>();

            Init();
        }

        private void Init()
        {
            ViewDetailCommand = new DelegateCommand(OnViewDetail);
            AddOrderCommand = new DelegateCommand(OnAddOrder);
            NewCustomerCommand = new DelegateCommand(OnNewCustomer);
            SyncCommand = new DelegateCommand(OnSync);
        }

        private void OnSync()
        {
        }

        private void OnNewCustomer()
        {
        }

        private void OnAddOrder()
        {
        }

        private void OnViewDetail()
        {
            _navigationService.RequestNavigate("CustomerDetailView",
                new NavigationParameters
                {
                    {"Id", SelectedCustomer?.Model.Id }
                });
        }

        public async Task<IList<CustomerWrapper>> GetCustomerListAsync()
        {
            var customers = await _contosoRepository.Customers.GetAsync();
            if (customers == null) return null;

            Customers?.Clear();
            var custs = from c in customers
                        select new CustomerWrapper(_contosoRepository, c);
            return new ObservableCollection<CustomerWrapper>(custs);
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            if(_navigationService == null)
            {
                _navigationService = navigationContext.NavigationService;
                await DispatcherHelper.ExecuteOnUIThreadAsync(
                    async () => Customers = await GetCustomerListAsync());
            }
        }
    }
}
