using System.ComponentModel;

namespace Jay.Testing;

public class ComplexEntity :
    IIdEntity<Guid>,
    INotifyPropertyChanged
{
    private string _name;
    private DateTime _updated;

    public Guid Id { get; }

    public string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }

    public DateTime Updated
    {
        get => _updated;
        set => SetField(ref _updated, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ComplexEntity()
    {
        this.Id = Guid.NewGuid();
        _name = "ABC";
        _updated = DateTime.Now;
    }
    
    public ComplexEntity(Guid id, string name)
    {
        this.Id = id;
        _name = name;
        _updated = DateTime.Now;
    }

    protected bool SetField<T>(ref T field, T value, bool force = false, [CallerMemberName] string? propertyName = null)
    {
        if (force || !EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
            this.Updated = DateTime.Now;
            return true;
        }
        return false;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new(propertyName));
    }

    public bool Equals(IEntity? entity)
    {
        return entity is ComplexEntity complexEntity && Equals(complexEntity);
    }

    public bool Equals(IIdEntity<Guid>? idEntity)
    {
        return idEntity is ComplexEntity complexEntity && Equals(complexEntity);
    }

    public bool Equals(ComplexEntity? complexEntity)
    {
        return complexEntity is not null && this.Id == complexEntity.Id;
    }

    public override bool Equals(object? obj)
    {
        return obj is ComplexEntity complexEntity && Equals(complexEntity);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override string ToString()
    {
        return Id.ToString();
    }
}