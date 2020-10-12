using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace UnoContoso.ViewModels
{
    public class OrderDetailViewModel : ViewModelBase
    {
        public OrderDetailViewModel()
        {

        }

        public OrderDetailViewModel(IContainerProvider containerProvider)
            : base(containerProvider)
        {
            Title = "Order Details";
            Init();
        }

        private void Init()
        {
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            var id = navigationContext.Parameters.GetValue<Guid>("CustomerId");
            Title = $"{Title} / {id}";
        }
    }
}
