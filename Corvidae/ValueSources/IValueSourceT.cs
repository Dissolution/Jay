namespace Corvidae
{

    public interface IValueSource<out TValue> : IValueSource
    {
        new TValue? Get(ISource source);
    }
}