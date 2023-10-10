using Jay.Text.Building;

namespace Jay.Text.Scratch;

public interface IBuildingText : IDisposable
{
    IBuiltText BuiltText { get; }

    string ToStringAndDispose();
}

internal class BuildingText : BuiltText, IBuildingText
{
    public IBuiltText BuiltText => this;
    
    public BuildingText() 
        : base(TextPool.Rent())
    {
    }

    public BuildingText(int minCapacity)
        : base(TextPool.Rent(minCapacity))
    {
        
    }

    public string ToStringAndDispose()
    {
        string str = this.ToString();
        this.Dispose();
        return str;
    }

    public void Dispose()
    {
        char[]? arrayToReturn = Interlocked.Exchange<char[]?>(ref _charArray!, null);
        TextPool.Return(arrayToReturn);
    }
}