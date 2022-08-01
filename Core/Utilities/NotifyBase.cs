using System.ComponentModel;

namespace Jay;

/// <summary>
/// A base <see langword="class"/> for <see cref="INotifyPropertyChanging"/> and <see cref="INotifyPropertyChanged"/>.
/// </summary>
public abstract class NotifyBase : INotifyPropertyChanging, INotifyPropertyChanged, IDisposable
{
    /// <inheritdoc />
    public event PropertyChangingEventHandler? PropertyChanging;
        
    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    protected NotifyBase()
    {
        
    }
    
    /// <summary>
    /// Raise the <see cref="PropertyChanging"/> event with the given <paramref name="propertyName"/>
    /// </summary>
    /// <param name="propertyName">The name of the property that's changing.</param>
    protected virtual void OnPropertyChanging([CallerMemberName] string? propertyName = null)
    {
        if (!string.IsNullOrWhiteSpace(propertyName))
        {
            this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }
    }
        
    /// <summary>
    /// Raise the <see cref="PropertyChanged"/> event with the given <paramref name="propertyName"/>
    /// </summary>
    /// <param name="propertyName">The name of the property that's changed.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        if (!string.IsNullOrWhiteSpace(propertyName))
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
        
    /// <summary>
    /// Sets the value of a <paramref name="field"/> to a given <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of value stored in the field.</typeparam>
    /// <param name="field">The field whose value is to be set.</param>
    /// <param name="value">The value to store in the field</param>
    /// <param name="force">If true, the field's value will always be set; otherwise only if the field's value is different.</param>
    /// <param name="propertyName">A <see cref="CallerMemberNameAttribute"/> that will use the calling Property's Name for the PropertyChanging and PropertyChanged event arguments.</param>
    /// <returns>True if the field's value was set</returns>
    protected bool SetField<T>(ref T field, 
        T value, 
        bool force = false, 
        [CallerMemberName] string? propertyName = null)
    {
        if (!force && EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }
        OnPropertyChanging(propertyName);
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    public virtual void Dispose()
    {
        this.PropertyChanging = null;
        this.PropertyChanged = null;
    }
}