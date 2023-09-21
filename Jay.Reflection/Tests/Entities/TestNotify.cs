namespace Jay.Reflection.Tests.Entities;

public class TestNotify : NotifyBase
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

    public TestNotify()
    {
        _id = EntityGenerator.New<int>();
        _name = EntityGenerator.New<string>();
    }
}