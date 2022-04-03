using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jay.Collections;

namespace Jay.Reflection.Cloning;

public class BinaryEncoder
{
    private delegate void EncodeDelegate<T>(T value, BinaryWriter writer);

    private static ConcurrentTypeDictionary<Delegate> _cache;

    static BinaryEncoder()
    {
        _cache = new();
    }




    public static void Encode<T>(T value, BinaryWriter writer)
    {

    }
}