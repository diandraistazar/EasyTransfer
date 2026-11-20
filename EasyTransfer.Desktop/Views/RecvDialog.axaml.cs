using Avalonia.Controls;

using EasyTransfer.Core.Helpers;
using EasyTransfer.Core.Logging;

namespace EasyTransfer.Desktop.Views;

public partial class RecvDialog : UserControl
{
    public RecvDialog()
    {
        InitializeComponent();

        AddressInput.TextChanging += OnAddressInputChanging;
        PortInput.TextChanging += OnPortInputChanging;
    }

    public void OnAddressInputChanging(object? sender, TextChangingEventArgs? e)
    {
        if (sender != null && e != null && sender is TextBox obj)
        {
            obj.Text = StringHelper.RemoveNonNumeric(obj.Text, ".".ToCharArray());
        }
    }

    public void OnPortInputChanging(object? sender, TextChangingEventArgs e)
    {
        if (sender != null && e != null && sender is TextBox obj)
        {
            obj.Text = StringHelper.RemoveNonNumeric(obj.Text, null);

            if(obj.Text == string.Empty)
                obj.Text = "0";
        }
    }
}