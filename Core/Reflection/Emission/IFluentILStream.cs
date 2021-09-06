namespace Jay.Reflection.Emission
{
    public interface IFluentILStream<TBuilder> : IMSILStream
        where TBuilder : IFluentILStream<TBuilder>
    {
        TBuilder Append(IMSILStream ilStream);
    }
    
    
}