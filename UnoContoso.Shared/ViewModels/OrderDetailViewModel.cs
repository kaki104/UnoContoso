using Microsoft.Toolkit.Uwp.Helpers;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using UnoContoso.Model;
using UnoContoso.Models;
using UnoContoso.Repository;

namespace UnoContoso.ViewModels
{
    public class OrderDetailViewModel : ViewModelBase
    {
        private readonly IContosoRepository _contosoRepository;

        private OrderWrapper _order;

        public OrderWrapper Order
        {
            get { return _order; }
            set { SetProperty(ref _order, value); }
        }

        public ICommand SaveCommand { get; set; }

        public ICommand RevertCommand { get; set; }

        public ICommand RefreshCommand { get; set; }

        public OrderDetailViewModel()
        {

        }

        public OrderDetailViewModel(IContainerProvider containerProvider,
            IContosoRepository contosoRepository)
            : base(containerProvider)
        {
            _contosoRepository = contosoRepository;
            Title = "Order Details";
            Init();
        }

        private void Init()
        {
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            if (navigationContext.Parameters.ContainsKey("CustomerId"))
            {
                //Order is a new order
                SetBusy("OrderLoad", true);
                var customerId = navigationContext.Parameters.GetValue<Guid>("CustomerId");
                await DispatcherHelper.ExecuteOnUIThreadAsync(
                    async () =>
                    {
                        var customer = await _contosoRepository.Customers.GetAsync(customerId);
                        Order = new OrderWrapper(_contosoRepository, new Order(customer));
                    });
                SetBusy("OrderLoad", false);
            }

            if(navigationContext.Parameters.ContainsKey("OrderId"))
            {
                //Order is an existing order
                SetBusy("OrderLoad", true);
                var orderId = navigationContext.Parameters.GetValue<Guid>("OrderId");
                await DispatcherHelper.ExecuteOnUIThreadAsync(
                    async () =>
                    {
                        var order = await _contosoRepository.Orders.GetAsync(orderId);
                        Order = new OrderWrapper(_contosoRepository, order);
                    });
                SetBusy("OrderLoad", false);
            }
        }

        /// <summary>
        /// Gets the set of order status values so we can populate the order status combo box. 
        /// </summary>
        public IList<string> OrderStatusValues => Enum.GetNames(typeof(OrderStatus)).ToList();

        /// <summary>
        /// Gets the set of payment term values, so we can populate the term combo box. 
        /// </summary>
        public IList<string> TermValues => Enum.GetNames(typeof(Term)).ToList();

        /// <summary>
        /// Gets the set of payment status values so we can populate the payment status combo box.
        /// </summary>
        public IList<string> PaymentStatusValues => Enum.GetNames(typeof(PaymentStatus)).ToList();

    }
}
