using Microsoft.Toolkit.Uwp.Helpers;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using UnoContoso.Model;
using UnoContoso.Repository;

namespace UnoContoso.ViewModels
{
    public class CustomerDetailViewModel : ViewModelBase
    {
        private CustomerWrapper _customer;
        private readonly IContosoRepository _contosoRepository;

        public CustomerWrapper Customer
        {
            get { return _customer; }
            set { SetProperty(ref _customer, value); }
        }

        public ICommand SaveCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        public ICommand EditCommand { get; set; }

        public ICommand AddOrderCommand { get; set; }

        public ICommand RefreshOrderCommand { get; set; }

        private string _searchBoxText;
        public string SearchBoxText
        {
            get { return _searchBoxText; }
            set { SetProperty(ref _searchBoxText, value); }
        }

        private IList<string> _suggestItems;

        public IList<string> SuggestItems
        {
            get { return _suggestItems; }
            set { SetProperty(ref _suggestItems, value); }
        }

        private string _queryText;
        public string QueryText
        {
            get { return _queryText; }
            set { SetProperty(ref _queryText, value); }
        }

        public CustomerDetailViewModel()
        {
        }

        public CustomerDetailViewModel(IContainerProvider containerProvider,
            IContosoRepository contosoRepository)
            : base(containerProvider)
        {
            Title = "Details";
            _contosoRepository = contosoRepository;
            Init();
        }
        private void Init()
        {
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            if(navigationContext.Parameters.ContainsKey("CustomerId") == false)
            {
                Customer = new CustomerWrapper 
                {
                    IsNewCustomer = true,
                    IsInEdit = true
                };
            }
            else
            {
                SetBusy("CustomerLoad", true);
                var id = navigationContext.Parameters.GetValue<Guid>("CustomerId");
                //Debug.WriteLine($"{Title} / {id}");
                await DispatcherHelper.ExecuteOnUIThreadAsync(
                    async () =>
                    {
                        var customer = await _contosoRepository.Customers.GetAsync(id);
                        Customer = new CustomerWrapper(_contosoRepository, customer);
                        await Customer.LoadOrdersAsync();
                    });
                SetBusy("CustomerLoad", false);
            }
        }
    }
}
