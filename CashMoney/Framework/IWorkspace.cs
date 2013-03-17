namespace CashMoney.UI.Framework
{
    public interface IWorkspace
    {
        string Label { get; }
        string Icon { get; }
        string OpenChildren { get; }

        void Show();
    }
}