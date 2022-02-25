using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Jay.Tests.Entities;

public class TestClass : INotifyPropertyChanged,
                         IEquatable<TestClass>
{
    private int _id;
    private string _name;

    public int Id
    {
        get => _id;
        set => SetField(ref _id, value);
    }
    
    public string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    public TestClass(int id, string name)
    {
        _id = id;
        _name = name;
    }
    
    protected virtual bool SetField<TField>(ref TField field, TField value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<TField>.Default.Equals(field, value))
            return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <inheritdoc />
    public bool Equals(TestClass? testClass)
    {
        if (testClass is null) return false;
        return _id == testClass._id;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is TestClass testClass && Equals(testClass);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return _id;
    }
}