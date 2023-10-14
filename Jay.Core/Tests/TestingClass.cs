using Bogus;

namespace Jay.Tests;

public class TestingClass : IEquatable<TestingClass>
{
    public Guid Id { get; }
    public string Name { get; set; }

    public TestingClass()
    {
        this.Id = Guid.NewGuid();
        this.Name = Id.ToString();
    }

    public TestingClass(Guid id, string name)
    {
        this.Id = id;
        this.Name = name;
    }

    public bool Equals(TestingClass? other)
    {
        return other is not null && other.Id == this.Id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is TestingClass testingClass)
            return Equals(testingClass);

        return false;
    }

    public override int GetHashCode()
    {
        return this.Id.GetHashCode();
    }

    public override string ToString()
    {
        return Name;
    }
}