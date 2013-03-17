using System.Collections.Generic;
using CashMoney.UI.Framework;
using Caliburn.Micro;

namespace CashMoney.UI.MainWindow
{
    public class MainWindowViewModel : Conductor<IWorkspace>.Collection.OneActive, IMainWindow
    {
        private static readonly ILog Log = LogManager.GetLog(typeof (MainWindowViewModel));

        public MainWindowViewModel(IEnumerable<IWorkspace> workspaces)
        {
            Log.Info("Main Window initialised successfully");
            Items.AddRange(workspaces);
        }
    }
}
