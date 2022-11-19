namespace Jay.Reflection.Fulfilling;

public interface IInstanceRefCtorImplementer
{
    ConstructorImpl ImplementInstanceReferenceConstructor(ConstructorInfo ctor);
}