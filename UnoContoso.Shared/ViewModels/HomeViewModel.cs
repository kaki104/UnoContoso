﻿using Prism.Commands;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using UnoContoso.Models;
using UnoContoso.Repository;

namespace UnoContoso.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly IContosoRepository _contosoRepository;

        public ICommand TestCommand { get; set; }

        private Customer _customer;
        public Customer Customer
        {
            get { return _customer; }
            set { SetProperty(ref _customer, value); }
        }

        private string _result;

        public string Result
        {
            get { return _result; }
            set { SetProperty(ref _result, value); }
        }


        public HomeViewModel()
        {
        }

        public HomeViewModel(IContainerProvider containerProvider,
            IContosoRepository contosoRepository)
            : base(containerProvider)
        {
            Title = "Home!";
            _contosoRepository = contosoRepository;

            Init();
        }

        private void Init()
        {
            TestCommand = new DelegateCommand(OnTest);
        }

        private async void OnTest()
        {
            var result = await _contosoRepository.Customers
                .GetJObjectAsync(Guid.Parse("E7E9AF6E-503A-444B-B596-004C32DE93BF"));
            Result = result.ToString();
            Customer = result.ToObject<Customer>();
        }
    }
}
