using System;
using System.Collections.Generic;
using Caliburn.Micro;
using CashMoney.UI.Framework;
using CashMoney.UI.Utils;
using SimpleInjector;

namespace CashMoney.UI.MainWindow
{
    public class CashMoneyBootstrapper : Bootstrapper<IMainWindow>
    {
        private Container _container;

        static CashMoneyBootstrapper()
        {
            LogManager.GetLog = type => new Log4netLogger(type);
        }

        protected override void Configure()
        {
            _container = new Container();

            _container.RegisterSingle<IWindowManager, WindowManager>();
            _container.RegisterSingle<IEventAggregator, EventAggregator>();

            _container.RegisterSingle<IMainWindow, MainWindowViewModel>();

            _container.Verify();
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            if (String.IsNullOrEmpty(key) == false)
            {
                throw new Exception("GetInstance using a String is not supported. Use a type instead.");
            }

            var instance = _container.GetInstance(serviceType);

            if (instance == null)
            {
                throw new Exception(string.Format("Could not locate any instances of type {0}.", serviceType));
            }

            return instance;
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.GetAllInstances(serviceType);
        }

        //TODO could put close confirmation logic in here. See HellowScreens sample for an example.
    }
}