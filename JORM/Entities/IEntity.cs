namespace JORM.Entities;

public interface IEntity : IEquatable<IEntity>
{
    
}

public interface IKeyedEntity<TKey> : IEntity, IEquatable<IKeyedEntity<TKey>>
    where TKey : IEqualityOperators<TKey, TKey>
{
    static abstract bool operator ==(IKeyedEntity<TKey> firstEntity, IKeyedEntity<TKey> secondEntity);
    static abstract bool operator !=(IKeyedEntity<TKey> firstEntity, IKeyedEntity<TKey> secondEntity);
    
    TKey Key { get; init; }

    /// <inheritdoc />
    bool IEquatable<IEntity>.Equals(IEntity? entity)
    {
        return entity is IKeyedEntity<TKey> keyedEntity && Key == keyedEntity.Key;
    }

    /// <inheritdoc />
    bool IEquatable<IKeyedEntity<TKey>>.Equals(IKeyedEntity<TKey>? keyedEntity)
    {
        return keyedEntity is not null && Key == keyedEntity.Key;
    }
}