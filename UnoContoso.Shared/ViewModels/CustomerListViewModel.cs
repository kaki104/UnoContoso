using Microsoft.Toolkit.Uwp.Helpers;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno;
using UnoContoso.Helpers;
using UnoContoso.Model;
using UnoContoso.Models;
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
            await DispatcherHelper.ExecuteOnUIThreadAsync(
                async() => Customers = await GetCustomerListAsync());
        }
    }
}
