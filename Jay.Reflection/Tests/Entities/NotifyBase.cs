namespace Jay.Reflection.Tests.Entities;

public class NotifyBase : INotifyPropertyChanging, INotifyPropertyChanged
{
    public event PropertyChangingEventHandler? PropertyChanging;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool SetField<T>(ref T field, T newValue, bool force = false, [CallerMemberName] string? propertyName = null)
    {
        if (force || !EqualityComparer<T>.Default.Equals(field, newValue))
        {
            this.OnPropertyChanging(propertyName);
            field = newValue;
            this.OnPropertyChanged(propertyName);
            return true;
        }
        return false;
    }

    protected virtual void OnPropertyChanging([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanging?.Invoke(this, new(propertyName));
    }
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new(propertyName));
    }
}