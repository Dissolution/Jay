namespace JaySON;

public interface IJObject
{
    
}

public interface IJArray
{
    
}


public enum JValueType
{
    String,
    Number,
    JObject,
    JArray,
    True,
    False,
    Null,
}

public interface IJValue
{
    JValueType JValueType { get; }
}

public interface IJString : IJValue
{
    JValueType IJValue.JValueType => JValueType.String;
}