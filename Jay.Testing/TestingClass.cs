namespace Jay.Testing;

public interface IEntity : IEquatable<IEntity>
{
}

public interface IIdEntity<TId> : IEntity, IEquatable<IIdEntity<TId>>
    where TId : IEquatable<TId>
{
    TId Id { get; }
}