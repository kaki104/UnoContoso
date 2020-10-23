using Microsoft.Toolkit.Uwp.Helpers;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using UnoContoso.Model;
using UnoContoso.Models;
using UnoContoso.Repository;
using Windows.ApplicationModel.Email;

namespace UnoContoso.ViewModels
{
    public class OrderDetailViewModel : ViewModelBase, IConfirmNavigationRequest
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

        public ICommand RemoveCommand { get; set; }

        public ICommand EmailCommand { get; set; }

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
            SaveCommand = new DelegateCommand(OnSave, () => Order != null && Order.IsModified)
                .ObservesProperty(() => Order.IsModified);
            RevertCommand = new DelegateCommand(OnRevert, () => Order != null && Order.IsModified)
                .ObservesProperty(() => Order.IsModified);
            RefreshCommand = new DelegateCommand(OnRefresh);
            RemoveCommand = new DelegateCommand<LineItem>(OnRemove);
            EmailCommand = new DelegateCommand(OnEmail);
        }

        private async void OnSave()
        {
            try
            {
                await Order.SaveOrderAsync();
            }
            catch (OrderSavingException ex)
            {
                DialogService.ShowDialog("MessageControl",
                    new DialogParameters
                    {
                        { "title", "Unable to save" },
                        { "message", $"There was an error saving your order:\n{ex.Message}"}
                    }, null);
            }
        }

        private void OnRevert()
        {
            DialogService.ShowDialog("ConfirmControl",
                new DialogParameters
                {
                    {"title", $"Save changes to Invoice # {Order.InvoiceNumber}?"},
                    {"message", $"Invoice # {Order.InvoiceNumber} " +
                        "has unsaved changes that will be lost. Do you want to save your changes?"}
                },
                async callback => 
                {
                    switch(callback.Result)
                    {
                        case ButtonResult.Yes:
                            await Order.SaveOrderAsync();
                            OnRefresh();
                            break;
                        case ButtonResult.No:
                            OnRefresh();
                            break;
                        case ButtonResult.Cancel:
                            break;
                    }
                });
        }

        private async void OnRefresh()
        {
            Order = new OrderWrapper(_contosoRepository,
                await _contosoRepository.Orders.GetAsync(Order.Id));
        }

        private async void OnEmail()
        {
            var emailMessage = new EmailMessage
            {
                Body = $"Dear {Order.CustomerName},",
                Subject = "A message from Contoso about order " +
                    $"#{Order.InvoiceNumber} placed on {Order.DatePlaced:MM/dd/yyyy}"
            };

            if (!string.IsNullOrEmpty(Order.Customer.Email))
            {
                var emailRecipient = new EmailRecipient(Order.Customer.Email);
                emailMessage.To.Add(emailRecipient);
            }

            await EmailManager.ShowComposeNewEmailAsync(emailMessage);
        }

        private void OnRemove(LineItem obj)
        {
            Order.LineItems.Remove(obj);
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
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

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            if (Order.IsModified)
            {
                DialogService.ShowDialog("ConfirmControl",
                    new DialogParameters
                    {
                        {"title", $"Save changes to Invoice # {Order.InvoiceNumber}?" },
                        {"message", $"Invoice # {Order.InvoiceNumber} " +
                            "has unsaved changes that will be lost. Do you want to save your changes?"}
                    },
                    async callback =>
                    {
                        switch (callback.Result)
                        {
                            case ButtonResult.Yes:
                                await Order.SaveOrderAsync();
                                continuationCallback.Invoke(true);
                                break;
                            case ButtonResult.No:
                                continuationCallback.Invoke(true);
                                break;
                            case ButtonResult.Cancel:
                                continuationCallback.Invoke(false);
                                break;
                        }
                    });
            }
            else
            {
                continuationCallback.Invoke(true);
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
