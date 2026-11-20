using System.ComponentModel;
using System.Runtime.CompilerServices;

public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    // The third parameter contains the member name which calls this method
    // for example in Address property "SetProperty(ref _address, value)"
    // even though, calling it without the third parameter, but [CallerMemberName]
    // automatically contains "Address" property name. Without that attribute,
    // it is just null
    protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = default)
    {
        field = value;
        
        RaiseProperty(name); 
    }

    protected void RaiseProperty([CallerMemberName] string? name = default) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}