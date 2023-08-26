using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using Jay.Reflection.Extensions;
using Jay.Reflection.Searching;

namespace Jay.Reflection.Tests.Adapters;

public class TestEvents
{
    [Fact]
    public void TestAddHandler()
    {
        uint counter = 0U;
        var testNotify = new TestNotify();
        
        testNotify.PropertyChanged += notifyOnPropertyChanged;
        Assert.Equal(0U, counter);
        testNotify.Id = TestGenerator.Int();
        Assert.Equal(1U, counter);
        
        testNotify.PropertyChanged -= notifyOnPropertyChanged;
        Assert.Equal(1U, counter);
        testNotify.Id = TestGenerator.Int();
        Assert.Equal(1U, counter);

        var propertyChangedEvent = typeof(TestNotify)
            .GetEvents(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .Where(e => e.Name == "PropertyChanged")
            .OneOrDefault();
        
        //var propertyChangedEvent = MemberSearch.One<TestNotify, EventInfo>("PropertyChanged");
        
        
        propertyChangedEvent!.AddHandler(ref testNotify, notifyOnPropertyChanged);
        Assert.Equal(1U, counter);
        testNotify.Id = TestGenerator.Int();
        Assert.Equal(2U, counter);
        return;


        void notifyOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Interlocked.Increment(ref counter);
            // Console.WriteLine($"{sender} : {e.PropertyName}");
        }
    }

}