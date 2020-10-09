using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace UnoContoso.ViewModels
{
    public abstract class ViewModelBase : BindableBase, IActiveAware, INavigationAware
    {
        private string _title;

        public event EventHandler IsActiveChanged;

        /// <summary>
        /// Title
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        protected IContainerProvider ContainerProvider { get; }

        protected IEventAggregator EventAggregator { get; }

        protected IRegionManager RegionManager { get; }

        bool _isActive;
        /// <summary>
        /// IsActive
        /// </summary>
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive == value) return;
                SetProperty(ref _isActive, value);
                OnIsActiveChanged();
            }
        }

        private void OnIsActiveChanged()
        {
            //IsActiveChanged?.Invoke(this, new EventArgs());
        }

        public ViewModelBase()
        {
        }

        public ViewModelBase(IContainerProvider containerProvider)
            : this()
        {
            ContainerProvider = containerProvider;
            EventAggregator = ContainerProvider.Resolve<IEventAggregator>();
            RegionManager = ContainerProvider.Resolve<IRegionManager>();

            InitBase();
        }

        private void InitBase()
        {

        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public virtual void Destroy()
        {

        }
    }
}
