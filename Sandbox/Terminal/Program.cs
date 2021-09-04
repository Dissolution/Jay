using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using Jay;
using Jay.Debugging;

namespace Jay.Sandbox
{
    internal static class Program
    {
        public static int Main(params string?[] args)
        {
            var testEnumTypes = Assembly.GetExecutingAssembly()
                    .ExportedTypes
                    .Where(type => type.Name.StartsWith("TestEnum", StringComparison.OrdinalIgnoreCase))
                    .ToList();
            foreach (var type in testEnumTypes)
            {
                var method = typeof(Enums<>).MakeGenericType(type)
                                            .GetMethod(nameof(Enums<TestEnumByte>.ULong), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                var eValue = Enum.ToObject(type, 3);
                var value = method.Invoke(null, new object?[] {eValue});
            
                Hold.Debug(eValue, value);
            }
            


            // Do not close the Console window automatically
            Console.WriteLine("Press Enter to close");
            Console.ReadLine();
            
            // Return success code
            return 0;
        }
    }
}