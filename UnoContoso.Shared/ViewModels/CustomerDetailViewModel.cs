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
                Title = $"Details / {id}";
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
