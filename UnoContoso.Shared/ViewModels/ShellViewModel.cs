﻿using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UnoContoso.EventArgs;
using UnoContoso.Events;
using UnoContoso.Models.Consts;
using UnoContoso.Models.System;
using UnoContoso.Views;

namespace UnoContoso.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        private NavigationMenuItem _selectedItem;
        /// <summary>
        /// Selected NavigationMenuItem
        /// </summary>
        public NavigationMenuItem SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }

        private IList<NavigationMenuItem> _menus;
        private readonly IDialogService _dialogService;

        /// <summary>
        /// Menus
        /// </summary>
        public IList<NavigationMenuItem> Menus
        {
            get { return _menus; }
            set { SetProperty(ref _menus, value); }
        }

        /// <summary>
        /// Busy list
        /// </summary>
        private readonly IList<BusyEventArgs> _busies = new List<BusyEventArgs>();


        public ShellViewModel()
        {
        }

        public ShellViewModel(IContainerProvider containerProvider,
            IDialogService dialogService)
            : base(containerProvider)
        {
            Title = "Shell";
            _dialogService = dialogService;
            Menus = new List<NavigationMenuItem>
            {
                new NavigationMenuItem{ Name = "Home", Content = "Home", Icon = "Home", Path = "HomeView"},
                new NavigationMenuItem{ Name = "Customer", Content = "Customer", Icon = "ContactInfo", Path = "CustomerListView"},
                new NavigationMenuItem{ Name = "Order", Content = "Order", Icon = "Shop", Path = "OrderListView"},
            };

            SelectedItem = Menus.First();

            //Start page
            RegionManager.RegisterViewWithRegion(Regions.CONTENT_REGION, typeof(HomeView));

            EventSubscribe();

            PropertyChanged += ShellViewModel_PropertyChanged;
        }

        private void ShellViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SelectedItem):
                    if (SelectedItem == null) return;
                    RegionManager.RequestNavigate(Regions.CONTENT_REGION, SelectedItem.Path);
                    break;
            }
        }

        private void EventSubscribe()
        {
            EventAggregator.GetEvent<MessageEvent>()
                .Subscribe(ReceivedMessageEvent);
            EventAggregator.GetEvent<BusyEvent>()
                .Subscribe(ReceivedBusyEvent, ThreadOption.UIThread, false);
        }

        private void ReceivedBusyEvent(BusyEventArgs obj)
        {
        }

        private void ReceivedMessageEvent(MessageEventArgs obj)
        {
        }
    }
}
