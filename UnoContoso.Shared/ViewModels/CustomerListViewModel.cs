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

        private IList<CustomerWrapper> _allCustomers;

        private IList<CustomerWrapper> _customers;

        public IList<CustomerWrapper> Customers 
        { 
            get => _customers;
            set => SetProperty(ref _customers, value); 
        }

        private CustomerWrapper _selectedCustomer;

        public CustomerWrapper SelectedCustomer
        {
            get { return _selectedCustomer; }
            set { SetProperty(ref _selectedCustomer, value); }
        }

        private string _searchBoxText;
        public string SearchBoxText
        {
            get { return _searchBoxText; }
            set { SetProperty(ref _searchBoxText, value); }
        }

        private string _queryText;
        public string QueryText
        {
            get { return _queryText; }
            set { SetProperty(ref _queryText, value); }
        }

        private IList<string> _suggestItems;

        public IList<string> SuggestItems
        {
            get { return _suggestItems; }
            set { SetProperty(ref _suggestItems, value); }
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
            ViewDetailCommand = new DelegateCommand(OnViewDetail, 
                () => SelectedCustomer != null)
                .ObservesProperty(() => SelectedCustomer);
            AddOrderCommand = new DelegateCommand(OnAddOrder,
                () => SelectedCustomer != null)
                .ObservesProperty(() => SelectedCustomer);
            NewCustomerCommand = new DelegateCommand(OnNewCustomer);
            SyncCommand = new DelegateCommand(OnSync);

            PropertyChanged += CustomerListViewModel_PropertyChanged;
        }

        public override void Destroy()
        {
            _allCustomers?.Clear();
            Customers?.Clear();
        }

        private async void CustomerListViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(SearchBoxText):
                    SetSuggestItems(SearchBoxText);
                    break;
                case nameof(QueryText):
                    await SetCustomersAsync(QueryText);
                    break;
            }
        }

        private async Task SetCustomersAsync(string queryText)
        {
            if(string.IsNullOrEmpty(queryText))
            {
                Customers = _allCustomers;
            }
            else
            {
                string[] parameters = queryText.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries);
                var customers = _allCustomers
                    .Where(c => parameters.Any(p =>
                        c.Address.StartsWith(p, StringComparison.OrdinalIgnoreCase) ||
                        c.FirstName.StartsWith(p, StringComparison.OrdinalIgnoreCase) ||
                        c.LastName.StartsWith(p, StringComparison.OrdinalIgnoreCase) ||
                        c.Company.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
                    .OrderByDescending(c => parameters.Count(p =>
                        c.Address.StartsWith(p, StringComparison.OrdinalIgnoreCase) ||
                        c.FirstName.StartsWith(p, StringComparison.OrdinalIgnoreCase) ||
                        c.LastName.StartsWith(p, StringComparison.OrdinalIgnoreCase) ||
                        c.Company.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    Customers = customers;
                });
            }
        }

        private void SetSuggestItems(string searchBoxText)
        {
            if(string.IsNullOrEmpty(searchBoxText))
            {
                Customers = _allCustomers;
                SuggestItems = null;
            }
            else
            {
                string[] parameters = searchBoxText.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries);
                SuggestItems = _allCustomers
                    .Where(c => parameters.Any(p =>
                        c.Address.StartsWith(p, StringComparison.OrdinalIgnoreCase) ||
                        c.FirstName.StartsWith(p, StringComparison.OrdinalIgnoreCase) ||
                        c.LastName.StartsWith(p, StringComparison.OrdinalIgnoreCase) ||
                        c.Company.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
                    .OrderByDescending(c => parameters.Count(p =>
                        c.Address.StartsWith(p, StringComparison.OrdinalIgnoreCase) ||
                        c.FirstName.StartsWith(p, StringComparison.OrdinalIgnoreCase) ||
                        c.LastName.StartsWith(p, StringComparison.OrdinalIgnoreCase) ||
                        c.Company.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
                    .Select(c => $"{c.FirstName} {c.LastName}")
                    .ToList();
            }
        }

        private void OnSync()
        {
            Task.Run(async () => 
            {
                SetBusy("Sync", true);
                foreach(var modifiedCustomer in Customers?
                    .Where(c => c.IsModified)
                    .Select(c => c.Model))
                {
                    await _contosoRepository.Customers.UpsertAsync(modifiedCustomer);
                }
                SetBusy("Sync", false);
            });
        }

        private void OnNewCustomer()
        {
            NavigationService.RequestNavigate("CustomerDetailView");
        }

        private void OnAddOrder()
        {
            NavigationService.RequestNavigate("OrderDetailView",
                new NavigationParameters
                {
                    {"CustomerId", SelectedCustomer?.Model.Id }
                });
        }

        private void OnViewDetail()
        {
            NavigationService.RequestNavigate("CustomerDetailView",
                new NavigationParameters
                {
                    {"CustomerId", SelectedCustomer?.Model.Id }
                });
        }

        public async Task<IList<CustomerWrapper>> GetCustomerListAsync()
        {
            var customers = await _contosoRepository.Customers.GetAsync();
            if (customers == null
                || customers.Any() == false) return null;

            Customers?.Clear();
            var custs = from c in customers
                        select new CustomerWrapper(_contosoRepository, c);
            return new ObservableCollection<CustomerWrapper>(custs);
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            if(Customers?.Any() == false)
            {
                SetBusy("FirstLoading", true);
                await DispatcherHelper.ExecuteOnUIThreadAsync(
                    async () => 
                    {
                        _allCustomers = await GetCustomerListAsync();
                        Customers = _allCustomers; 
                    });
                SetBusy("FirstLoading", false);
            }
        }
    }
}
